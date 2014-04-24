using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mime;
using System.Reflection;
using Colyar.SourceControl;
using System.Text.RegularExpressions;
using System.IO;
using log4net;
using System.Text;
using System.Threading;

namespace Colyar.SourceControl.Subversion
{
    public class SvnImporter
    {
        #region Private Variables

        private string _repositoryPath;
        private string _workingCopyPath;
        private readonly string _svnPath;
        private readonly Dictionary<string, string> _usernameMap = new Dictionary<string, string>();
        private static readonly ILog log = LogManager.GetLogger(typeof(SvnImporter));

        #endregion

        #region Public Properties

        public string WorkingCopyPath
        {
            get { return this._workingCopyPath; }
        }

        public string RepositoryPath
        {
            get { return this._repositoryPath; }
        }

        #endregion

        #region Public Constructor

        public SvnImporter(string repositoryPath, string workingCopyPath, string svnBinFolder)
        {
            this._repositoryPath = repositoryPath.Replace("\\", "/");
            this._workingCopyPath = workingCopyPath;
            this._svnPath = svnBinFolder;
        }

        #endregion

        #region Public Methods

        public void CreateRepository(string repositoryPath)
        {
            RunSvnAdminCommand("create \"" + repositoryPath + "\"");
        }
        public void CreateRepository()
        {
            CreateRepository(this._repositoryPath);
        }
        public void Checkout(string repositoryPath, string workingCopyPath)
        {
            this._repositoryPath = repositoryPath;
            this._workingCopyPath = workingCopyPath;

            Checkout();
        }
        public void Checkout()
        {
            RunSvnCommand("co \"" + this._repositoryPath + "\" \"" + this._workingCopyPath + "\"");
        }
        public void Update()
        {
            RunSvnCommand("up \"" + this._workingCopyPath + "\"");
        }
        public void Commit(string message, string committer, DateTime commitDate, int changeSet)
        {
            // clean-up message for svn and remove non-ASCII chars
            if (message != null)
            {
                message = message.Replace("\"", "\\\"").Replace("\r\n", "\n");
                // http://svnbook.red-bean.com/en/1.2/svn.advanced.l10n.html
                message = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(message));
            }

            message = String.Format("[TFS Changeset #{0}]\n{1}",
                changeSet.ToString(CultureInfo.InvariantCulture),
                message);

            RunSvnCommand(String.Format("commit \"{0}\" -m \"{1}\"",
                this._workingCopyPath,
                message));

            SetCommitAuthorAndDate(commitDate, committer);
        }
        public void Add(string path)
        {
            if (path != this._workingCopyPath)
            {
                AddMissingDirectoryIfNeeded(path);
                RunSvnCommand("add \"" + path + "\"");
            }
        }
        public void Remove(string path, bool isFolder)
        {
            RunSvnCommand("rm \"" + path + "\"");

            if (isFolder)
                RunSvnCommand("up \"" + path + "\"");
        }
        public void MoveFile(string oldPath, string newPath, bool isFolder)
        {
            AddMissingDirectoryIfNeeded(newPath);
            RunSvnCommand("mv \"" + oldPath + "\" \"" + newPath + "\"");
        }
        public void MoveServerSide(string oldPath, string newPath, int changeset, string committer, DateTime commitDate)
        {
            string oldUrl = _repositoryPath + ToUrlPath(oldPath.Remove(0, _workingCopyPath.Length));
            string newUrl = _repositoryPath + ToUrlPath(newPath.Remove(0, _workingCopyPath.Length));

            //when only casing is different, we need a server-side move/rename (because windows is case unsensitive!)

            RunSvnCommand(String.Format("mv \"{0}\" \"{1}\" --message \"[TFS Changeset #{2}]\ntfs2svn: server-side rename\"", oldUrl, newUrl, changeset));
            Update(); //todo: only update common rootpath of oldPath and newPath?

            SetCommitAuthorAndDate(commitDate, committer);
        }

        public void AddUsernameMapping(string tfsUsername, string svnUsername)
        {
            this._usernameMap[tfsUsername] = svnUsername;
        }
        #endregion

        #region Private Methods

        private void AddMissingDirectoryIfNeeded(string path)
        {
            string directory = Directory.GetParent(path).FullName;

            if (Directory.Exists(directory))
                return;

            log.Info("Adding: " + directory);
            Directory.CreateDirectory(directory);
            RunSvnCommand("add --force \"" + directory + "\"");
            Commit("Adding missing directory", "tfs2svn", DateTime.Today, 0);
        }
        private void SetCommitAuthorAndDate(DateTime commitDate, string committer)
        {
            string username = GetMappedUsername(committer);

            //set time after commit
            RunSvnCommand(String.Format("propset svn:date --revprop -rHEAD {0} \"{1}\"",
                commitDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture),
                this._workingCopyPath));

            RunSvnCommand(String.Format("propset svn:author --revprop -rHEAD \"{0}\" \"{1}\"",
                username,
                this._workingCopyPath));
        }
        private string ToUrlPath(string path)
        {
            return path.Replace("\\", "/");
        }

        private void RunSvnCommand(string command)
        {
            log.Info("svn " + command);

            Process p = new Process();
            p.StartInfo.FileName = this._svnPath + @"\svn.exe";
            p.StartInfo.Arguments = command;

            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

			//Debug.WriteLine("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);

			p.Start();
            p.PriorityClass = ProcessPriorityClass.High;
            p.StandardOutput.ReadToEnd(); //read standard output and swallow
            ParseSvnOuput(command, p.StandardError.ReadToEnd());
            p.WaitForExit();

        }
        private void RunSvnAdminCommand(string command)
        {
            log.Info("svnadmin " + command);

            Process p = new Process();
            p.StartInfo.FileName = this._svnPath + @"\svnadmin.exe";
            p.StartInfo.Arguments = command;

            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

			Debug.WriteLine("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);

            p.Start();
            p.PriorityClass = ProcessPriorityClass.High;
            p.StandardOutput.ReadToEnd(); //read standard output and swallow
            ParseSvnAdminOuput(command, p.StandardError.ReadToEnd());
            p.WaitForExit();
        }

        private void ParseSvnOuput(string input, string output)
        {
            if(Regex.Match(output, "^svn: warning:").Success)
            {
                log.Warn("Warning: " + output);
                return;
            }
            if (output != "")
            {
                throw new Exception(String.Format("svn error when executing 'svn {0}'. Exception: {1}.", input, output));
            }
        }

        private void ParseSvnAdminOuput(string input, string output)
        {
            if (output != "")
            {
                throw new Exception(String.Format("svn error when executing 'svn {0}'. Exception: {1}.", input, output));
            }
        }

        private string GetMappedUsername(string committer)
        {
            foreach (string tfsUsername in _usernameMap.Keys)
                if (committer.ToLowerInvariant().Contains(tfsUsername.ToLowerInvariant()))
                    return _usernameMap[tfsUsername];

            return committer; //no mapping found, return committer's unmapped name
        }

        #endregion
    }
}
