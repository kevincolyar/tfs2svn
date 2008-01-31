using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Configuration.Provider;

namespace Colyar.SourceControl.TeamFoundationServer
{
    public class TfsClient
    {
        private static TfsClientProviderBase _provider = null;
        private static TfsClientProviderCollection _providers = null;
        private static object _lock = new object();

        public static TfsClientProviderBase Provider
        {
            get 
            {
                // Make sure a provider is loaded
                LoadProviders();

                return _provider; 
            }
        }

        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <imageService> section
                        TfsClientProviderSection section = (TfsClientProviderSection)
                            ConfigurationManager.GetSection("tfsClientProvider");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new TfsClientProviderCollection();

                        ProvidersHelper.InstantiateProviders
                            (section.Providers, _providers, typeof(TfsClientProviderBase));
                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                            throw new ProviderException("Unable to load default TfsClientProvider");
                    }
                }
            }
        }
    }
}
