using System;
using System.Threading;
using System.Windows.Forms;
using Colyar.SourceControl.Tfs2Svn;
using log4net;
using tfs2svn.Winforms.Properties;
using log4net.Config;
using System.Collections.Specialized;

namespace tfs2svn.Winforms
{
    public partial class MainForm : Form
    {
        // Static log4net logger instance
        private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));

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

            //init log4net
            XmlConfigurator.Configure();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            button1.Enabled = false;
            listBox1.Items.Clear();

            Thread thread = new Thread(DoWork);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
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
            listBox1.Items.Add(message);
            listBox1.SetSelected(listBox1.Items.Count - 1, true);
            listBox1.SetSelected(listBox1.Items.Count - 1, false);
        }
        private void AppendListboxText(string message)
        {
            listBox1.Items[listBox1.Items.Count - 1] = listBox1.Items[listBox1.Items.Count - 1] + message;
        }

        #region Event Handlers

        void BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            log.Info("Processing TFS Changeset #" + changeset);

            //Show message in listBox
            this.BeginInvoke(
                new MethodInvoker(delegate() { AddListboxLine(String.Format("Processing TFS Changeset: {0} ...", changeset)); }));
        }

        void ChangeSetsFound(int totalChangesets)
        {
            //set progressbar
            this.BeginInvoke(
                new MethodInvoker(delegate()
            {
                progressBar1.Maximum = totalChangesets;
                progressBar1.Minimum = 0;
                progressBar1.Step = 1;
            }));
        }

        void SvnAdminEvent(string svnAdminMessage)
        {
            //Show message in listBox
            this.BeginInvoke(
                new MethodInvoker(delegate() { AddListboxLine(svnAdminMessage); }));
        }

        void EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            log.Info("Finished processing Changeset #" + changeset);

            //Show message in listBox
            this.BeginInvoke(
                new MethodInvoker(delegate() { AppendListboxText(" done"); progressBar1.Value++; }));
        }

        #endregion

        #region Private Methods

        private void HookupEventHandlers(Tfs2SvnConverter tfs2svnConverter)
        {
            tfs2svnConverter.BeginChangeSet += BeginChangeSet;
            tfs2svnConverter.EndChangeSet += EndChangeSet;
            tfs2svnConverter.ChangeSetsFound += ChangeSetsFound;
            tfs2svnConverter.SvnAdminEvent += SvnAdminEvent;
        }
        #endregion

        private void tbSVNUrl_TextChanged(object sender, EventArgs e)
        {
            cbCreateRepository.Enabled = tbSVNUrl.Text.StartsWith("file:///");
        }
    }
}