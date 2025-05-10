using System;
using System.Windows.Forms;
using System.IO;
using cmdSender;
using System.Drawing;

namespace CmdSender
{
    public partial class MainForm : Form
    {
        private IntPtr _targetHandle = IntPtr.Zero;
        private Scheduler _scheduler;
        private bool _isSending = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeScheduler();
            SetupDefaults();
        }

        private void SetupDefaults()
        {
            numericUpDownLineInterval.Value = 100;    // 行间隔默认100ms
            numericUpDownCycleInterval.Value = 2000;  // 循环间隔默认2000ms
            numericUpDownCycleCount.Value = 0;        // 循环次数默认0（无限）
        }

        private void InitializeScheduler()
        {
            _scheduler = new Scheduler();
            _scheduler.OnCommandSent += (s, line) =>
                UpdateStatus($"已发送第 {line} 行");
            _scheduler.OnCompleted += (s, e) =>
                ToggleCycleControl(false);
        }

        // 窗口选择功能
        private void btnSelectWindow_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void btnSelectWindow_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            Point screenPoint = PointToScreen(e.Location);
            _targetHandle = CommandSender.GetWindowFromPoint(screenPoint);
            lblHandle.Text = $"0x{_targetHandle.ToInt64():X8}";
            UpdateStatus("已捕获目标窗口");
        }

        // 文件操作
        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    richTextBoxContent.LoadFile(ofd.FileName, RichTextBoxStreamType.PlainText);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            { 
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    richTextBoxContent.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            richTextBoxContent.Clear();
        }

        // 发送控制
        private void btnSingleSend_Click(object sender, EventArgs e)
        {
            if (!ValidateTargetWindow()) return;

            string currentLine = GetCurrentLine();
            CommandSender.AppendText(_targetHandle, currentLine);
            UpdateStatus("单次发送完成");
        }

        private void btnCycleControl_Click(object sender, EventArgs e)
        {
            ToggleCycleControl(!_isSending);
        }

        private bool ValidateTargetWindow()
        {
            if (_targetHandle != IntPtr.Zero) return true;

            MessageBox.Show("请先选择目标窗口", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private string GetCurrentLine()
        {
            int lineIndex = richTextBoxContent.GetLineFromCharIndex(richTextBoxContent.SelectionStart);
            return richTextBoxContent.Lines.Length > lineIndex ?
                richTextBoxContent.Lines[lineIndex] : "";
        }

        private void ToggleCycleControl(bool isStart)
        {
            _isSending = isStart;
            btnCycleControl.Text = isStart ? "停止循环" : "开始循环";

            if (isStart)
            {
                StartSendingCycle();
            }
            else
            {
                StopSendingCycle();
            }
        }

        private void StartSendingCycle()
        {
            SchedulerConfig config = new SchedulerConfig
            {
                LineInterval = (int)numericUpDownLineInterval.Value,
                CycleInterval = (int)numericUpDownCycleInterval.Value,
                CycleCount = (int)numericUpDownCycleCount.Value
            };
            _scheduler.Start(_targetHandle, richTextBoxContent.Lines, config);
        }

        private void StopSendingCycle()
        {
            _scheduler.Stop();
            UpdateStatus("已停止发送");
        }
         
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

        // 引用处必须注释掉，否则会导致拖动时无法精准选定窗口
        private void btnSelectWindow_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void numericUpDownCycleCount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutBottom_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBoxContent_TextChanged(object sender, EventArgs e)
        {

        }
    }
}