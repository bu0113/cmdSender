using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

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

        #endregion

        #region 构造与初始化

        public MainForm()
        {
            InitializeComponent();
            _scheduler = new Scheduler();
            WireSchedulerEvents();
            comboBoxSendMethod.SelectedIndex = 1; // 默认前台发送，兼容性最佳，点击即达

            // 加载窗口图标（标题栏用）
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(iconPath))
                    this.Icon = new Icon(iconPath);
            }
            catch { /* ignore icon load errors */ }
        }

        private void WireSchedulerEvents()
        {
            _scheduler.OnCommandSent += (s, args) =>
            {
                string cycleInfo = args.TotalCycles > 0
                    ? $"第 {args.CycleNumber}/{args.TotalCycles} 轮"
                    : $"第 {args.CycleNumber} 轮";
                UpdateStatus($"发送中: {cycleInfo}, 第 {args.LineNumber}/{args.TotalLines} 行 | {args.Command}");
            };
            _scheduler.OnStatusChanged += (s, msg) => UpdateStatus(msg);
            _scheduler.OnCompleted += (s, e) =>
            {
                // 仅在调度器确实已停止时才恢复 UI
                if (!_scheduler.IsRunning)
                {
                    ToggleCycleControl(false);
                }
            };
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
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        richTextBoxContent.Text = File.ReadAllText(ofd.FileName, Encoding.UTF8);
                        _currentFilePath = ofd.FileName;
                        _isDirty = false;
                        UpdateTitle();
                        UpdateStatus($"已打开: {ofd.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"打开文件失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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
                string title = CommandSender.GetWindowTitle(hWnd);
                string className = CommandSender.GetWindowClassName(hWnd);
                lblHandle.Text = $"句柄: 0x{hWnd.ToInt64():X8}";
                lblWindowTitle.Text = $"标题: {TruncateDisplay(title, 25)}";
                lblWindowClass.Text = $"类名: {TruncateDisplay(className, 25)}";
                UpdateStatus($"已捕获目标窗口: 0x{hWnd.ToInt64():X8}");
            }
            else
            {
                // 恢复之前的选择或显示默认值
                if (_targetHandle != IntPtr.Zero)
                {
                    lblHandle.Text = $"句柄: 0x{_targetHandle.ToInt64():X8}";
                }
                else
                {
                    lblHandle.Text = "句柄: 0x00000000";
                    lblWindowTitle.Text = "标题: (未选择)";
                    lblWindowClass.Text = "类名: (未选择)";
                }
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
                if (method == SendMethod.SendKeys)
                {
                    CommandSender.SendBySendKeys(_targetHandle, currentLine, sendEnter);
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
        }

        private void StopSending()
        {
            _scheduler.Stop();
            UpdateStatus("正在停止发送...");
        }

        /// <summary>
        /// 切换循环发送的 UI 状态。
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
            if (statusStrip.InvokeRequired)
            {
                statusStrip.BeginInvoke((Action)(() => lblStatus.Text = message));
            }
            else
            {
                lblStatus.Text = message;
            }
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
            // 先停止调度器
            _scheduler?.Stop();

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

            base.OnFormClosing(e);
        }

        #endregion
    }
}
