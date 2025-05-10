using System.Windows.Forms;
using System;

namespace CmdSender
{
    partial class MainForm
    {
        // 控件声明（必须与设计器中的名称一致）
        private System.ComponentModel.IContainer components = null;

        // 按钮
        private System.Windows.Forms.Button btnSelectWindow;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSingleSend;
        private System.Windows.Forms.Button btnCycleControl;

        // 文本框与数值输入
        private System.Windows.Forms.RichTextBox richTextBoxContent;
        private System.Windows.Forms.NumericUpDown numericUpDownLineInterval;
        private System.Windows.Forms.NumericUpDown numericUpDownCycleInterval;
        private System.Windows.Forms.NumericUpDown numericUpDownCycleCount;

        // 标签与状态栏
        private System.Windows.Forms.Label lblHandle;
        private System.Windows.Forms.Label labelLineInterval;
        private System.Windows.Forms.Label labelCycleInterval;
        private System.Windows.Forms.Label labelCycleCount;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        // 布局容器
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutTop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutBottom;

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
            this.btnSelectWindow = new System.Windows.Forms.Button();
            this.lblHandle = new System.Windows.Forms.Label();
            this.richTextBoxContent = new System.Windows.Forms.RichTextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.numericUpDownLineInterval = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCycleInterval = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCycleCount = new System.Windows.Forms.NumericUpDown();
            this.btnSingleSend = new System.Windows.Forms.Button();
            this.btnCycleControl = new System.Windows.Forms.Button();
            this.labelLineInterval = new System.Windows.Forms.Label();
            this.labelCycleInterval = new System.Windows.Forms.Label();
            this.labelCycleCount = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutTop = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutBottom = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleCount)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectWindow
            // 
            this.btnSelectWindow.Location = new System.Drawing.Point(650, 482);
            this.btnSelectWindow.Name = "btnSelectWindow";
            this.btnSelectWindow.Size = new System.Drawing.Size(120, 20);
            this.btnSelectWindow.TabIndex = 0;
            this.btnSelectWindow.Text = "选择目标窗口";
            //this.btnSelectWindow.Click += new System.EventHandler(this.btnSelectWindow_Click);
            this.btnSelectWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSelectWindow_MouseDown);
            this.btnSelectWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnSelectWindow_MouseUp);
            // 
            // lblHandle
            // 
            this.lblHandle.AutoSize = true;
            this.lblHandle.Location = new System.Drawing.Point(648, 517);
            this.lblHandle.Name = "lblHandle";
            this.lblHandle.Size = new System.Drawing.Size(65, 12);
            this.lblHandle.TabIndex = 1;
            this.lblHandle.Text = "0x00000000";
            // 
            // richTextBoxContent
            // 
            this.richTextBoxContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxContent.Location = new System.Drawing.Point(3, 11);
            this.richTextBoxContent.Name = "richTextBoxContent";
            this.richTextBoxContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxContent.Size = new System.Drawing.Size(565, 467);
            this.richTextBoxContent.TabIndex = 1;
            this.richTextBoxContent.Text = "测试";
            this.richTextBoxContent.TextChanged += new System.EventHandler(this.richTextBoxContent_TextChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(12, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "打开";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(182, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(93, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 4;
            this.btnNew.Text = "新建";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // numericUpDownLineInterval
            // 
            this.numericUpDownLineInterval.Location = new System.Drawing.Point(650, 369);
            this.numericUpDownLineInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownLineInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownLineInterval.Name = "numericUpDownLineInterval";
            this.numericUpDownLineInterval.Size = new System.Drawing.Size(120, 21);
            this.numericUpDownLineInterval.TabIndex = 1;
            this.numericUpDownLineInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownCycleInterval
            // 
            this.numericUpDownCycleInterval.Location = new System.Drawing.Point(650, 429);
            this.numericUpDownCycleInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownCycleInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownCycleInterval.Name = "numericUpDownCycleInterval";
            this.numericUpDownCycleInterval.Size = new System.Drawing.Size(120, 21);
            this.numericUpDownCycleInterval.TabIndex = 3;
            this.numericUpDownCycleInterval.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // numericUpDownCycleCount
            // 
            this.numericUpDownCycleCount.Location = new System.Drawing.Point(650, 296);
            this.numericUpDownCycleCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownCycleCount.Name = "numericUpDownCycleCount";
            this.numericUpDownCycleCount.Size = new System.Drawing.Size(120, 21);
            this.numericUpDownCycleCount.TabIndex = 5;
            this.numericUpDownCycleCount.ValueChanged += new System.EventHandler(this.numericUpDownCycleCount_ValueChanged);
            // 
            // btnSingleSend
            // 
            this.btnSingleSend.Location = new System.Drawing.Point(650, 76);
            this.btnSingleSend.Name = "btnSingleSend";
            this.btnSingleSend.Size = new System.Drawing.Size(75, 23);
            this.btnSingleSend.TabIndex = 6;
            this.btnSingleSend.Text = "单次发送";
            this.btnSingleSend.Click += new System.EventHandler(this.btnSingleSend_Click);
            // 
            // btnCycleControl
            // 
            this.btnCycleControl.Location = new System.Drawing.Point(650, 130);
            this.btnCycleControl.Name = "btnCycleControl";
            this.btnCycleControl.Size = new System.Drawing.Size(75, 23);
            this.btnCycleControl.TabIndex = 7;
            this.btnCycleControl.Text = "开始循环";
            this.btnCycleControl.Click += new System.EventHandler(this.btnCycleControl_Click);
            // 
            // labelLineInterval
            // 
            this.labelLineInterval.Location = new System.Drawing.Point(648, 332);
            this.labelLineInterval.Name = "labelLineInterval";
            this.labelLineInterval.Size = new System.Drawing.Size(100, 23);
            this.labelLineInterval.TabIndex = 0;
            this.labelLineInterval.Text = "行间隔(ms):";
            // 
            // labelCycleInterval
            // 
            this.labelCycleInterval.Location = new System.Drawing.Point(648, 403);
            this.labelCycleInterval.Name = "labelCycleInterval";
            this.labelCycleInterval.Size = new System.Drawing.Size(100, 23);
            this.labelCycleInterval.TabIndex = 2;
            this.labelCycleInterval.Text = "循环间隔(ms):";
            // 
            // labelCycleCount
            // 
            this.labelCycleCount.Location = new System.Drawing.Point(648, 258);
            this.labelCycleCount.Name = "labelCycleCount";
            this.labelCycleCount.Size = new System.Drawing.Size(100, 23);
            this.labelCycleCount.TabIndex = 4;
            this.labelCycleCount.Text = "循环次数:";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 578);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(32, 17);
            this.lblStatus.Text = "就绪";
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 1;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.Controls.Add(this.richTextBoxContent, 0, 1);
            this.tableLayoutMain.Location = new System.Drawing.Point(12, 59);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 3;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 473F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutMain.Size = new System.Drawing.Size(571, 481);
            this.tableLayoutMain.TabIndex = 0;
            this.tableLayoutMain.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutMain_Paint);
            // 
            // flowLayoutTop
            // 
            this.flowLayoutTop.Location = new System.Drawing.Point(650, 215);
            this.flowLayoutTop.Name = "flowLayoutTop";
            this.flowLayoutTop.Size = new System.Drawing.Size(120, 26);
            this.flowLayoutTop.TabIndex = 0;
            this.flowLayoutTop.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutTop_Paint);
            // 
            // flowLayoutBottom
            // 
            this.flowLayoutBottom.Location = new System.Drawing.Point(650, 175);
            this.flowLayoutBottom.Name = "flowLayoutBottom";
            this.flowLayoutBottom.Size = new System.Drawing.Size(120, 34);
            this.flowLayoutBottom.TabIndex = 2;
            this.flowLayoutBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutBottom_Paint);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.flowLayoutTop);
            this.Controls.Add(this.flowLayoutBottom);
            this.Controls.Add(this.lblHandle);
            this.Controls.Add(this.btnSelectWindow);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSingleSend);
            this.Controls.Add(this.numericUpDownCycleCount);
            this.Controls.Add(this.btnCycleControl);
            this.Controls.Add(this.labelCycleCount);
            this.Controls.Add(this.numericUpDownCycleInterval);
            this.Controls.Add(this.labelCycleInterval);
            this.Controls.Add(this.numericUpDownLineInterval);
            this.Controls.Add(this.labelLineInterval);
            this.Controls.Add(this.tableLayoutMain);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.Text = "窗口命令发送器";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCycleCount)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}