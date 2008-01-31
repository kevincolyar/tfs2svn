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
            System.Windows.Forms.ListViewItem listViewItem37 = new System.Windows.Forms.ListViewItem("One");
            System.Windows.Forms.ListViewItem listViewItem38 = new System.Windows.Forms.ListViewItem("Two");
            System.Windows.Forms.ListViewItem listViewItem39 = new System.Windows.Forms.ListViewItem("Three");
            this.button1 = new System.Windows.Forms.Button();
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabStatus = new System.Windows.Forms.TabPage();
            this.lstStatus = new System.Windows.Forms.ListBox();
            this.tabDetails = new System.Windows.Forms.TabPage();
            this.lstMovement = new System.Windows.Forms.ListView();
            this.colChangeset = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.colAction = new System.Windows.Forms.ColumnHeader();
            this.colNewPath = new System.Windows.Forms.ColumnHeader();
            this.colOldPath = new System.Windows.Forms.ColumnHeader();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblTimeRemaining = new System.Windows.Forms.ToolStripStatusLabel();
            this.comboTfsClientProvider = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabStatus.SuspendLayout();
            this.tabDetails.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(666, 269);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Convert!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbTFSUrl
            // 
            this.tbTFSUrl.Location = new System.Drawing.Point(130, 23);
            this.tbTFSUrl.Name = "tbTFSUrl";
            this.tbTFSUrl.Size = new System.Drawing.Size(443, 20);
            this.tbTFSUrl.TabIndex = 5;
            // 
            // tbSVNUrl
            // 
            this.tbSVNUrl.Location = new System.Drawing.Point(130, 19);
            this.tbSVNUrl.Name = "tbSVNUrl";
            this.tbSVNUrl.Size = new System.Drawing.Size(443, 20);
            this.tbSVNUrl.TabIndex = 6;
            this.tbSVNUrl.TextChanged += new System.EventHandler(this.tbSVNUrl_TextChanged);
            // 
            // tbChangesetStart
            // 
            this.tbChangesetStart.Location = new System.Drawing.Point(130, 107);
            this.tbChangesetStart.Name = "tbChangesetStart";
            this.tbChangesetStart.Size = new System.Drawing.Size(123, 20);
            this.tbChangesetStart.TabIndex = 7;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 549);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(729, 23);
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
            this.label3.Location = new System.Drawing.Point(22, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Start at Changeset#";
            // 
            // cbDoInitialCheckout
            // 
            this.cbDoInitialCheckout.AutoSize = true;
            this.cbDoInitialCheckout.Checked = true;
            this.cbDoInitialCheckout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDoInitialCheckout.Location = new System.Drawing.Point(579, 63);
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
            this.tbWorkingCopyFolder.Size = new System.Drawing.Size(443, 20);
            this.tbWorkingCopyFolder.TabIndex = 14;
            // 
            // tbTFSUsername
            // 
            this.tbTFSUsername.Location = new System.Drawing.Point(130, 51);
            this.tbTFSUsername.Name = "tbTFSUsername";
            this.tbTFSUsername.Size = new System.Drawing.Size(123, 20);
            this.tbTFSUsername.TabIndex = 15;
            // 
            // tbTFSPassword
            // 
            this.tbTFSPassword.Location = new System.Drawing.Point(318, 51);
            this.tbTFSPassword.Name = "tbTFSPassword";
            this.tbTFSPassword.Size = new System.Drawing.Size(100, 20);
            this.tbTFSPassword.TabIndex = 16;
            this.tbTFSPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "TFS Username";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(259, 54);
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
            this.label7.Size = new System.Drawing.Size(310, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Note: Assuming locally saved SVN Authentication for this server.";
            // 
            // tbTFSDomain
            // 
            this.tbTFSDomain.Location = new System.Drawing.Point(473, 51);
            this.tbTFSDomain.Name = "tbTFSDomain";
            this.tbTFSDomain.Size = new System.Drawing.Size(100, 20);
            this.tbTFSDomain.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(424, 54);
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
            this.cbCreateRepository.Location = new System.Drawing.Point(579, 21);
            this.cbCreateRepository.Name = "cbCreateRepository";
            this.cbCreateRepository.Size = new System.Drawing.Size(146, 17);
            this.cbCreateRepository.TabIndex = 22;
            this.cbCreateRepository.Text = "Create local file repository";
            this.cbCreateRepository.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboTfsClientProvider);
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
            this.groupBox1.Size = new System.Drawing.Size(729, 136);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TFS source settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tbSVNUrl);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cbCreateRepository);
            this.groupBox2.Controls.Add(this.cbDoInitialCheckout);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbWorkingCopyFolder);
            this.groupBox2.Location = new System.Drawing.Point(12, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(729, 100);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SVN destination settings";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabStatus);
            this.tabControl1.Controls.Add(this.tabDetails);
            this.tabControl1.Location = new System.Drawing.Point(12, 290);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(729, 253);
            this.tabControl1.TabIndex = 26;
            // 
            // tabStatus
            // 
            this.tabStatus.Controls.Add(this.lstStatus);
            this.tabStatus.Location = new System.Drawing.Point(4, 22);
            this.tabStatus.Name = "tabStatus";
            this.tabStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatus.Size = new System.Drawing.Size(721, 227);
            this.tabStatus.TabIndex = 0;
            this.tabStatus.Text = "Status";
            this.tabStatus.UseVisualStyleBackColor = true;
            // 
            // lstStatus
            // 
            this.lstStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStatus.FormattingEnabled = true;
            this.lstStatus.Location = new System.Drawing.Point(3, 3);
            this.lstStatus.Name = "lstStatus";
            this.lstStatus.ScrollAlwaysVisible = true;
            this.lstStatus.Size = new System.Drawing.Size(715, 212);
            this.lstStatus.TabIndex = 4;
            // 
            // tabDetails
            // 
            this.tabDetails.Controls.Add(this.lstMovement);
            this.tabDetails.Location = new System.Drawing.Point(4, 22);
            this.tabDetails.Name = "tabDetails";
            this.tabDetails.Size = new System.Drawing.Size(721, 215);
            this.tabDetails.TabIndex = 1;
            this.tabDetails.Text = "Details";
            this.tabDetails.UseVisualStyleBackColor = true;
            // 
            // lstMovement
            // 
            this.lstMovement.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.lstMovement.AutoArrange = false;
            this.lstMovement.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colChangeset,
            this.colType,
            this.colAction,
            this.colNewPath,
            this.colOldPath});
            this.lstMovement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMovement.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem37,
            listViewItem38,
            listViewItem39});
            this.lstMovement.Location = new System.Drawing.Point(0, 0);
            this.lstMovement.Name = "lstMovement";
            this.lstMovement.Size = new System.Drawing.Size(721, 215);
            this.lstMovement.TabIndex = 0;
            this.lstMovement.UseCompatibleStateImageBehavior = false;
            this.lstMovement.View = System.Windows.Forms.View.Details;
            // 
            // colChangeset
            // 
            this.colChangeset.Text = "Changeset";
            this.colChangeset.Width = 66;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            // 
            // colAction
            // 
            this.colAction.Text = "Action";
            // 
            // colNewPath
            // 
            this.colNewPath.Text = "New Path";
            this.colNewPath.Width = 228;
            // 
            // colOldPath
            // 
            this.colOldPath.Text = "Old Path";
            this.colOldPath.Width = 227;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTimeRemaining});
            this.statusStrip1.Location = new System.Drawing.Point(0, 575);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(752, 22);
            this.statusStrip1.TabIndex = 27;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(0, 17);
            // 
            // comboTfsClientProvider
            // 
            this.comboTfsClientProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTfsClientProvider.FormattingEnabled = true;
            this.comboTfsClientProvider.Location = new System.Drawing.Point(130, 79);
            this.comboTfsClientProvider.Name = "comboTfsClientProvider";
            this.comboTfsClientProvider.Size = new System.Drawing.Size(123, 21);
            this.comboTfsClientProvider.TabIndex = 22;
            this.comboTfsClientProvider.SelectedIndexChanged += new System.EventHandler(this.comboTfsClientProvider_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(38, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "TFS Client library";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(259, 110);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(401, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Note: Starting at Changeset# greater than 1 is suitable for incremental updates o" +
                "nly!";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(259, 83);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(432, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Note: OpenTF libs are included. To use Microsoft\'s libs you\'ll need Team Explorer" +
                " installed.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 597);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(623, 338);
            this.Name = "MainForm";
            this.Text = "tfs2svn 0.1 [WinForms client]";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabStatus.ResumeLayout(false);
            this.tabDetails.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabStatus;
        private System.Windows.Forms.TabPage tabDetails;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblTimeRemaining;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.ListView lstMovement;
        private System.Windows.Forms.ColumnHeader colAction;
        private System.Windows.Forms.ColumnHeader colNewPath;
        private System.Windows.Forms.ColumnHeader colOldPath;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colChangeset;
        private System.Windows.Forms.ComboBox comboTfsClientProvider;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
    }
}

