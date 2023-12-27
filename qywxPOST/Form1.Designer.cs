namespace qywxPOST
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.getTimeNow = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textDate = new System.Windows.Forms.TextBox();
            this.textData = new System.Windows.Forms.TextBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxForcePush = new System.Windows.Forms.CheckBox();
            this.buttonFileUpdata = new System.Windows.Forms.Button();
            this.checkBoxShowTodayTasks = new System.Windows.Forms.CheckBox();
            this.checkBoxMultipleRobots = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(137, 169);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(203, 72);
            this.button1.TabIndex = 0;
            this.button1.Text = "定时推送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // getTimeNow
            // 
            this.getTimeNow.Location = new System.Drawing.Point(361, 51);
            this.getTimeNow.Name = "getTimeNow";
            this.getTimeNow.Size = new System.Drawing.Size(105, 72);
            this.getTimeNow.TabIndex = 1;
            this.getTimeNow.Text = "获取当前时间";
            this.getTimeNow.UseVisualStyleBackColor = true;
            this.getTimeNow.Click += new System.EventHandler(this.getTimeNow_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "推送时间：";
            // 
            // textDate
            // 
            this.textDate.Location = new System.Drawing.Point(137, 51);
            this.textDate.Name = "textDate";
            this.textDate.Size = new System.Drawing.Size(203, 25);
            this.textDate.TabIndex = 3;
            // 
            // textData
            // 
            this.textData.Location = new System.Drawing.Point(137, 82);
            this.textData.Multiline = true;
            this.textData.Name = "textData";
            this.textData.Size = new System.Drawing.Size(203, 81);
            this.textData.TabIndex = 4;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Location = new System.Drawing.Point(12, 293);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(482, 85);
            this.textBoxOutput.TabIndex = 5;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(500, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(423, 366);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "推送内容：";
            // 
            // checkBoxForcePush
            // 
            this.checkBoxForcePush.AutoSize = true;
            this.checkBoxForcePush.Location = new System.Drawing.Point(177, 259);
            this.checkBoxForcePush.Name = "checkBoxForcePush";
            this.checkBoxForcePush.Size = new System.Drawing.Size(119, 19);
            this.checkBoxForcePush.TabIndex = 11;
            this.checkBoxForcePush.Text = "启用强制提醒";
            this.checkBoxForcePush.UseVisualStyleBackColor = true;
            this.checkBoxForcePush.CheckedChanged += new System.EventHandler(this.checkBoxForcePush_CheckedChanged);
            // 
            // buttonFileUpdata
            // 
            this.buttonFileUpdata.Location = new System.Drawing.Point(361, 129);
            this.buttonFileUpdata.Name = "buttonFileUpdata";
            this.buttonFileUpdata.Size = new System.Drawing.Size(105, 72);
            this.buttonFileUpdata.TabIndex = 12;
            this.buttonFileUpdata.Text = "从文件更新";
            this.buttonFileUpdata.UseVisualStyleBackColor = true;
            this.buttonFileUpdata.Click += new System.EventHandler(this.buttonFileUpdata_Click);
            // 
            // checkBoxShowTodayTasks
            // 
            this.checkBoxShowTodayTasks.AutoSize = true;
            this.checkBoxShowTodayTasks.Location = new System.Drawing.Point(332, 259);
            this.checkBoxShowTodayTasks.Name = "checkBoxShowTodayTasks";
            this.checkBoxShowTodayTasks.Size = new System.Drawing.Size(134, 19);
            this.checkBoxShowTodayTasks.TabIndex = 13;
            this.checkBoxShowTodayTasks.Text = "只显示当天任务";
            this.checkBoxShowTodayTasks.UseVisualStyleBackColor = true;
            this.checkBoxShowTodayTasks.CheckedChanged += new System.EventHandler(this.checkBoxShowTodayTasks_CheckedChanged);
            // 
            // checkBoxMultipleRobots
            // 
            this.checkBoxMultipleRobots.AutoSize = true;
            this.checkBoxMultipleRobots.Location = new System.Drawing.Point(12, 259);
            this.checkBoxMultipleRobots.Name = "checkBoxMultipleRobots";
            this.checkBoxMultipleRobots.Size = new System.Drawing.Size(149, 19);
            this.checkBoxMultipleRobots.TabIndex = 14;
            this.checkBoxMultipleRobots.Text = "推送到多个机器人";
            this.checkBoxMultipleRobots.UseVisualStyleBackColor = true;
            this.checkBoxMultipleRobots.CheckedChanged += new System.EventHandler(this.checkBoxMultipleRobots_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 386);
            this.Controls.Add(this.checkBoxMultipleRobots);
            this.Controls.Add(this.checkBoxShowTodayTasks);
            this.Controls.Add(this.buttonFileUpdata);
            this.Controls.Add(this.checkBoxForcePush);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.textData);
            this.Controls.Add(this.textDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.getTimeNow);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "qywxPOST";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button getTimeNow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textDate;
        private System.Windows.Forms.TextBox textData;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxForcePush;
        private System.Windows.Forms.Button buttonFileUpdata;
        private System.Windows.Forms.CheckBox checkBoxShowTodayTasks;
        private System.Windows.Forms.CheckBox checkBoxMultipleRobots;
    }
}

