namespace tfs2svn.Winforms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tbTFSUrl = new System.Windows.Forms.TextBox();
            this.tbSVNUrl = new System.Windows.Forms.TextBox();
            this.tbChangesetStart = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbDoInitialCheckout = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbWorkingCopyFolder = new System.Windows.Forms.TextBox();
            this.tbTFSUsername = new System.Windows.Forms.TextBox();
            this.tbTFSPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTFSDomain = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbCreateRepository = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(529, 239);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Convert!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 279);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(592, 264);
            this.listBox1.TabIndex = 4;
            // 
            // tbTFSUrl
            // 
            this.tbTFSUrl.Location = new System.Drawing.Point(130, 23);
            this.tbTFSUrl.Name = "tbTFSUrl";
            this.tbTFSUrl.Size = new System.Drawing.Size(288, 20);
            this.tbTFSUrl.TabIndex = 5;
            // 
            // tbSVNUrl
            // 
            this.tbSVNUrl.Location = new System.Drawing.Point(130, 19);
            this.tbSVNUrl.Name = "tbSVNUrl";
            this.tbSVNUrl.Size = new System.Drawing.Size(288, 20);
            this.tbSVNUrl.TabIndex = 6;
            this.tbSVNUrl.TextChanged += new System.EventHandler(this.tbSVNUrl_TextChanged);
            // 
            // tbChangesetStart
            // 
            this.tbChangesetStart.Location = new System.Drawing.Point(130, 75);
            this.tbChangesetStart.Name = "tbChangesetStart";
            this.tbChangesetStart.Size = new System.Drawing.Size(102, 20);
            this.tbChangesetStart.TabIndex = 7;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 549);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(592, 23);
            this.progressBar1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "TFS repository";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "SVN repository (dest)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Start on Changeset#";
            // 
            // cbDoInitialCheckout
            // 
            this.cbDoInitialCheckout.AutoSize = true;
            this.cbDoInitialCheckout.Checked = true;
            this.cbDoInitialCheckout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDoInitialCheckout.Location = new System.Drawing.Point(427, 64);
            this.cbDoInitialCheckout.Name = "cbDoInitialCheckout";
            this.cbDoInitialCheckout.Size = new System.Drawing.Size(114, 17);
            this.cbDoInitialCheckout.TabIndex = 12;
            this.cbDoInitialCheckout.Text = "Do initial checkout";
            this.cbDoInitialCheckout.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Working copy folder";
            // 
            // tbWorkingCopyFolder
            // 
            this.tbWorkingCopyFolder.Location = new System.Drawing.Point(130, 61);
            this.tbWorkingCopyFolder.Name = "tbWorkingCopyFolder";
            this.tbWorkingCopyFolder.Size = new System.Drawing.Size(288, 20);
            this.tbWorkingCopyFolder.TabIndex = 14;
            // 
            // tbTFSUsername
            // 
            this.tbTFSUsername.Location = new System.Drawing.Point(130, 49);
            this.tbTFSUsername.Name = "tbTFSUsername";
            this.tbTFSUsername.Size = new System.Drawing.Size(123, 20);
            this.tbTFSUsername.TabIndex = 15;
            // 
            // tbTFSPassword
            // 
            this.tbTFSPassword.Location = new System.Drawing.Point(318, 49);
            this.tbTFSPassword.Name = "tbTFSPassword";
            this.tbTFSPassword.Size = new System.Drawing.Size(100, 20);
            this.tbTFSPassword.TabIndex = 16;
            this.tbTFSPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "TFS Username";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(259, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Password";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(212, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "[Assuming saved SVN Authentication Data]";
            // 
            // tbTFSDomain
            // 
            this.tbTFSDomain.Location = new System.Drawing.Point(473, 49);
            this.tbTFSDomain.Name = "tbTFSDomain";
            this.tbTFSDomain.Size = new System.Drawing.Size(100, 20);
            this.tbTFSDomain.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(424, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Domain";
            // 
            // cbCreateRepository
            // 
            this.cbCreateRepository.AutoSize = true;
            this.cbCreateRepository.Checked = true;
            this.cbCreateRepository.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCreateRepository.Enabled = false;
            this.cbCreateRepository.Location = new System.Drawing.Point(427, 22);
            this.cbCreateRepository.Name = "cbCreateRepository";
            this.cbCreateRepository.Size = new System.Drawing.Size(146, 17);
            this.cbCreateRepository.TabIndex = 22;
            this.cbCreateRepository.Text = "Create local file repository";
            this.cbCreateRepository.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbTFSUrl);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbChangesetStart);
            this.groupBox1.Controls.Add(this.tbTFSDomain);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbTFSUsername);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbTFSPassword);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(592, 106);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TFS source settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbSVNUrl);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cbCreateRepository);
            this.groupBox2.Controls.Add(this.cbDoInitialCheckout);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbWorkingCopyFolder);
            this.groupBox2.Location = new System.Drawing.Point(12, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(592, 100);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SVN destination settings";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 582);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "tfs2svn 0.1 [WinForms client]";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox tbTFSUrl;
        private System.Windows.Forms.TextBox tbSVNUrl;
        private System.Windows.Forms.TextBox tbChangesetStart;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbDoInitialCheckout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbWorkingCopyFolder;
        private System.Windows.Forms.TextBox tbTFSUsername;
        private System.Windows.Forms.TextBox tbTFSPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTFSDomain;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbCreateRepository;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

