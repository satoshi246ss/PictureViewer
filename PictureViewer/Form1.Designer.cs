namespace PictureViewer
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ShowButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ObsStart = new System.Windows.Forms.Button();
            this.ObsEndButton = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.ButtonSaveEnd = new System.Windows.Forms.Button();
            this.buttonMakeDark = new System.Windows.Forms.Button();
            this.checkBoxObsAuto = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.timerSavePostTime = new System.Windows.Forms.Timer(this.components);
            this.timerSaveMainTime = new System.Windows.Forms.Timer(this.components);
            this.timerMakeDark = new System.Windows.Forms.Timer(this.components);
            this.timerObsOnOff = new System.Windows.Forms.Timer(this.components);
            this.timerMTmonSend = new System.Windows.Forms.Timer(this.components);
            this.timer1min = new System.Windows.Forms.Timer(this.components);
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(this.checkBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            tableLayoutPanel1.Controls.Add(this.textBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(646, 620);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 2);
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.checkBox1.Location = new System.Drawing.Point(3, 489);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(75, 16);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "DarkMode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ShowButton);
            this.flowLayoutPanel1.Controls.Add(this.CloseButton);
            this.flowLayoutPanel1.Controls.Add(this.ObsStart);
            this.flowLayoutPanel1.Controls.Add(this.ObsEndButton);
            this.flowLayoutPanel1.Controls.Add(this.buttonSave);
            this.flowLayoutPanel1.Controls.Add(this.ButtonSaveEnd);
            this.flowLayoutPanel1.Controls.Add(this.buttonMakeDark);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxObsAuto);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(99, 489);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(544, 48);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // ShowButton
            // 
            this.ShowButton.AutoSize = true;
            this.ShowButton.Location = new System.Drawing.Point(3, 3);
            this.ShowButton.Name = "ShowButton";
            this.ShowButton.Size = new System.Drawing.Size(58, 23);
            this.ShowButton.TabIndex = 0;
            this.ShowButton.Text = "Show";
            this.ShowButton.UseVisualStyleBackColor = true;
            this.ShowButton.Click += new System.EventHandler(this.ShowButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.AutoSize = true;
            this.CloseButton.Location = new System.Drawing.Point(67, 3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(60, 23);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ObsStart
            // 
            this.ObsStart.Location = new System.Drawing.Point(133, 3);
            this.ObsStart.Name = "ObsStart";
            this.ObsStart.Size = new System.Drawing.Size(75, 23);
            this.ObsStart.TabIndex = 4;
            this.ObsStart.Text = "Obs Start";
            this.ObsStart.UseVisualStyleBackColor = true;
            this.ObsStart.Click += new System.EventHandler(this.ObsStart_Click);
            // 
            // ObsEndButton
            // 
            this.ObsEndButton.AutoSize = true;
            this.ObsEndButton.Location = new System.Drawing.Point(214, 3);
            this.ObsEndButton.Name = "ObsEndButton";
            this.ObsEndButton.Size = new System.Drawing.Size(75, 23);
            this.ObsEndButton.TabIndex = 1;
            this.ObsEndButton.Text = "Obs End";
            this.ObsEndButton.UseVisualStyleBackColor = true;
            this.ObsEndButton.Click += new System.EventHandler(this.ObsEndButton_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(295, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save Start";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // ButtonSaveEnd
            // 
            this.ButtonSaveEnd.AutoSize = true;
            this.ButtonSaveEnd.Location = new System.Drawing.Point(376, 3);
            this.ButtonSaveEnd.Name = "ButtonSaveEnd";
            this.ButtonSaveEnd.Size = new System.Drawing.Size(75, 23);
            this.ButtonSaveEnd.TabIndex = 2;
            this.ButtonSaveEnd.Text = "Save End";
            this.ButtonSaveEnd.UseVisualStyleBackColor = true;
            this.ButtonSaveEnd.Click += new System.EventHandler(this.ButtonSaveEnd_Click);
            // 
            // buttonMakeDark
            // 
            this.buttonMakeDark.Location = new System.Drawing.Point(457, 3);
            this.buttonMakeDark.Name = "buttonMakeDark";
            this.buttonMakeDark.Size = new System.Drawing.Size(70, 23);
            this.buttonMakeDark.TabIndex = 6;
            this.buttonMakeDark.Text = "MakeDark";
            this.buttonMakeDark.UseVisualStyleBackColor = true;
            this.buttonMakeDark.Click += new System.EventHandler(this.buttonMakeDark_Click);
            // 
            // checkBoxObsAuto
            // 
            this.checkBoxObsAuto.AutoSize = true;
            this.checkBoxObsAuto.Checked = true;
            this.checkBoxObsAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxObsAuto.Location = new System.Drawing.Point(3, 32);
            this.checkBoxObsAuto.Name = "checkBoxObsAuto";
            this.checkBoxObsAuto.Size = new System.Drawing.Size(68, 16);
            this.checkBoxObsAuto.TabIndex = 7;
            this.checkBoxObsAuto.Text = "ObsAuto";
            this.checkBoxObsAuto.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            tableLayoutPanel1.SetColumnSpan(this.textBox1, 2);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 602);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(640, 19);
            this.textBox1.TabIndex = 3;
            // 
            // richTextBox1
            // 
            tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 2);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 543);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(640, 53);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|All file" +
                "s (*.*)|*.* ";
            this.openFileDialog1.Title = "Select a picture file ";
            // 
            // timerSavePostTime
            // 
            this.timerSavePostTime.Interval = 3000;
            this.timerSavePostTime.Tick += new System.EventHandler(this.timerSavePostTime_Tick);
            // 
            // timerSaveMainTime
            // 
            this.timerSaveMainTime.Interval = 10000;
            this.timerSaveMainTime.Tick += new System.EventHandler(this.timerSaveMainTime_Tick);
            // 
            // timerMakeDark
            // 
            this.timerMakeDark.Interval = 4000;
            this.timerMakeDark.Tick += new System.EventHandler(this.timerMakeDark_Tick);
            // 
            // timerObsOnOff
            // 
            this.timerObsOnOff.Enabled = true;
            this.timerObsOnOff.Interval = 10000;
            this.timerObsOnOff.Tick += new System.EventHandler(this.timerObsOnOff_Tick);
            // 
            // timerMTmonSend
            // 
            this.timerMTmonSend.Enabled = true;
            this.timerMTmonSend.Interval = 2000;
            this.timerMTmonSend.Tick += new System.EventHandler(this.timerMTmonSend_Tick);
            // 
            // timer1min
            // 
            this.timer1min.Enabled = true;
            this.timer1min.Interval = 60000;
            this.timer1min.Tick += new System.EventHandler(this.timer1min_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 620);
            this.Controls.Add(tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "PictureViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button ShowButton;
        private System.Windows.Forms.Button ObsEndButton;
        private System.Windows.Forms.Button ButtonSaveEnd;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button ObsStart;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Timer timerSavePostTime;
        private System.Windows.Forms.Timer timerSaveMainTime;
        private System.Windows.Forms.Button buttonMakeDark;
        private System.Windows.Forms.Timer timerMakeDark;
        private System.Windows.Forms.Timer timerObsOnOff;
        private System.Windows.Forms.CheckBox checkBoxObsAuto;
        private System.Windows.Forms.Timer timerMTmonSend;
        private System.Windows.Forms.Timer timer1min;
    }
}

