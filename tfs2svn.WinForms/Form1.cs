using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Colyar.SourceControl;
using Colyar.SourceControl.Tfs2Svn;
using Colyar.Utils;
using log4net;
using tfs2svn.Winforms.Properties;
using log4net.Config;
using System.Collections.Specialized;
using Colyar.SourceControl.TeamFoundationServer;

namespace tfs2svn.Winforms
{
    public partial class MainForm : Form
    {
        // Static log4net logger instance
        private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));
        private static ProgressTimeEstimator _progressTimeEstimator;

        public MainForm()
        {
            InitializeComponent();

            //load initial settings
            tbTFSUrl.Text = Settings.Default.TFSUrl;
            tbSVNUrl.Text = Settings.Default.SVNUrl;
            tbTFSUsername.Text = Settings.Default.TFSUsername;
            tbTFSDomain.Text = Settings.Default.TFSDomain;
            tbChangesetStart.Text = Settings.Default.FromChangeset.ToString();
            tbWorkingCopyFolder.Text = Settings.Default.WorkingCopyPath;
            cbDoInitialCheckout.Checked = Settings.Default.DoInitialCheckout;

            if (TfsClient.Providers != null)
            {
                foreach (TfsClientProviderBase tfsProvider in TfsClient.Providers)
                {
                    comboTfsClientProvider.Items.Insert(0, tfsProvider.Name);
                }

                for (int i = 0; i < comboTfsClientProvider.Items.Count; i++)
                {
                    if ((string)comboTfsClientProvider.Items[i] == (String.IsNullOrEmpty(Settings.Default.TFSClientProvider) ? TfsClient.Provider.Name : Settings.Default.TFSClientProvider))
                    {
                        comboTfsClientProvider.SelectedIndex = i;
                        break;
                    }
                }
            }

            //init log4net
            XmlConfigurator.Configure();
        }

        #region Event Handlers

        void tfs2svnConverter_BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            log.Info("Processing TFS Changeset #" + changeset);

            //Show message in listBox
            this.BeginInvoke(
                new MethodInvoker(delegate() { AddListboxLine(String.Format("Processing TFS Changeset: {0} ...", changeset)); }));
        }
        void tfs2svnConverter_ChangeSetsFound(int totalChangesets)
        {
            _progressTimeEstimator = new ProgressTimeEstimator(DateTime.Now, totalChangesets);

            //set progressbar
            this.BeginInvoke(
                new MethodInvoker(delegate()
            {
                progressBar1.Maximum = totalChangesets;
                progressBar1.Minimum = 0;
                progressBar1.Step = 1;
            }));
        }
        void tfs2svnConverter_SvnAdminEvent(string svnAdminMessage)
        {
            //Show message in listBox
            this.BeginInvoke(
                new MethodInvoker(delegate() { AddListboxLine(svnAdminMessage); }));
        }
        void tfs2svnConverter_EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            log.Info("Finished processing Changeset #" + changeset);
            _progressTimeEstimator.Update();

            //Show message in listBox
            this.BeginInvoke(
                new MethodInvoker(delegate() { AppendListboxText(" done"); progressBar1.Value++; }));

            this.BeginInvoke(
                new MethodInvoker(delegate() { lblTimeRemaining.Text = _progressTimeEstimator.GetApproxTimeRemaining(); }));
        }
        void tfs2svnConverter_FolderUndeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "Folder", "Undelete", path, "", Color.Pink);
        }
        void tfs2svnConverter_FolderRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "Folder", "Rename", newPath, oldPath, Color.YellowGreen);
        }
        void tfs2svnConverter_FolderDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "Folder", "Delete", path, "", Color.Red);
        }
        void tfs2svnConverter_FolderBranched(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "Folder", "Branch", path, "", Color.Orange);
        }
        void tfs2svnConverter_FolderAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "Folder", "Add", path, "", Color.Green);
        }
        void tfs2svnConverter_FileRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "File", "Rename", newPath, oldPath, Color.YellowGreen);
        }
        void tfs2svnConverter_FileEdited(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "File", "Edit", path, "", Color.Blue);
        }
        void tfs2svnConverter_FileDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "File", "Delete", path, "", Color.Red);
        }
        void tfs2svnConverter_FileBranched(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "File", "Branch", path, "", Color.Orange);
        }
        void tfs2svnConverter_FileAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            AddMovementLine(changeset, "File", "Add", path, "", Color.Green);
        }
        void tfs2svnConverter_SvnAuthenticationRetry(string command, int retryCount)
        {
            AddMovementLine(-1, "Authentication failed! Retrying...", "", "", "", Color.DarkRed);
        }

        private void tbSVNUrl_TextChanged(object sender, EventArgs e)
        {
            cbCreateRepository.Enabled = tbSVNUrl.Text.StartsWith("file:///");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            button1.Enabled = false;
            lstStatus.Items.Clear();
            lstMovement.Items.Clear();

            Thread thread = new Thread(DoWork);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        #endregion

        #region Private Methods

        private void HookupEventHandlers(Tfs2SvnConverter tfs2svnConverter)
        {
            tfs2svnConverter.BeginChangeSet += tfs2svnConverter_BeginChangeSet;
            tfs2svnConverter.EndChangeSet += tfs2svnConverter_EndChangeSet;
            tfs2svnConverter.ChangeSetsFound += tfs2svnConverter_ChangeSetsFound;
            tfs2svnConverter.SvnAdminEvent += tfs2svnConverter_SvnAdminEvent;
            tfs2svnConverter.FileAdded += tfs2svnConverter_FileAdded;
            tfs2svnConverter.FileBranched +=  tfs2svnConverter_FileBranched;
            tfs2svnConverter.FileDeleted += tfs2svnConverter_FileDeleted;
            tfs2svnConverter.FileEdited += tfs2svnConverter_FileEdited;
            tfs2svnConverter.FileRenamed += tfs2svnConverter_FileRenamed;
            tfs2svnConverter.FolderAdded += tfs2svnConverter_FolderAdded;
            tfs2svnConverter.FolderBranched +=  tfs2svnConverter_FolderBranched;
            tfs2svnConverter.FolderDeleted +=  tfs2svnConverter_FolderDeleted;
            tfs2svnConverter.FolderRenamed +=  tfs2svnConverter_FolderRenamed;
            tfs2svnConverter.FolderUndeleted += tfs2svnConverter_FolderUndeleted;
            tfs2svnConverter.SvnAuthenticationRetry += tfs2svnConverter_SvnAuthenticationRetry;
        }

        
        private void AddMovementLine(int changeset, string type, string action, string newPath, string oldPath, Color color)
        {
            this.BeginInvoke(
                new MethodInvoker(delegate()
                                      {
                                          ListViewItem listViewItem = new ListViewItem(new string[] { changeset.ToString(), type, action, newPath, oldPath });
                                          listViewItem.ForeColor = color;
                                          lstMovement.Items.Add(listViewItem);
                                          lstMovement.Items[lstMovement.Items.Count - 1].EnsureVisible();

                                      }));
        }
        private void DoWork(object obj)
        {
            try
            {
                string tfsUrl = Settings.Default.TFSUrl = tbTFSUrl.Text;
                string svnUrl = Settings.Default.SVNUrl = tbSVNUrl.Text;
                int startChangeset = Settings.Default.FromChangeset = int.Parse(tbChangesetStart.Text);
                string workingCopyFolder = Settings.Default.WorkingCopyPath = tbWorkingCopyFolder.Text;
                workingCopyFolder = workingCopyFolder.Replace("[MyDocuments]", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                bool doInitialCheckout = Settings.Default.DoInitialCheckout = cbDoInitialCheckout.Checked;
                string tfsUsername = Settings.Default.TFSUsername = tbTFSUsername.Text;
                string tfsDomain = Settings.Default.TFSDomain = tbTFSDomain.Text;
                string tfsPassword = tbTFSPassword.Text;
                Settings.Default.Save(); //save settings

                //starting converting
                log.Info(String.Format("======== Starting tfs2svn converting from {0} to {1}", tfsUrl, svnUrl));
                this.BeginInvoke(
                    new MethodInvoker(delegate() { AddListboxLine("Starting converting from TFS to SVN"); }));

                Tfs2SvnConverter tfs2svnConverter = new Tfs2SvnConverter(tfsUrl, svnUrl, cbCreateRepository.Enabled && cbCreateRepository.Checked, startChangeset, workingCopyFolder, Settings.Default.SvnBinFolder, doInitialCheckout, tfsUsername, tfsPassword, tfsDomain);
                HookupEventHandlers(tfs2svnConverter);
                AddUsernameMappings(tfs2svnConverter);
                tfs2svnConverter.Convert();

                //done converting
                log.Info("======== Finished tfs2svn converting");
                this.BeginInvoke(
                    new MethodInvoker(delegate() { AddListboxLine("Finished converting!"); }));
            }
            catch (Exception ex)
            {
                log.Error("Exception while converting", ex);
                this.BeginInvoke(
                    new MethodInvoker(delegate() { AddListboxLine("!!!ERROR(S) FOUND"); MessageBox.Show(ex.ToString(), "Error found"); }));
            }
            finally
            {
                this.BeginInvoke(
                    new MethodInvoker(delegate() { button1.Enabled = true; }));
            }
        }
        private void AddUsernameMappings(Tfs2SvnConverter tfs2svnConverter)
        {
            StringCollection mappings = Settings.Default.TFS2SVNUserMappings;
            foreach (string mapping in mappings)
            {
                string tfsUserName = mapping.Split(';')[0];
                string svnUserName = mapping.Split(';')[1];

                tfs2svnConverter.AddUsernameMapping(tfsUserName, svnUserName);
            }
        }
        private void AddListboxLine(string message)
        {
            lstStatus.Items.Add(message);
            lstStatus.SetSelected(lstStatus.Items.Count - 1, true);
            lstStatus.SetSelected(lstStatus.Items.Count - 1, false);
        }
        private void AppendListboxText(string message)
        {
            lstStatus.Items[lstStatus.Items.Count - 1] = lstStatus.Items[lstStatus.Items.Count - 1] + message;
        }

        private void comboTfsClientProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set the choosen tfsclient provider
            string selectedProvideName = (string)comboTfsClientProvider.SelectedItem;
            TfsClient.SetProvider(selectedProvideName);

            Settings.Default.TFSClientProvider = selectedProvideName;
            Settings.Default.Save();
        }
        #endregion

    }
}