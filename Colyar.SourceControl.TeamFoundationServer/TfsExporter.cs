using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Colyar.SourceControl;

namespace Colyar.SourceControl.TeamFoundationServer
{
    public class TfsExporter
    {
        #region Public Events

        public event ChangesetHandler BeginChangeSet;
        public event ChangesetHandler EndChangeSet;
        public event SinglePathHandler FileAdded;
        public event SinglePathHandler FileEdited;
        public event SinglePathHandler FileDeleted;
        public event SinglePathHandler FileUndeleted;
        public event DualPathHandler FileRenamed;
        public event DualPathHandler FileBranched;
        public event SinglePathHandler FolderAdded;
        public event SinglePathHandler FolderDeleted;
        public event SinglePathHandler FolderUndeleted;
        public event DualPathHandler FolderRenamed;
        public event DualPathHandler FolderBranched;

        #endregion

        #region Private Variables

        private readonly string _serverUri;
        private readonly string _remotePath;
        private readonly string _localPath;
        private readonly Microsoft.TeamFoundation.Client.TeamFoundationServer _teamFoundationServer;
        private readonly VersionControlServer _versionControlServer;
        private Dictionary<int, string> _itemPaths = new Dictionary<int, string>();

        #endregion

        #region Public Constructor

        public TfsExporter(string serverUri, string remotePath, string localPath, string username, string password, string domain)
        {
            this._serverUri = serverUri;
            this._remotePath = remotePath;
            this._localPath = localPath;

            NetworkCredential networkCredential = new NetworkCredential(username, password, domain);

            this._teamFoundationServer = new Microsoft.TeamFoundation.Client.TeamFoundationServer(this._serverUri, networkCredential);
            this._versionControlServer = (VersionControlServer)this._teamFoundationServer.GetService(typeof(VersionControlServer));
        }
        public TfsExporter(string serverUri, string remotePath, string localPath)
        {
            this._serverUri = serverUri;
            this._remotePath = remotePath;
            this._localPath = localPath;

            this._teamFoundationServer = TeamFoundationServerFactory.GetServer(this._serverUri);
            this._versionControlServer = (VersionControlServer)this._teamFoundationServer.GetService(typeof(VersionControlServer));
        }

        #endregion

        #region Public Methods

        public void ProcessAllChangeSets()
        {
            foreach (Changeset changeset in GetChangesets())
            {
                if (this.BeginChangeSet != null)
                    this.BeginChangeSet(changeset.ChangesetId, changeset.Committer, changeset.Comment, changeset.CreationDate);

                foreach (Change change in OrderChanges(changeset.Changes))
                    ProcessChange(changeset, change);

                if (this.EndChangeSet != null)
                    this.EndChangeSet(changeset.ChangesetId, changeset.Committer, changeset.Comment, changeset.CreationDate);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable GetChangesets()
        {
            SortedList sortedChangesets = new SortedList();

            try
            {
                VersionSpec firstVersion = VersionSpec.ParseSingleSpec("1", this._versionControlServer.AuthenticatedUser);
                IEnumerable changesets = this._versionControlServer.QueryHistory(this._remotePath, VersionSpec.Latest, 0, RecursionType.Full, null, firstVersion, VersionSpec.Latest, int.MaxValue, true, false);

                foreach (Changeset changeset in changesets)
                    sortedChangesets.Add(changeset.ChangesetId, changeset);
             }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return sortedChangesets.Values;
        }
        private List<Change> OrderChanges(Change[] changes)
        {
          List<Change> fileChanges = new List<Change>();
          List<Change> folderChanges = new List<Change>();
          List<Change> returnChanges = new List<Change>();

          foreach (Change change in changes)
          {
              if (change.Item.ItemType == ItemType.File)
                  fileChanges.Add(change);
              else
                  folderChanges.Add(change);
          }

          returnChanges.AddRange(folderChanges);
          returnChanges.AddRange(fileChanges);
          return returnChanges;
        }

        private void ProcessChange(Changeset changeset, Change change)
        {
            if (change.Item.ItemType == ItemType.File)
              ProcessFileChange(changeset, change);
            else if (change.Item.ItemType == ItemType.Folder)
                ProcessFolderChange(changeset, change);
        }
        private void ProcessFileChange(Changeset changeset, Change change)
        {
            // Rename file.
            if ((change.ChangeType & ChangeType.Rename) == ChangeType.Rename)
                RenameFile(changeset, change);

            // Branch file.
            else if ((change.ChangeType & ChangeType.Branch) == ChangeType.Branch)
                AddFile(changeset, change);

            // Add file.
            else if ((change.ChangeType & ChangeType.Add) == ChangeType.Add)
                AddFile(changeset, change);

            // Delete file.
            else if ((change.ChangeType & ChangeType.Delete) == ChangeType.Delete)
                DeleteFile(changeset, change);

            // Edit file.
            else if ((change.ChangeType & ChangeType.Edit) == ChangeType.Edit)
                EditFile(changeset, change);

            // Undelete file.
            else if ((change.ChangeType & ChangeType.Undelete) == ChangeType.Undelete)
                UndeleteFile(changeset, change);
        }
        private void ProcessFolderChange(Changeset changeset, Change change)
        {
            // Rename folder.
            if ((change.ChangeType & ChangeType.Rename) == ChangeType.Rename)
                RenameFolder(changeset, change);

            // Branch folder.
            else if ((change.ChangeType & ChangeType.Branch) == ChangeType.Branch)
                AddFolder(changeset, change);

            // Add folder.
            else if ((change.ChangeType & ChangeType.Add) == ChangeType.Add)
                AddFolder(changeset, change);

            // Delete folder.
            else if ((change.ChangeType & ChangeType.Delete) == ChangeType.Delete)
                DeleteFolder(changeset, change);

            // Undelete folder.
            else if ((change.ChangeType & ChangeType.Undelete) == ChangeType.Undelete)
                UndeleteFolder(changeset, change);
        }

        private void AddFile(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            DownloadFile(change, itemPath);

            this._itemPaths[change.Item.ItemId] = itemPath;

            if (this.FileAdded != null)
                this.FileAdded(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void DeleteFile(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            //File.Delete(itemPath);
            //this._itemPaths.Remove(change.Item.ItemId);

            if (this.FileDeleted != null)
                this.FileDeleted(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void EditFile(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            DownloadFile(change, itemPath);

            if (this.FileEdited != null)
                this.FileEdited(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void RenameFile(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);

            string oldPath = this._itemPaths[change.Item.ItemId];
            string newPath = itemPath;

            this._itemPaths[change.Item.ItemId] = newPath;

            if (this.FileRenamed != null)
                this.FileRenamed(changeset.ChangesetId, oldPath, newPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void BranchFile(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            DownloadFile(change, itemPath);

            string oldPath = this._itemPaths[change.Item.ItemId];
            string newPath = itemPath;

            this._itemPaths[change.Item.ItemId] = itemPath;

            if (this.FileBranched != null)
                this.FileBranched(changeset.ChangesetId, oldPath, newPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void UndeleteFile(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            DownloadFile(change, itemPath);

            this._itemPaths[change.Item.ItemId] = itemPath;

            if (this.FileUndeleted != null)
                this.FileUndeleted(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        
        }

        private void AddFolder(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            Directory.CreateDirectory(itemPath);

            this._itemPaths[change.Item.ItemId] = itemPath;

            if (this.FolderAdded != null)
                this.FolderAdded(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void DeleteFolder(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);

            if (this.FolderDeleted != null)
                this.FolderDeleted(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void BranchFolder(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);

            string oldPath = this._itemPaths[change.Item.ItemId];
            string newPath = itemPath;

            this._itemPaths[change.Item.ItemId] = itemPath;

            if (this.FolderBranched != null)
                this.FolderBranched(changeset.ChangesetId, oldPath, newPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void RenameFolder(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);

            string oldPath = this._itemPaths[change.Item.ItemId];
            string newPath = itemPath;

            this._itemPaths[change.Item.ItemId] = newPath;

            if (this.FolderRenamed != null)
                this.FolderRenamed(changeset.ChangesetId, oldPath, newPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }
        private void UndeleteFolder(Changeset changeset, Change change)
        {
            string itemPath = GetItemPath(change);
            Directory.CreateDirectory(itemPath);

            this._itemPaths[change.Item.ItemId] = itemPath;

            if (this.FolderUndeleted != null)
                this.FolderUndeleted(changeset.ChangesetId, itemPath, changeset.Committer, changeset.Comment, changeset.CreationDate);
        }

        private string GetItemPath(Change change)
        {
            return this._localPath + "" + change.Item.ServerItem.Replace(this._remotePath, "");
        }
        private void DownloadFile(Change change, string path)
        {
            File.Delete(path);
            change.Item.DownloadFile(path);
        }

        #endregion
    }
}
