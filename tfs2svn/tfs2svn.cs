using System;
using System.IO;
using System.Text.RegularExpressions;
using Colyar.SourceControl;
using Colyar.SourceControl.Subversion;
using Colyar.SourceControl.TeamFoundationServer;

namespace Colyar.SourceControl
{
    public class tfs2svn
    {
        #region Private Variables
        
        private readonly TfsExporter _tfsExporter;
        private readonly SvnImporter _svnImporter;

        private string _tfsServer;
        private string _tfsRepository;
        private string _svnRepository;


        private readonly string _tfsUrlRegex = @"(?<server>https?://([\w+-]\.?)+(:\d+)?)(?<repo>(/[\w- ]+)+)?";
        private readonly string _svnUrlRegex = @"(?<server>(https?|file|svn|svn\+ssh):///?([\w-]+\.?)+)(?<repo>(/[\w-]+)+)?";

        #endregion

        #region Public Constructor

        public tfs2svn(string tfsPath, string svnPath, bool createSvnRepository)
        {
            ParsePaths(tfsPath, svnPath);

            string workingPath = Path.GetTempPath() + "tfs2svn";
            DeletePath(workingPath);

            this._tfsExporter = new TfsExporter(this._tfsServer, this._tfsRepository, workingPath);
            this._svnImporter = new SvnImporter(this._svnRepository, workingPath);

            if(createSvnRepository && this._svnRepository.StartsWith("file:///"))
            {
                string localSvnPath = this._svnRepository.Replace("file:///", "");

                DeletePath(localSvnPath);
                this._svnImporter.CreateRepository(localSvnPath);
                this._svnImporter.AddRevisionPropertyChangeHookFile(localSvnPath);
                this._svnImporter.Checkout();
            }

            HookupTfsExporterEventHandlers();
        }

        #endregion

        #region Public Properties

        public TfsExporter TfsExporter
        {
            get { return this._tfsExporter; }
        }

        public SvnImporter SvnExporter
        {
            get { return this._svnImporter; }
        }

        #endregion

        #region Public Property Events

        public event ChangesetHandler BeginChangeSet
        {
            add { this._tfsExporter.BeginChangeSet += value; }
            remove { this._tfsExporter.BeginChangeSet -= value; }
        }
        public event ChangesetHandler EndChangeSet
        {
            add { this._tfsExporter.EndChangeSet += value; }
            remove { this._tfsExporter.EndChangeSet -= value; }
        }
        public event SinglePathHandler FileAdded
        {
            add { this._tfsExporter.FileAdded += value; }
            remove { this._tfsExporter.FileAdded -= value; }
        }
        public event SinglePathHandler FileDeleted
        {
            add { this._tfsExporter.FileDeleted += value; }
            remove { this._tfsExporter.FileDeleted -= value; }
        }
        public event SinglePathHandler FileEdited
        {
            add { this._tfsExporter.FileEdited += value; }
            remove { this._tfsExporter.FileEdited -= value; }
        }
        public event DualPathHandler FileRenamed
        {
            add { this._tfsExporter.FileRenamed += value; }
            remove { this._tfsExporter.FileRenamed -= value; }
        }
        public event DualPathHandler FileBranched
        {
            add { this._tfsExporter.FileBranched += value; }
            remove { this._tfsExporter.FileBranched -= value; }
        }
        public event SinglePathHandler FolderAdded
        {
            add { this._tfsExporter.FolderAdded += value; }
            remove { this._tfsExporter.FolderAdded -= value; }
        }
        public event SinglePathHandler FolderDeleted
        {
            add { this._tfsExporter.FolderDeleted += value; }
            remove { this._tfsExporter.FolderDeleted -= value; }
        }
        public event DualPathHandler FolderRenamed
        {
            add { this._tfsExporter.FolderRenamed += value; }
            remove { this._tfsExporter.FolderRenamed -= value; }
        }
        public event DualPathHandler FolderBranched
        {
            add { this._tfsExporter.FolderBranched += value; }
            remove { this._tfsExporter.FolderBranched -= value; }
        }

        public event ConsoleOutputHandler SubversionConsoleOutput
        {
            add { this._svnImporter.ConsoleOutput += value; }
            remove { this._svnImporter.ConsoleOutput -= value; }
        }
        public event SubversionCommandErrorHandler SubversionCommandError
        {
            add { this._svnImporter.SubversionCommandError += value; }
            remove { this._svnImporter.SubversionCommandError -= value; }
        }

        #endregion

        #region Public Methods

        public void Convert()
        {
            this._tfsExporter.ProcessAllChangeSets();
        }
        public void AddUserMapping(string regex, string username, string password)
        {
            this._svnImporter.AddUserMapping(regex, username, password);
        }

        #endregion

        #region Private Methods

        private void HookupTfsExporterEventHandlers()
        {
            this._tfsExporter.BeginChangeSet += tfsExporter_BeginChangeSet;
            this._tfsExporter.EndChangeSet += tfsExporter_EndChangeSet;
            this._tfsExporter.FileAdded += tfsExporter_FileAdded;
            this._tfsExporter.FileDeleted += tfsExporter_FileDeleted;
            this._tfsExporter.FileEdited += tfsExporter_FileEdited;
            this._tfsExporter.FileRenamed += tfsExporter_FileRenamed;
            this._tfsExporter.FileBranched += tfsExporter_FileBranched;
            this._tfsExporter.FileUndeleted += tfsExporter_FileUndeleted;
            this._tfsExporter.FolderAdded += tfsExporter_FolderAdded;
            this._tfsExporter.FolderDeleted += tfsExporter_FolderDeleted;
            this._tfsExporter.FolderRenamed += tfsExporter_FolderRenamed;
            this._tfsExporter.FolderBranched += tfsExporter_FolderBranched;
            this._tfsExporter.FolderUndeleted += tfsExporter_FolderUndeleted;
        }

        private void ParsePaths(string tfsPath, string svnPath)
        {
            this._tfsServer = ParseTfsServer(tfsPath);
            this._tfsRepository = ParseTfsRepository(tfsPath);

            this._svnRepository = ParseSvnRepository(svnPath);
        }
        
        private string ParseTfsServer(string tfsPath)
        {
            Match match = Regex.Match(tfsPath, this._tfsUrlRegex, RegexOptions.IgnoreCase);

            return match.Groups["server"].Value;
        }
        private string ParseTfsRepository(string tfsPath)
        {
            Match match = Regex.Match(tfsPath, this._tfsUrlRegex, RegexOptions.IgnoreCase);

            return "$" +  match.Groups["repo"].Value;
        }
        private string ParseSvnRepository(string svnPath)
        {
            return svnPath;
        }

        private void DeletePath(string path)
        {
            if (!Directory.Exists(path))
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                File.SetAttributes(fileInfo.FullName, FileAttributes.Normal);

            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                DeletePath(subDirectoryInfo.FullName);

            Directory.Delete(path, true);
        }

        #endregion

        #region Event Handlers

        void tfsExporter_BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            this._svnImporter.Update();
        }
        void tfsExporter_EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            this._svnImporter.Commit(comment, committer);
            this._svnImporter.SetCommitDate(this._svnImporter.WorkingCopyPath, date);
        }
        
        void tfsExporter_FileAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            if (File.Exists(path))
                this._svnImporter.Add(path);
        }
        void tfsExporter_FileEdited(int changeset, string path, string committer, string comment, DateTime date)
        {
        }
        void tfsExporter_FileDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            if (File.Exists(path))
                this._svnImporter.Remove(path);
        }
        void tfsExporter_FileBranched(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            if (File.Exists(oldPath) && !File.Exists(newPath))
                this._svnImporter.Branch(oldPath, newPath);
        }
        void tfsExporter_FileRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            if (File.Exists(oldPath) && !File.Exists(newPath))
                this._svnImporter.Move(oldPath, newPath);
        }
        void tfsExporter_FileUndeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            if (File.Exists(path))
                this._svnImporter.Add(path);
        }

        void tfsExporter_FolderAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            if (Directory.Exists(path))
                this._svnImporter.Add(path);
        }
        void tfsExporter_FolderDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            if (Directory.Exists(path))
                this._svnImporter.Remove(path);
        }
        void tfsExporter_FolderBranched(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            if (Directory.Exists(oldPath) && !Directory.Exists(newPath))
                this._svnImporter.Branch(oldPath, newPath);
        }
        void tfsExporter_FolderRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            if (Directory.Exists(oldPath) && !Directory.Exists(newPath))
                this._svnImporter.Move(oldPath, newPath);
        }
        void tfsExporter_FolderUndeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
             if (Directory.Exists(path))
                this._svnImporter.Add(path);
        }
                
        #endregion
    }
}
