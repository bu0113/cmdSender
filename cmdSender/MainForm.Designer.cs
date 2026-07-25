using System.Windows.Forms;

namespace CmdSender
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // 顶部工具栏
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveAs;

        // 右侧控制面板
        private System.Windows.Forms.Panel panelRight;

        // 窗口选择组
        private System.Windows.Forms.GroupBox groupBoxWindow;
        private System.Windows.Forms.Button btnSelectWindow;
        private System.Windows.Forms.Label lblHandle;
        private System.Windows.Forms.Label lblWindowTitle;
        private System.Windows.Forms.Label lblWindowClass;

        // 发送控制组
        private System.Windows.Forms.GroupBox groupBoxSend;
        private System.Windows.Forms.Button btnSingleSend;
        private System.Windows.Forms.Button btnCycleControl;

        // 循环参数组
        private System.Windows.Forms.GroupBox groupBoxParams;
        private System.Windows.Forms.Label labelLineInterval;
        private System.Windows.Forms.NumericUpDown numericUpDownLineInterval;
        private System.Windows.Forms.Label labelCycleInterval;
        private System.Windows.Forms.NumericUpDown numericUpDownCycleInterval;
        private System.Windows.Forms.Label labelCycleCount;
        private System.Windows.Forms.NumericUpDown numericUpDownCycleCount;
        private System.Windows.Forms.Label labelCycleCountHint;
        private System.Windows.Forms.CheckBox checkBoxSendEnter;
        private System.Windows.Forms.Label labelSendMethod;
        private System.Windows.Forms.ComboBox comboBoxSendMethod;

        // 文本编辑器
        private System.Windows.Forms.RichTextBox richTextBoxContent;

        // 状态栏
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblPosition;

        // 工具提示
        private System.Windows.Forms.ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // 创建控件
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();

            this.panelRight = new System.Windows.Forms.Panel();

            this.groupBoxWindow = new System.Windows.Forms.GroupBox();
            this.btnSelectWindow = new System.Windows.Forms.Button();
            this.lblHandle = new System.Windows.Forms.Label();
            this.lblWindowTitle = new System.Windows.Forms.Label();
            this.lblWindowClass = new System.Windows.Forms.Label();

            this.groupBoxSend = new System.Windows.Forms.GroupBox();
            this.btnSingleSend = new System.Windows.Forms.Button();
            this.btnCycleControl = new System.Windows.Forms.Button();

            this.groupBoxParams = new System.Windows.Forms.GroupBox();
            this.labelLineInterval = new System.Windows.Forms.Label();
            this.numericUpDownLineInterval = new System.Windows.Forms.NumericUpDown();
            this.labelCycleInterval = new System.Windows.Forms.Label();
            this.numericUpDownCycleInterval = new System.Windows.Forms.NumericUpDown();
            this.labelCycleCount = new System.Windows.Forms.Label();
            this.numericUpDownCycleCount = new System.Windows.Forms.NumericUpDown();
            this.labelCycleCountHint = new System.Windows.Forms.Label();
            this.checkBoxSendEnter = new System.Windows.Forms.CheckBox();
            this.labelSendMethod = new System.Windows.Forms.Label();
            this.comboBoxSendMethod = new System.Windows.Forms.ComboBox();

            this.richTextBoxContent = new System.Windows.Forms.RichTextBox();

            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPosition = new System.Windows.Forms.ToolStripStatusLabel();

            this.toolTip = new System.Windows.Forms.ToolTip(this.components);

            this.panelTop.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBoxWindow.SuspendLayout();
            this.groupBoxSend.SuspendLayout();
            this.groupBoxParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleCount)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // ===== panelTop（顶部工具栏） =====
            this.panelTop.Controls.Add(this.btnNew);
            this.panelTop.Controls.Add(this.btnOpen);
            this.panelTop.Controls.Add(this.btnSave);
            this.panelTop.Controls.Add(this.btnSaveAs);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 42);
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(245, 245, 247);
            this.panelTop.Padding = new System.Windows.Forms.Padding(8, 6, 0, 6);

            // btnNew
            this.btnNew.Location = new System.Drawing.Point(8, 6);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(92, 30);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "📄  新建";
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // btnOpen
            this.btnOpen.Location = new System.Drawing.Point(106, 6);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(92, 30);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "📂  打开";
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.FlatAppearance.BorderSize = 0;
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.btnOpen.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnOpen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(204, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 30);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "💾  保存";
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnSaveAs
            this.btnSaveAs.Location = new System.Drawing.Point(302, 6);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(100, 30);
            this.btnSaveAs.TabIndex = 3;
            this.btnSaveAs.Text = "📑  另存为";
            this.btnSaveAs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAs.FlatAppearance.BorderSize = 0;
            this.btnSaveAs.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            this.btnSaveAs.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnSaveAs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);

            // ===== panelRight（右侧控制面板） =====
            this.panelRight.AutoScroll = true;
            this.panelRight.Controls.Add(this.groupBoxWindow);
            this.panelRight.Controls.Add(this.groupBoxSend);
            this.panelRight.Controls.Add(this.groupBoxParams);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(620, 35);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(280, 593);

            // ===== groupBoxWindow（窗口选择） =====
            this.groupBoxWindow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxWindow.Controls.Add(this.btnSelectWindow);
            this.groupBoxWindow.Controls.Add(this.lblHandle);
            this.groupBoxWindow.Controls.Add(this.lblWindowTitle);
            this.groupBoxWindow.Controls.Add(this.lblWindowClass);
            this.groupBoxWindow.Location = new System.Drawing.Point(8, 8);
            this.groupBoxWindow.Name = "groupBoxWindow";
            this.groupBoxWindow.Size = new System.Drawing.Size(264, 130);
            this.groupBoxWindow.TabStop = false;
            this.groupBoxWindow.Text = "窗口选择";

            // btnSelectWindow
            this.btnSelectWindow.Location = new System.Drawing.Point(10, 22);
            this.btnSelectWindow.Name = "btnSelectWindow";
            this.btnSelectWindow.Size = new System.Drawing.Size(244, 32);
            this.btnSelectWindow.TabIndex = 0;
            this.btnSelectWindow.Text = "🎯  按住拖动到目标窗口";
            this.btnSelectWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectWindow.FlatAppearance.BorderSize = 0;
            this.btnSelectWindow.BackColor = System.Drawing.Color.FromArgb(0, 133, 119);
            this.btnSelectWindow.ForeColor = System.Drawing.Color.White;
            this.btnSelectWindow.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSelectWindow.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.btnSelectWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSelectWindow_MouseDown);
            this.btnSelectWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnSelectWindow_MouseMove);
            this.btnSelectWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnSelectWindow_MouseUp);
            this.toolTip.SetToolTip(this.btnSelectWindow, "按住鼠标左键并拖动到目标窗口，释放鼠标即可获取窗口句柄");

            // lblHandle
            this.lblHandle.AutoSize = false;
            this.lblHandle.AutoEllipsis = true;
            this.lblHandle.Location = new System.Drawing.Point(10, 58);
            this.lblHandle.Name = "lblHandle";
            this.lblHandle.Size = new System.Drawing.Size(244, 20);
            this.lblHandle.Text = "句柄: 0x00000000";

            // lblWindowTitle
            this.lblWindowTitle.AutoSize = false;
            this.lblWindowTitle.AutoEllipsis = true;
            this.lblWindowTitle.Location = new System.Drawing.Point(10, 78);
            this.lblWindowTitle.Name = "lblWindowTitle";
            this.lblWindowTitle.Size = new System.Drawing.Size(244, 20);
            this.lblWindowTitle.Text = "标题: (未选择)";

            // lblWindowClass
            this.lblWindowClass.AutoSize = false;
            this.lblWindowClass.AutoEllipsis = true;
            this.lblWindowClass.Location = new System.Drawing.Point(10, 98);
            this.lblWindowClass.Name = "lblWindowClass";
            this.lblWindowClass.Size = new System.Drawing.Size(244, 20);
            this.lblWindowClass.Text = "类名: (未选择)";

            // ===== groupBoxSend（发送控制） =====
            this.groupBoxSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSend.Controls.Add(this.btnSingleSend);
            this.groupBoxSend.Controls.Add(this.btnCycleControl);
            this.groupBoxSend.Location = new System.Drawing.Point(8, 144);
            this.groupBoxSend.Name = "groupBoxSend";
            this.groupBoxSend.Size = new System.Drawing.Size(264, 102);
            this.groupBoxSend.TabStop = false;
            this.groupBoxSend.Text = "发送控制";

            // btnSingleSend
            this.btnSingleSend.Location = new System.Drawing.Point(10, 22);
            this.btnSingleSend.Name = "btnSingleSend";
            this.btnSingleSend.Size = new System.Drawing.Size(244, 32);
            this.btnSingleSend.TabIndex = 0;
            this.btnSingleSend.Text = "▶  单次发送 (光标所在行)";
            this.btnSingleSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSingleSend.FlatAppearance.BorderSize = 0;
            this.btnSingleSend.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSingleSend.ForeColor = System.Drawing.Color.White;
            this.btnSingleSend.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSingleSend.Click += new System.EventHandler(this.btnSingleSend_Click);
            this.toolTip.SetToolTip(this.btnSingleSend, "将光标所在行的文本发送到目标窗口");

            // btnCycleControl
            this.btnCycleControl.Location = new System.Drawing.Point(10, 58);
            this.btnCycleControl.Name = "btnCycleControl";
            this.btnCycleControl.Size = new System.Drawing.Size(244, 32);
            this.btnCycleControl.TabIndex = 1;
            this.btnCycleControl.Text = "🔁  开始循环";
            this.btnCycleControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCycleControl.FlatAppearance.BorderSize = 0;
            this.btnCycleControl.BackColor = System.Drawing.Color.FromArgb(16, 124, 16);
            this.btnCycleControl.ForeColor = System.Drawing.Color.White;
            this.btnCycleControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCycleControl.Click += new System.EventHandler(this.btnCycleControl_Click);
            this.toolTip.SetToolTip(this.btnCycleControl, "循环发送选中的行（未选中则发送全部非空行）");

            // ===== groupBoxParams（循环参数） =====
            this.groupBoxParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxParams.Controls.Add(this.labelLineInterval);
            this.groupBoxParams.Controls.Add(this.numericUpDownLineInterval);
            this.groupBoxParams.Controls.Add(this.labelCycleInterval);
            this.groupBoxParams.Controls.Add(this.numericUpDownCycleInterval);
            this.groupBoxParams.Controls.Add(this.labelCycleCount);
            this.groupBoxParams.Controls.Add(this.numericUpDownCycleCount);
            this.groupBoxParams.Controls.Add(this.labelCycleCountHint);
            this.groupBoxParams.Controls.Add(this.checkBoxSendEnter);
            this.groupBoxParams.Controls.Add(this.labelSendMethod);
            this.groupBoxParams.Controls.Add(this.comboBoxSendMethod);
            this.groupBoxParams.Location = new System.Drawing.Point(8, 252);
            this.groupBoxParams.Name = "groupBoxParams";
            this.groupBoxParams.Size = new System.Drawing.Size(264, 180);
            this.groupBoxParams.TabStop = false;
            this.groupBoxParams.Text = "循环参数";

            // labelLineInterval
            this.labelLineInterval.Location = new System.Drawing.Point(10, 25);
            this.labelLineInterval.Name = "labelLineInterval";
            this.labelLineInterval.Size = new System.Drawing.Size(85, 20);
            this.labelLineInterval.Text = "行间隔(ms):";

            // numericUpDownLineInterval
            this.numericUpDownLineInterval.Location = new System.Drawing.Point(100, 22);
            this.numericUpDownLineInterval.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            this.numericUpDownLineInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numericUpDownLineInterval.Name = "numericUpDownLineInterval";
            this.numericUpDownLineInterval.Size = new System.Drawing.Size(90, 21);
            this.numericUpDownLineInterval.Value = new decimal(new int[] { 100, 0, 0, 0 });

            // labelCycleInterval
            this.labelCycleInterval.Location = new System.Drawing.Point(10, 50);
            this.labelCycleInterval.Name = "labelCycleInterval";
            this.labelCycleInterval.Size = new System.Drawing.Size(85, 20);
            this.labelCycleInterval.Text = "循环间隔(ms):";

            // numericUpDownCycleInterval
            this.numericUpDownCycleInterval.Location = new System.Drawing.Point(100, 47);
            this.numericUpDownCycleInterval.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            this.numericUpDownCycleInterval.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numericUpDownCycleInterval.Name = "numericUpDownCycleInterval";
            this.numericUpDownCycleInterval.Size = new System.Drawing.Size(90, 21);
            this.numericUpDownCycleInterval.Value = new decimal(new int[] { 2000, 0, 0, 0 });

            // labelCycleCount
            this.labelCycleCount.Location = new System.Drawing.Point(10, 75);
            this.labelCycleCount.Name = "labelCycleCount";
            this.labelCycleCount.Size = new System.Drawing.Size(85, 20);
            this.labelCycleCount.Text = "循环次数:";

            // numericUpDownCycleCount
            this.numericUpDownCycleCount.Location = new System.Drawing.Point(100, 72);
            this.numericUpDownCycleCount.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numericUpDownCycleCount.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericUpDownCycleCount.Name = "numericUpDownCycleCount";
            this.numericUpDownCycleCount.Size = new System.Drawing.Size(90, 21);
            this.numericUpDownCycleCount.Value = new decimal(new int[] { 0, 0, 0, 0 });

            // labelCycleCountHint
            this.labelCycleCountHint.Location = new System.Drawing.Point(195, 75);
            this.labelCycleCountHint.Name = "labelCycleCountHint";
            this.labelCycleCountHint.Size = new System.Drawing.Size(60, 20);
            this.labelCycleCountHint.Text = "(0=无限)";
            this.labelCycleCountHint.ForeColor = System.Drawing.Color.Gray;

            // checkBoxSendEnter
            this.checkBoxSendEnter.AutoSize = true;
            this.checkBoxSendEnter.Checked = true;
            this.checkBoxSendEnter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSendEnter.Location = new System.Drawing.Point(10, 103);
            this.checkBoxSendEnter.Name = "checkBoxSendEnter";
            this.checkBoxSendEnter.Size = new System.Drawing.Size(150, 16);
            this.checkBoxSendEnter.Text = "发送后按回车键";
            this.checkBoxSendEnter.UseVisualStyleBackColor = true;

            // labelSendMethod
            this.labelSendMethod.Location = new System.Drawing.Point(10, 125);
            this.labelSendMethod.Name = "labelSendMethod";
            this.labelSendMethod.Size = new System.Drawing.Size(85, 20);
            this.labelSendMethod.Text = "发送方式:";

            // comboBoxSendMethod
            this.comboBoxSendMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSendMethod.Location = new System.Drawing.Point(100, 122);
            this.comboBoxSendMethod.Name = "comboBoxSendMethod";
            this.comboBoxSendMethod.Size = new System.Drawing.Size(155, 21);
            this.comboBoxSendMethod.Items.AddRange(new object[] {
                "后台发送 (PostMessage)",
                "前台发送 (SendKeys)"
            });

            // ===== richTextBoxContent（文本编辑器） =====
            this.richTextBoxContent.AcceptsTab = true;
            this.richTextBoxContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxContent.Font = new System.Drawing.Font("Consolas", 10F);
            this.richTextBoxContent.Location = new System.Drawing.Point(0, 35);
            this.richTextBoxContent.Name = "richTextBoxContent";
            this.richTextBoxContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxContent.Size = new System.Drawing.Size(620, 593);
            this.richTextBoxContent.TabIndex = 0;
            this.richTextBoxContent.WordWrap = false;
            this.richTextBoxContent.TextChanged += new System.EventHandler(this.richTextBoxContent_TextChanged);
            this.richTextBoxContent.SelectionChanged += new System.EventHandler(this.richTextBoxContent_SelectionChanged);

            // ===== statusStrip（状态栏） =====
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatus,
                this.lblPosition});
            this.statusStrip.Location = new System.Drawing.Point(0, 628);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 22);

            // lblStatus
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(32, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "就绪";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // lblPosition
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(100, 17);
            this.lblPosition.Text = "行: 1, 列: 1";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ===== MainForm =====
            // 控件添加顺序决定 Dock 停靠优先级：
            // 最后添加的控件 z-order 最高，最先被 Dock 处理。
            // 顺序：Fill(最先添加，最后停靠) → Right → Top → Bottom(最后添加，最先停靠)
            this.ClientSize = new System.Drawing.Size(900, 650);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Controls.Add(this.richTextBoxContent);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.Text = "窗口命令发送器 - 新建文件";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // ResumeLayout
            this.panelTop.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.groupBoxWindow.ResumeLayout(false);
            this.groupBoxSend.ResumeLayout(false);
            this.groupBoxParams.ResumeLayout(false);
            this.groupBoxParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleCount)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
