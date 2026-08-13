using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CmdSender
{
    public partial class MainForm : Form
    {
        #region 字段

        private IntPtr _targetHandle = IntPtr.Zero;
        private readonly Scheduler _scheduler;
        private bool _isSending = false;
        private bool _isDragging = false;
        private string _currentFilePath = null;
        private bool _isDirty = false;
        private bool _closing = false;
        private AppSettings _settings = new AppSettings();

        #endregion

        #region 构造与初始化

        public MainForm()
        {
            InitializeComponent();
            _scheduler = new Scheduler();
            WireSchedulerEvents();

            // 会话记忆：恢复上次参数与窗口状态
            _settings = SettingsStore.Load();
            RestoreUiState();

            // 加载窗口图标（标题栏用）
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                    this.Icon = new Icon(iconPath);
            }
            catch { /* ignore icon load errors */ }

            // 最后打开的文件
            if (!string.IsNullOrEmpty(_settings.LastFile) && File.Exists(_settings.LastFile))
            {
                try { OpenFileAt(_settings.LastFile); } catch { }
            }

            UpdateTargetDisplay();
        }

        /// <summary>
        /// 从设置恢复：窗口位置、循环参数、发送方式、回车选项。
        /// </summary>
        private void RestoreUiState()
        {
            if (_settings.WindowX >= 0 && _settings.WindowWidth > 0)
            {
                var rect = new Rectangle(_settings.WindowX, _settings.WindowY,
                    _settings.WindowWidth, _settings.WindowHeight);
                if (Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(rect)))
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Bounds = rect;
                    if (_settings.Maximized)
                        this.WindowState = FormWindowState.Maximized;
                }
            }

            numericUpDownLineInterval.Value = Clamp(_settings.LineInterval,
                numericUpDownLineInterval.Minimum, numericUpDownLineInterval.Maximum);
            numericUpDownCycleInterval.Value = Clamp(_settings.CycleInterval,
                numericUpDownCycleInterval.Minimum, numericUpDownCycleInterval.Maximum);
            numericUpDownCycleCount.Value = Clamp(_settings.CycleCount,
                numericUpDownCycleCount.Minimum, numericUpDownCycleCount.Maximum);
            comboBoxSendMethod.SelectedIndex = _settings.SendMethod >= 0 && _settings.SendMethod < comboBoxSendMethod.Items.Count
                ? _settings.SendMethod
                : 1; // 默认前台发送，兼容性最佳，点击即达
            checkBoxSendEnter.Checked = _settings.SendEnter;
        }

        private static decimal Clamp(int value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void WireSchedulerEvents()
        {
            _scheduler.OnCommandSent += (s, args) => SafeInvoke(() =>
            {
                string cycleInfo = args.TotalCycles > 0
                    ? $"第 {args.CycleNumber}/{args.TotalCycles} 轮"
                    : $"第 {args.CycleNumber} 轮";
                lblStatus.Text = $"发送中: {cycleInfo} 第 {args.LineNumber}/{args.TotalLines} 行 | {TruncateDisplay(args.Command, 60)}";
                lblCounter.Text = args.EstimatedRemainingSeconds.HasValue
                    ? $"已发 {args.TotalSent} 条 · 预计剩余 {args.EstimatedRemainingSeconds} 秒"
                    : $"已发 {args.TotalSent} 条 · 无限循环";
            });
            _scheduler.OnStatusChanged += (s, msg) => SafeInvoke(() => lblStatus.Text = msg);
            _scheduler.OnCompleted += (s, e) => SafeInvoke(() =>
            {
                // 仅在调度器确实已停止时才恢复 UI
                if (!_scheduler.IsRunning)
                {
                    ToggleCycleControl(false);
                    lblCounter.Text = "";
                }
            });
        }

        /// <summary>
        /// 跨线程安全地更新 UI。窗体关闭或已销毁时静默跳过。
        /// </summary>
        private void SafeInvoke(Action action)
        {
            if (_closing || IsDisposed || !IsHandleCreated) return;
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (InvalidOperationException) { }
        }

        #endregion

        #region 文件操作

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (_isDirty && !ConfirmSave()) return;
            richTextBoxContent.Clear();
            _currentFilePath = null;
            _isDirty = false;
            UpdateTitle();
            UpdateStatus("已新建文件");
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (_isDirty && !ConfirmSave()) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                ofd.Title = "打开文本文件";
                if (!string.IsNullOrEmpty(_settings.LastDirectory) && Directory.Exists(_settings.LastDirectory))
                    ofd.InitialDirectory = _settings.LastDirectory;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenFileAt(ofd.FileName);
                }
            }
        }

        /// <summary>
        /// 打开文本文件（自动识别 UTF-8 / GBK / Unicode 编码）。
        /// </summary>
        private void OpenFileAt(string path)
        {
            try
            {
                richTextBoxContent.Text = ReadTextSmart(path);
                _currentFilePath = path;
                _settings.LastDirectory = Path.GetDirectoryName(path);
                _isDirty = false;
                UpdateTitle();
                UpdateStatus($"已打开: {path}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开文件失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 智能读取文本：优先 BOM 识别，其次严格 UTF-8，失败回退 GBK。
        /// </summary>
        private static string ReadTextSmart(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);

            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

            try
            {
                // 严格 UTF-8：遇到非法字节抛异常 → 说明不是 UTF-8
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                // 中文 Windows 常见 GBK/ANSI 编码
                return Encoding.GetEncoding(936).GetString(bytes);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                btnSaveAs_Click(sender, e);
            }
            else
            {
                SaveToFile(_currentFilePath);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                sfd.Title = "另存为";
                if (!string.IsNullOrEmpty(_currentFilePath))
                {
                    sfd.FileName = Path.GetFileName(_currentFilePath);
                    sfd.InitialDirectory = Path.GetDirectoryName(_currentFilePath) ?? "";
                }
                else if (!string.IsNullOrEmpty(_settings.LastDirectory) && Directory.Exists(_settings.LastDirectory))
                {
                    sfd.InitialDirectory = _settings.LastDirectory;
                }
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveToFile(sfd.FileName);
                }
            }
        }

        private void SaveToFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, richTextBoxContent.Text, Encoding.UTF8);
                _currentFilePath = filePath;
                _settings.LastDirectory = Path.GetDirectoryName(filePath);
                _isDirty = false;
                UpdateTitle();
                UpdateStatus($"已保存: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ConfirmSave()
        {
            DialogResult result = MessageBox.Show("有未保存的更改，是否保存？", "确认",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                btnSave_Click(this, EventArgs.Empty);
                return !_isDirty; // 保存被取消则阻止后续操作
            }
            return result == DialogResult.No;
        }

        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(_currentFilePath)
                ? "新建文件"
                : Path.GetFileName(_currentFilePath);
            string dirty = _isDirty ? " *" : "";
            this.Text = $"窗口命令发送器 - {fileName}{dirty}";
        }

        #endregion

        #region 拖放文件打开

        private void richTextBoxContent_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0 &&
                    (files[0].EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                     files[0].EndsWith(".log", StringComparison.OrdinalIgnoreCase) ||
                     files[0].EndsWith(".cmd", StringComparison.OrdinalIgnoreCase)))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void richTextBoxContent_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    if (_isDirty && !ConfirmSave()) return;
                    OpenFileAt(files[0]);
                }
            }
        }

        #endregion

        #region 快捷键

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && !e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.N: btnNew_Click(this, EventArgs.Empty); e.Handled = true; return;
                    case Keys.O: btnOpen_Click(this, EventArgs.Empty); e.Handled = true; return;
                    case Keys.S:
                        if (e.Shift) btnSaveAs_Click(this, EventArgs.Empty);
                        else btnSave_Click(this, EventArgs.Empty);
                        e.Handled = true; return;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.F5:
                    if (btnSingleSend.Enabled) btnSingleSend_Click(this, EventArgs.Empty);
                    e.Handled = true; return;
                case Keys.F6:
                    btnCycleControl_Click(this, EventArgs.Empty);
                    e.Handled = true; return;
            }

            base.OnKeyDown(e);
        }

        #endregion

        #region 窗口选择（拖动）

        private void btnSelectWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                btnSelectWindow.Capture = true;
                Cursor.Current = Cursors.Cross;
                lblHandle.Text = "句柄: (拖动中...)";
                lblWindowTitle.Text = "标题: (拖动中...)";
                lblWindowClass.Text = "类名: (拖动中...)";
            }
        }

        private void btnSelectWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            Point screenPoint = Cursor.Position;
            IntPtr hWnd = CommandSender.GetWindowFromPoint(screenPoint);

            if (IsOwnWindow(hWnd))
            {
                lblHandle.Text = "句柄: (跳过自身窗口)";
                return;
            }

            if (hWnd != IntPtr.Zero)
            {
                lblHandle.Text = $"句柄: 0x{hWnd.ToInt64():X8}";
                string title = CommandSender.GetWindowTitle(hWnd);
                string className = CommandSender.GetWindowClassName(hWnd);
                lblWindowTitle.Text = $"标题: {TruncateDisplay(title, 25)}";
                lblWindowClass.Text = $"类名: {TruncateDisplay(className, 25)}";
            }
        }

        private void btnSelectWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            _isDragging = false;
            btnSelectWindow.Capture = false;
            Cursor.Current = Cursors.Default;

            Point screenPoint = Cursor.Position;
            IntPtr hWnd = CommandSender.GetWindowFromPoint(screenPoint);

            if (hWnd != IntPtr.Zero && !IsOwnWindow(hWnd))
            {
                _targetHandle = hWnd;
                UpdateTargetDisplay();
                UpdateStatus($"已捕获目标窗口: 0x{hWnd.ToInt64():X8}");
            }
            else
            {
                UpdateTargetDisplay();
                UpdateStatus("未选择有效目标窗口");
            }
        }

        /// <summary>
        /// 判断句柄是否属于本应用程序窗口。
        /// </summary>
        private bool IsOwnWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return true;
            if (hWnd == this.Handle) return true;
            // Control.FromHandle 仅返回本进程内的托管控件
            Control ctrl = Control.FromHandle(hWnd);
            if (ctrl != null && ctrl.TopLevelControl == this) return true;
            return false;
        }

        /// <summary>
        /// 刷新目标窗口信息显示，并根据有效性着色（有效绿色 / 无效灰色）。
        /// </summary>
        private void UpdateTargetDisplay()
        {
            bool valid = _targetHandle != IntPtr.Zero && CommandSender.IsWindowValid(_targetHandle);
            Color color = valid ? Color.FromArgb(16, 124, 16) : Color.Gray;

            lblHandle.ForeColor = color;
            lblWindowTitle.ForeColor = color;
            lblWindowClass.ForeColor = color;
            lblTarget.ForeColor = color;

            if (valid)
            {
                string title = CommandSender.GetWindowTitle(_targetHandle);
                lblHandle.Text = $"句柄: 0x{_targetHandle.ToInt64():X8}";
                lblWindowTitle.Text = $"标题: {TruncateDisplay(title, 25)}";
                lblWindowClass.Text = $"类名: {TruncateDisplay(CommandSender.GetWindowClassName(_targetHandle), 25)}";
                lblTarget.Text = $"目标: {TruncateDisplay(title, 14)} 0x{_targetHandle.ToInt64():X8}";
            }
            else
            {
                lblHandle.Text = _targetHandle == IntPtr.Zero ? "句柄: 0x00000000" : "句柄: (窗口已关闭)";
                lblWindowTitle.Text = _targetHandle == IntPtr.Zero ? "标题: (未选择)" : "标题: (窗口已关闭)";
                lblWindowClass.Text = _targetHandle == IntPtr.Zero ? "类名: (未选择)" : "类名: (窗口已关闭)";
                lblTarget.Text = "目标: 无";
            }
        }

        private string TruncateDisplay(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s)) return "(空)";
            return s.Length > maxLen ? s.Substring(0, maxLen) + "..." : s;
        }

        #endregion

        #region 发送控制

        private void btnSingleSend_Click(object sender, EventArgs e)
        {
            if (!ValidateTargetWindow()) return;

            string currentLine = GetCurrentLine();
            if (string.IsNullOrWhiteSpace(currentLine))
            {
                MessageBox.Show("当前行为空，无内容可发送", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool sendEnter = checkBoxSendEnter.Checked;
            SendMethod method = (SendMethod)comboBoxSendMethod.SelectedIndex;

            try
            {
                if (method == SendMethod.SendInput)
                {
                    CommandSender.SendBySendInput(_targetHandle, currentLine, sendEnter);
                }
                else
                {
                    CommandSender.SendByPostMessage(_targetHandle, currentLine, sendEnter);
                }
                UpdateStatus($"单次发送完成: {currentLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发送失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCycleControl_Click(object sender, EventArgs e)
        {
            if (_isSending)
            {
                StopSending();
            }
            else
            {
                StartSending();
            }
        }

        private void StartSending()
        {
            if (!ValidateTargetWindow()) return;

            string[] commands = GetCommandsForLoop();
            if (commands.Length == 0)
            {
                MessageBox.Show("没有可发送的命令（请确保文本中包含非空行）", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SchedulerConfig config = new SchedulerConfig
            {
                LineInterval = (int)numericUpDownLineInterval.Value,
                CycleInterval = (int)numericUpDownCycleInterval.Value,
                CycleCount = (int)numericUpDownCycleCount.Value,
                SendEnter = checkBoxSendEnter.Checked,
                Method = (SendMethod)comboBoxSendMethod.SelectedIndex
            };

            ToggleCycleControl(true);
            _scheduler.Start(_targetHandle, commands, config);

            string cycleDesc = config.CycleCount > 0 ? $"{config.CycleCount} 轮" : "无限循环";
            UpdateStatus($"开始循环发送: {commands.Length} 条命令, {cycleDesc}");
            lblCounter.Text = "已发 0 条";
        }

        private void StopSending()
        {
            _scheduler.Stop();
            UpdateStatus("正在停止发送...");
        }

        /// <summary>
        /// 切换循环发送的 UI 状态。跨线程安全。
        /// </summary>
        private void ToggleCycleControl(bool isStart)
        {
            _isSending = isStart;
            btnCycleControl.Text = isStart ? "⏹  停止循环" : "🔁  开始循环";
            btnCycleControl.BackColor = isStart ? Color.FromArgb(192, 0, 0) : Color.FromArgb(16, 124, 16);

            // 发送期间禁用参数编辑，避免运行时修改造成不一致
            numericUpDownLineInterval.Enabled = !isStart;
            numericUpDownCycleInterval.Enabled = !isStart;
            numericUpDownCycleCount.Enabled = !isStart;
            comboBoxSendMethod.Enabled = !isStart;
            checkBoxSendEnter.Enabled = !isStart;
            btnSingleSend.Enabled = !isStart;
        }

        private bool ValidateTargetWindow()
        {
            if (_targetHandle != IntPtr.Zero && CommandSender.IsWindowValid(_targetHandle))
                return true;

            MessageBox.Show("请先拖动选择有效的目标窗口", "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        /// <summary>
        /// 获取光标所在行的文本。
        /// </summary>
        private string GetCurrentLine()
        {
            int lineIndex = richTextBoxContent.GetLineFromCharIndex(richTextBoxContent.SelectionStart);
            string[] lines = richTextBoxContent.Lines;
            return lines.Length > lineIndex ? lines[lineIndex] : "";
        }

        /// <summary>
        /// 获取循环发送的命令列表。
        /// 如果有选中文本，使用选中的行；否则使用全部行。
        /// 自动过滤空行和纯空白行。
        /// </summary>
        private string[] GetCommandsForLoop()
        {
            string text;
            if (richTextBoxContent.SelectionLength > 0)
            {
                text = richTextBoxContent.SelectedText;
            }
            else
            {
                text = richTextBoxContent.Text;
            }

            return text
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
        }

        #endregion

        #region 状态更新与文本编辑器事件

        private void UpdateStatus(string message)
        {
            SafeInvoke(() => lblStatus.Text = message);
        }

        private void richTextBoxContent_TextChanged(object sender, EventArgs e)
        {
            if (!_isDirty)
            {
                _isDirty = true;
                UpdateTitle();
            }
        }

        private void richTextBoxContent_SelectionChanged(object sender, EventArgs e)
        {
            int line = richTextBoxContent.GetLineFromCharIndex(richTextBoxContent.SelectionStart) + 1;
            int firstChar = richTextBoxContent.GetFirstCharIndexOfCurrentLine();
            int col = richTextBoxContent.SelectionStart - firstChar + 1;
            lblPosition.Text = $"行: {line}, 列: {col}";
        }

        #endregion

        #region 窗体关闭

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _closing = true;

            // 先停止调度器并等待其退出，避免后台线程在窗体销毁后触发事件
            _scheduler?.WaitForStop(1000);

            // 检查未保存的更改
            if (_isDirty)
            {
                DialogResult result = MessageBox.Show("有未保存的更改，是否保存？", "确认",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    btnSave_Click(this, EventArgs.Empty);
                    if (_isDirty) e.Cancel = true; // 保存被取消
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                SaveSettings();
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// 保存会话记忆：窗口位置、循环参数、发送选项、最后文件。
        /// </summary>
        private void SaveSettings()
        {
            Rectangle bounds = this.WindowState == FormWindowState.Normal ? this.Bounds : this.RestoreBounds;

            _settings.WindowX = bounds.X;
            _settings.WindowY = bounds.Y;
            _settings.WindowWidth = bounds.Width;
            _settings.WindowHeight = bounds.Height;
            _settings.Maximized = this.WindowState == FormWindowState.Maximized;
            _settings.LineInterval = (int)numericUpDownLineInterval.Value;
            _settings.CycleInterval = (int)numericUpDownCycleInterval.Value;
            _settings.CycleCount = (int)numericUpDownCycleCount.Value;
            _settings.SendEnter = checkBoxSendEnter.Checked;
            _settings.SendMethod = comboBoxSendMethod.SelectedIndex;
            _settings.LastFile = _currentFilePath;

            SettingsStore.Save(_settings);
        }

        #endregion
    }
}
