using System;
using System.Threading;
using System.Windows.Forms;
using Colyar.SourceControl.Tfs2Svn;

namespace Colyar.SourceControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(DoWork);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        private void DoWork(object obj)
        {
            Tfs2SvnConverter tfs2svnConverter = new Tfs2SvnConverter("https://tfs.dcpud.net/AMMPS", "file:///" + Environment.CurrentDirectory + "/AMMPS", true);
            tfs2svnConverter.AddUserMapping("kevinc", "kevinc", "password");
            tfs2svnConverter.AddUserMapping("kevinb", "kevinb", "password");
            tfs2svnConverter.AddUserMapping("brianr", "brianr", "password");
            tfs2svnConverter.AddUserMapping("richk", "richk", "password");
            tfs2svnConverter.AddUserMapping("susanm", "susanm", "password");

            HookupEventHandlers(tfs2svnConverter);

            tfs2svnConverter.Convert();
        }
        

        #region Event Handlers

        void BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("New Changeset: " + changeset);
        }
        void EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Commiting Changeset: " + changeset);
        }
        void FileAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FileEdited(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FileDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FileBranched(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FileRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            Report(changeset, newPath, committer, comment);
        }
        void FolderAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FolderDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FolderBranched(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        void FolderRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            Report(changeset, newPath, committer, comment);
        }

        void tfs2svnConverter_SvnConsoleOutput(string output)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
                                                   {
                                                       txtOutput.Text += output + Environment.NewLine;
                                                   }));
        }

        #endregion

        #region Private Methods

        void Report(int changeset, string path, string committer, string comment)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
                                                        {
                                                            txtOutput.Text += "------------------------------------------------------------------------" + Environment.NewLine;
                                                            txtOutput.Text += "Changeset: " + changeset + Environment.NewLine;
                                                            txtOutput.Text += "Path     : " + path + Environment.NewLine;
                                                            txtOutput.Text += "Committer: " + committer + Environment.NewLine;
                                                            txtOutput.Text += "Comment  : " + comment + Environment.NewLine;
                                                            
                                                            txtOutput.ScrollToCaret();
                                                        }));
        }
        private void HookupEventHandlers(Tfs2SvnConverter tfs2svnConverter)
        {
            tfs2svnConverter.BeginChangeSet += BeginChangeSet;
            tfs2svnConverter.EndChangeSet += EndChangeSet;
            tfs2svnConverter.FileAdded += FileAdded;
            tfs2svnConverter.FileDeleted += FileDeleted;
            tfs2svnConverter.FileEdited += FileEdited;
            tfs2svnConverter.FileRenamed += FileRenamed;
            tfs2svnConverter.FileBranched += FileBranched;
            tfs2svnConverter.FolderAdded += FolderAdded;
            tfs2svnConverter.FolderDeleted += FolderDeleted;
            tfs2svnConverter.FolderRenamed += FolderRenamed;
            tfs2svnConverter.FolderBranched += FolderBranched;

            tfs2svnConverter.SubversionConsoleOutput += tfs2svnConverter_SvnConsoleOutput;
        }

        #endregion
    }
}