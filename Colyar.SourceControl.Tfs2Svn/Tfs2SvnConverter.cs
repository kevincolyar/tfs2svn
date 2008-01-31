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
        
        //private readonly TfsExporter _tfsExporter;
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

            //this._tfsExporter = new TfsExporter(this._tfsServer, this._tfsRepository, workingCopyPath, fromChangeset, tfsUsername, tfsPassword, tfsDomain);
            TfsClient.Provider.Connect(this._tfsServer, this._tfsRepository, workingCopyPath, fromChangeset, tfsUsername, tfsPassword, tfsDomain);

            this._svnImporter = new SvnImporter(this._svnRepository, workingCopyPath, svnBinFolder);
            _createSvnFileRepository = createSvnFileRepository;
            _doInitialCheckout = doInitialCheckout;
            _workingCopyPath = workingCopyPath;

            HookupTfsExporterEventHandlers();
        }

        #endregion

        #region Public Property Events

        public event ChangesetHandler BeginChangeSet
        {
            add { TfsClient.Provider.BeginChangeSet += value; }
            remove { TfsClient.Provider.BeginChangeSet -= value; }
        }
        public event ChangesetsFoundHandler ChangeSetsFound
        {
            add { TfsClient.Provider.ChangeSetsFound += value; }
            remove { TfsClient.Provider.ChangeSetsFound -= value; }
        }
        public event ChangesetHandler EndChangeSet
        {
            add { TfsClient.Provider.EndChangeSet += value; }
            remove { TfsClient.Provider.EndChangeSet -= value; }
        }
        public event SvnAdminEventHandler SvnAdminEvent;
        public event SinglePathHandler FileAdded
        {
            add { TfsClient.Provider.FileAdded += value; }
            remove { TfsClient.Provider.FileAdded -= value; }
        }
        public event SinglePathHandler FileBranched
        {
            add { TfsClient.Provider.FileBranched += value; }
            remove { TfsClient.Provider.FileBranched -= value; }
        }
        public event SinglePathHandler FileDeleted
        {
            add { TfsClient.Provider.FileDeleted += value; }
            remove { TfsClient.Provider.FileDeleted -= value; }
        }
        public event SinglePathHandler FileEdited
        {
            add { TfsClient.Provider.FileEdited += value; }
            remove { TfsClient.Provider.FileEdited -= value; }
        }
        public event DualPathHandler FileRenamed
        {
            add { TfsClient.Provider.FileRenamed += value; }
            remove { TfsClient.Provider.FileRenamed -= value; }
        }
        public event SinglePathHandler FileUndeleted
        {
            add { TfsClient.Provider.FileUndeleted += value; }
            remove { TfsClient.Provider.FileUndeleted -= value; }
        }
        public event SinglePathHandler FolderAdded
        {
            add { TfsClient.Provider.FolderAdded += value; }
            remove { TfsClient.Provider.FolderAdded -= value; }
        }
        public event SinglePathHandler FolderBranched
        {
            add { TfsClient.Provider.FolderBranched += value; }
            remove { TfsClient.Provider.FolderBranched -= value; }
        }
        public event SinglePathHandler FolderDeleted
        {
            add { TfsClient.Provider.FolderDeleted += value; }
            remove { TfsClient.Provider.FolderDeleted -= value; }
        }
        public event DualPathHandler FolderRenamed
        {
            add { TfsClient.Provider.FolderRenamed += value; }
            remove { TfsClient.Provider.FolderRenamed -= value; }
        }
        public event SinglePathHandler FolderUndeleted
        {
            add { TfsClient.Provider.FolderUndeleted += value; }
            remove { TfsClient.Provider.FolderUndeleted -= value; }
        }
        public event SvnCommandRetryHandler SvnAuthenticationRetry
        {
            add { this._svnImporter.AuthenticationRetry += value; }
            remove { this._svnImporter.AuthenticationRetry -= value; }
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
            TfsClient.Provider.ProcessAllChangeSets();
        }
        public void AddUsernameMapping(string tfsUsername, string svnUsername)
        {
            this._svnImporter.AddUsernameMapping(tfsUsername, svnUsername);
        }

        #endregion

        #region Private Methods

        private void HookupTfsExporterEventHandlers()
        {
            TfsClient.Provider.BeginChangeSet += tfsExporter_BeginChangeSet;
            TfsClient.Provider.EndChangeSet += tfsExporter_EndChangeSet;
            TfsClient.Provider.FileAdded += tfsExporter_FileAdded;
            TfsClient.Provider.FileDeleted += tfsExporter_FileDeleted;
            TfsClient.Provider.FileEdited += tfsExporter_FileEdited;
            TfsClient.Provider.FileRenamed += tfsExporter_FileRenamed;
            TfsClient.Provider.FileBranched += tfsExporter_FileBranched;
            TfsClient.Provider.FileUndeleted += tfsExporter_FileUndeleted;
            TfsClient.Provider.FolderAdded += tfsExporter_FolderAdded;
            TfsClient.Provider.FolderDeleted += tfsExporter_FolderDeleted;
            TfsClient.Provider.FolderRenamed += tfsExporter_FolderRenamed;
            TfsClient.Provider.FolderBranched += tfsExporter_FolderBranched;
            TfsClient.Provider.FolderUndeleted += tfsExporter_FolderUndeleted;
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

            oldPath = FixPreviouslyRenamedFolder(oldPath);

            if (oldPath == newPath)
                return; //no need for a rename
            
            if (!File.Exists(oldPath))
                throw new Exception("File error in tfsExporter_FileRenamed");
            
            if (!File.Exists(newPath))
            {
                this._svnImporter.MoveFile(oldPath, newPath, false);
            }
            else
            {
                //check if no file exists with same case (i.e.: in that case the file was renamed automatically when a parent-folder was renamed)
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
