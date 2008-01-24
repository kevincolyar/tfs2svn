using System;
using System.IO;
using System.Text.RegularExpressions;
using Colyar.SourceControl;
using Colyar.SourceControl.Subversion;
using Colyar.SourceControl.TeamFoundationServer;
using log4net;
using System.Collections.Generic;

namespace Colyar.SourceControl.Tfs2Svn
{
    public class Tfs2SvnConverter
    {
        #region Private Variables
        
        private readonly TfsExporter _tfsExporter;
        private readonly SvnImporter _svnImporter;
        private static readonly ILog log = LogManager.GetLogger(typeof(Tfs2SvnConverter));

        private string _tfsServer;
        private string _tfsRepository;
        private string _svnRepository;
        private string _workingCopyPath;

        private bool _createSvnFileRepository;
        private bool _doInitialCheckout;

        private readonly string _tfsUrlRegex = @"(?<server>https?://([\w+-]\.?)+(:\d+)?)(?<repo>(/[\w- ]+)+)?";
        //private readonly string _svnUrlRegex = @"(?<server>(https?|file|svn|svn\+ssh):///?([\w-]+\.?)+)(?<repo>(/[\w-]+)+)?";

        private Dictionary<string, string> fileSwapBackups = new Dictionary<string, string>();
        private Dictionary<string, string> renamedFolders = new Dictionary<string, string>();
        #endregion

        #region Public Constructor

        public Tfs2SvnConverter(string tfsPath, string svnPath, bool createSvnFileRepository, int fromChangeset, string workingCopyPath, string svnBinFolder, bool doInitialCheckout)
            : this(tfsPath, svnPath, createSvnFileRepository, fromChangeset, workingCopyPath, svnBinFolder, doInitialCheckout, null, null, null) { }

        public Tfs2SvnConverter(string tfsPath, string svnPath, bool createSvnFileRepository, int fromChangeset, string workingCopyPath, string svnBinFolder, bool doInitialCheckout, string tfsUsername, string tfsPassword, string tfsDomain)
        {
            ParsePaths(tfsPath, svnPath);

            this._tfsExporter = new TfsExporter(this._tfsServer, this._tfsRepository, workingCopyPath, fromChangeset, tfsUsername, tfsPassword, tfsDomain);
            this._svnImporter = new SvnImporter(this._svnRepository, workingCopyPath, svnBinFolder);
            _createSvnFileRepository = createSvnFileRepository;
            _doInitialCheckout = doInitialCheckout;
            _workingCopyPath = workingCopyPath;

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
        public event ChangesetsFoundHandler ChangeSetsFound
        {
            add { this._tfsExporter.ChangeSetsFound += value; }
            remove { this._tfsExporter.ChangeSetsFound -= value; }
        }
        public event ChangesetHandler EndChangeSet
        {
            add { this._tfsExporter.EndChangeSet += value; }
            remove { this._tfsExporter.EndChangeSet -= value; }
        }
        public event SvnAdminEventHandler SvnAdminEvent;
        public event SinglePathHandler FileAdded
        {
            add { this._tfsExporter.FileAdded += value; }
            remove { this._tfsExporter.FileAdded -= value; }
        }
        public event SinglePathHandler FileBranched
        {
            add { this._tfsExporter.FileBranched += value; }
            remove { this._tfsExporter.FileBranched -= value; }
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
        public event SinglePathHandler FileUndeleted
        {
            add { this._tfsExporter.FileUndeleted += value; }
            remove { this._tfsExporter.FileUndeleted -= value; }
        }
        public event SinglePathHandler FolderAdded
        {
            add { this._tfsExporter.FolderAdded += value; }
            remove { this._tfsExporter.FolderAdded -= value; }
        }
        public event SinglePathHandler FolderBranched
        {
            add { this._tfsExporter.FolderBranched += value; }
            remove { this._tfsExporter.FolderBranched -= value; }
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
        public event SinglePathHandler FolderUndeleted
        {
            add { this._tfsExporter.FolderUndeleted += value; }
            remove { this._tfsExporter.FolderUndeleted -= value; }
        }

        #endregion

        #region Public Methods

        public void Convert()
        {
            //see if repository should be created (e.g. file:///c:\myrepository)
            if (_createSvnFileRepository && this._svnRepository.StartsWith("file:///"))
            {
                string localSvnPath = this._svnRepository.Replace("file:///", String.Empty).Replace("/","\\");

                if (!String.IsNullOrEmpty(localSvnPath))
                    DeletePath(localSvnPath);

                log.Info("Start creating file repository " + localSvnPath);
                if (SvnAdminEvent != null)
                    SvnAdminEvent("Start creating file repository " + localSvnPath);

                this._svnImporter.CreateRepository(localSvnPath);

                //add empty Pre-RevisionPropertyChange hookfile (to make it possible to use propset)
                string hookPath = localSvnPath + "/hooks/pre-revprop-change.cmd";
                if (!File.Exists(hookPath))
                {
                    FileStream fs = File.Create(hookPath);
                    fs.Close();
                }

                log.Info("Finished creating file repository " + localSvnPath);
                if (SvnAdminEvent != null)
                    SvnAdminEvent("Finished creating file repository " + localSvnPath);
            }

            //initial checkout?
            if (_doInitialCheckout)
            {
                DeletePath(_workingCopyPath);
                this._svnImporter.Checkout();
            }

            //now read and process all TFS changesets
            this._tfsExporter.ProcessAllChangeSets();
        }
        public void AddUsernameMapping(string tfsUsername, string svnUsername)
        {
            this._svnImporter.AddUsernameMapping(tfsUsername, svnUsername);
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

            //unhide .svn folders 
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                File.SetAttributes(fileInfo.FullName, FileAttributes.Normal);

            //delete recursively
            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                DeletePath(subDirectoryInfo.FullName);

            Directory.Delete(path, true);
        }

        private string GetBackupFilename(string path)
        {
            return path.Insert(path.LastIndexOf(@"\") + 1, "___temp");
        }

        private string FixPreviouslyRenamedFolder(string path)
        {
            if (path != null)
            {
                foreach (string preRenameFolder in renamedFolders.Keys)
                {
                    if (path.ToLowerInvariant().StartsWith(preRenameFolder.ToLowerInvariant()))
                    {
                        path = path.Remove(0, preRenameFolder.Length).Insert(0, renamedFolders[preRenameFolder]);
                        //note: do not break now: each next preRenameFolder must also be checked
                    }
                }
            }

            return path;
        }

        private bool FileWasMovedWithFolder(string path)
        {
            if (path == null)
                return false;

            foreach (string preRenameFolder in renamedFolders.Keys)
                if (path.ToLowerInvariant().StartsWith(preRenameFolder.ToLowerInvariant()))
                    return true;

            return false;
        }

        #endregion

        #region Event Handlers

        void tfsExporter_BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            renamedFolders.Clear();
            fileSwapBackups.Clear();
            //this._svnImporter.Update(); //no need: workingcopy should always be up-to-date
        }

        void tfsExporter_EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            //check if cyclic swapped files were all handled
            if (fileSwapBackups.Count > 0)
            {
                foreach (string destinationPath in fileSwapBackups.Keys)
                {
                    string sourcePath = fileSwapBackups[destinationPath];

                    if (!fileSwapBackups.ContainsKey(sourcePath))
                        throw new Exception("Error in file-swapping; cannot continue.");

                    string sourceSourcePath = GetBackupFilename(sourcePath);
                    File.Delete(sourceSourcePath);
                }
            }

            this._svnImporter.Commit(comment, committer, date, changeset);
        }
        
        void tfsExporter_FileAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info("Adding file " + path);

            if (!File.Exists(path))
                throw new Exception("File not found in tfsExporter_FileAdded");

            this._svnImporter.Add(path);
        }

        void tfsExporter_FileEdited(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info("Editing file " + path);
        }

        void tfsExporter_FileDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info("Deleting file " + path);

            if (File.Exists(path))
                this._svnImporter.Remove(path, false);
        }

        void tfsExporter_FileBranched(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info("Adding branched file " + path);

            if (!File.Exists(path))
                throw new Exception("File not found in tfsExporter_FileBranched");

            this._svnImporter.Add(path);
        }

        void tfsExporter_FileUndeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info("Adding undeleted file " + path);

            if (!File.Exists(path))
                throw new Exception("File not found in tfsExporter_FileUndeleted");

            this._svnImporter.Add(path);
        }

        void tfsExporter_FileRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            log.Info(String.Format("tfs2svn: Renaming file {0} to {1}", oldPath, newPath));

            if (FileWasMovedWithFolder(oldPath))
            {
                log.Info(String.Format("tfs2svn: Ingored renaming file {0} to {1}. File was renamed in a folder move.", oldPath, newPath));
                return;
            }

             //oldPath = FixPreviouslyRenamedFolder(oldPath);

            if (!File.Exists(oldPath))
                throw new Exception("File error in tfsExporter_FileRenamed");

            if (!File.Exists(newPath))
            {
                this._svnImporter.MoveFile(oldPath, newPath, false);
            }
            else
            {
                //check if no file exists with same case
                if (oldPath != newPath)
                {
                    if (oldPath.ToLowerInvariant() == newPath.ToLowerInvariant())
                    {
                        //rename with only casing different: do a server-side rename
                        this._svnImporter.MoveServerSide(oldPath, newPath, changeset, committer, date);
                    }
                    else
                    {
                        //this should be a file-swapping!!
                        log.Warn(String.Format("tfsExporter_FileRenamed: rename of file '{0}' to existing file '{1}'. This is only allowed in case of a 'filename-swapping'. Please check if this was the case.", oldPath, newPath));

                        if (fileSwapBackups.ContainsKey(newPath))
                            throw new Exception(String.Format("Problem renaming {0} to {1}. Another file was already renamed to target.", oldPath, newPath));

                        string tempNewPath = GetBackupFilename(newPath);
                        File.Copy(newPath, tempNewPath);

                        if (fileSwapBackups.ContainsKey(oldPath))
                        {
                            string tempOldPath = GetBackupFilename(oldPath);
                            File.Copy(tempOldPath, newPath, true);
                        }
                        else
                            File.Copy(oldPath, newPath, true);

                        fileSwapBackups.Add(newPath, oldPath);
                    }
                }
            }
        }

        void tfsExporter_FolderAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info(String.Format("Adding folder {0}", path));

            if (!Directory.Exists(path))
                throw new Exception("Directory not found in tfsExporter_FolderAdded");

            this._svnImporter.Add(path);
        }

        void tfsExporter_FolderDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info(String.Format("Deleting folder {0}", path));

            if (Directory.Exists(path) && path != _workingCopyPath) //cannot delete workingcopy root-folder
                this._svnImporter.Remove(path, true);
        }

        void tfsExporter_FolderBranched(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info(String.Format("Adding branched folder {0}", path));

            if (!Directory.Exists(path))
                throw new Exception("Directory not found in tfsExporter_FolderBranched");

            this._svnImporter.Add(path);
        }

        void tfsExporter_FolderUndeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            log.Info(String.Format("Adding undeleted folder {0}", path));

            if (!Directory.Exists(path))
                throw new Exception("Directory not found in tfsExporter_FolderUndeleted");

            this._svnImporter.Add(path);
        }

        void tfsExporter_FolderRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            log.Info(String.Format("tfs2svn: Renaming folder {0} to {1}", oldPath, newPath));

            oldPath = FixPreviouslyRenamedFolder(oldPath);

            if (oldPath == newPath)
                return; //no need for a rename

            if (!Directory.Exists(oldPath))
                throw new Exception("Folder error in tfsExporter_FolderRenamed");

            //rename to an existing directory is only allowed when the casing of the folder-name was changed 
            if (Directory.Exists(newPath) && oldPath.ToLowerInvariant() != newPath.ToLowerInvariant())
                throw new Exception("tfsExporter_FolderRenamed: renaming a folder to an already existing folder is not supported (yet)");

            //folder renames must be done server-side (see 'Moving files and folders' in http://tortoisesvn.net/docs/nightly/TortoiseSVN_sk/tsvn-dug-rename.html)
            this._svnImporter.MoveServerSide(oldPath, newPath, changeset, committer, date);
            renamedFolders.Add(oldPath, newPath);
        }
     
        #endregion
    }
}
