using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Configuration;

namespace Colyar.SourceControl.TeamFoundationServer
{
    public abstract class TfsClientProviderBase : ProviderBase
    {
        public abstract event ChangesetHandler BeginChangeSet;
        public abstract event ChangesetHandler EndChangeSet;
        public abstract event SinglePathHandler FileAdded;
        public abstract event SinglePathHandler FileEdited;
        public abstract event SinglePathHandler FileDeleted;
        public abstract event SinglePathHandler FileUndeleted;
        public abstract event SinglePathHandler FileBranched;
        public abstract event DualPathHandler FileRenamed;
        public abstract event SinglePathHandler FolderAdded;
        public abstract event SinglePathHandler FolderDeleted;
        public abstract event SinglePathHandler FolderUndeleted;
        public abstract event SinglePathHandler FolderBranched;
        public abstract event DualPathHandler FolderRenamed;
        public abstract event ChangesetsFoundHandler ChangeSetsFound;
        public abstract void Connect(string serverUri, string remotePath, string localPath, int fromChangeset, string tfsUsername, string tfsPassword, string tfsDomain);
        public abstract void ProcessAllChangeSets();
    }

    public class TfsClientProviderCollection : ProviderCollection
    {
        public new TfsClientProviderBase this[string name]
        {
            get { return (TfsClientProviderBase)base[name]; }
        }

        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is TfsClientProviderBase))
                throw new ArgumentException("Invalid provider type", "provider");

            base.Add(provider);
        }
    }

    public class TfsClientProviderSection : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }

        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider", DefaultValue = "OpenTF")]
        public string DefaultProvider
        {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }
    }
}
