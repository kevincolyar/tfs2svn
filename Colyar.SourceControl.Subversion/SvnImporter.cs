using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Colyar.SourceControl;
using System.Text.RegularExpressions;
using System.IO;

namespace Colyar.SourceControl.Subversion
{
    public class SvnImporter
    {
        #region Private Variables

        private string _repositoryPath;
        private string _workingCopyPath;
        private readonly string _svnPath = @"C:\Program Files\Subversion\bin";
        private readonly Dictionary<string, byte[]> _passwordHash = new Dictionary<string, byte[]>();
        private readonly Dictionary<string, string> _usernameMap = new Dictionary<string, string>();

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

        #region Public Events

        public event ConsoleOutputHandler ConsoleOutput;
        public event SubversionCommandErrorHandler SubversionCommandError;

        #endregion

        #region Public Constructor

        public SvnImporter(string repositoryPath, string workingCopyPath)
        {
            this._repositoryPath = repositoryPath.Replace("\\", "/");
            this._workingCopyPath = workingCopyPath;
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
            RunSvnCommand("update \"" + this._workingCopyPath + "\"");
        }
        public void Commit(string message, string committer)
        {
            string username = GetUser(committer);
            string password = GetPassword(committer);
            
            if(message != null) message = message.Replace("\"", "");

            string command = "commit \"" + this._workingCopyPath + "\" -m \"" + message + "\"";

            if (username != "") command += " --username " + username;
            if (password != "") command += " --password " + password;

            RunSvnCommand(command);
        }
        public void Add(string path)
        {
            RunSvnCommand("add \"" + path + "\"");
        }
        public void Remove(string path)
        {
            RunSvnCommand("rm \"" + path + "\"");
        }
        public void Move(string oldPath, string newPath)
        {
            RunSvnCommand("mv \"" + oldPath + "\" \"" + newPath + "\"");
        }
        public void Branch(string oldPath, string newPath)
        {
            RunSvnCommand("branch \"" + oldPath + "\" \"" + newPath + "\"");
        }
        public void SetCommitDate(string path, DateTime date)
        {
            RunSvnCommand(String.Format("propset svn:date --revprop -rHEAD {0} \"{1}\"" , date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture), path));
        }
        
        public void AddUser(string username, string password)
        {
            this._usernameMap[username] = username;
            this._passwordHash[username] = Convert.FromBase64String(password);
        }
        public void AddUserMapping(string regex, string username, string password)
        {
            this._usernameMap[regex] = username;
            this._passwordHash[username] = Convert.FromBase64String(password);
        }

        #endregion

        #region Private Methods

        public void AddRevisionPropertyChangeHookFile(string path)
        {
            string hookPath = path + "/hooks/pre-revprop-change.cmd";

            if (File.Exists(hookPath))
                return;

            File.Create(hookPath);
        }

        private void RunSvnCommand(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = this._svnPath + @"\svn.exe";
            p.StartInfo.Arguments = command;

            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            p.Start();
            ParseSvnProcessOuput(command, p.StandardOutput.ReadToEnd());
            ParseSvnProcessOuput(command, p.StandardError.ReadToEnd());
            p.WaitForExit();

        }
        private void RunSvnAdminCommand(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = this._svnPath + @"\svnadmin.exe";
            p.StartInfo.Arguments = command;

            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            p.Start();
            ParseSvnAdminProcessOuput(command, p.StandardOutput.ReadToEnd());
            ParseSvnAdminProcessOuput(command, p.StandardError.ReadToEnd());
            p.WaitForExit();
        }

        private void ParseSvnProcessOuput(string input, string output)
        {
            if (Regex.IsMatch(output, "^svn(.exe)?:", RegexOptions.IgnoreCase))
            {
                if(this.SubversionCommandError != null)
                    this.SubversionCommandError(input, output, DateTime.Now);
            }

            if (output != "" && this.ConsoleOutput != null)
                this.ConsoleOutput(output);
        }
        private void ParseSvnAdminProcessOuput(string input, string output)
        {
            if (Regex.IsMatch(output, "^svnadmin(.exe)?:", RegexOptions.IgnoreCase))
            {
                if (this.SubversionCommandError != null)
                    this.SubversionCommandError(input, output, DateTime.Now);
            }

            if (output != "" && this.ConsoleOutput != null)
                this.ConsoleOutput(output);
        }

        private string GetUser(string committer)
        {
            foreach (string regex in this._usernameMap.Keys)
                if (Regex.IsMatch(committer, regex, RegexOptions.IgnoreCase))
                    return this._usernameMap[regex];

            return committer;
        }
        private string GetPassword(string committer)
        {
            string user = GetUser(committer);

            if(this._passwordHash.ContainsKey(user))
                return Convert.ToBase64String(this._passwordHash[user]);
            
            return "";
        }

        #endregion
    }
}
