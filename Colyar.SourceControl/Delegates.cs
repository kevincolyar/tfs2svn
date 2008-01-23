using System;

namespace Colyar.SourceControl
{
    #region Public Delegates

    public delegate void ChangesetHandler(int changeset, string committer, string comment, DateTime date);
    public delegate void SinglePathHandler(int changeset, string path, string committer, string comment, DateTime date);
    public delegate void DualPathHandler(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date);
    public delegate void ChangesetsFoundHandler(int numberOfChangesetsFound);
    public delegate void SvnAdminEventHandler(string message);

    #endregion
}
