using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace StrixMusic.Sdk.Services.Localization
{
    /// <summary>
    /// A Service for getting localized strings from <see cref="ResourceManager"/> providers.
    /// </summary>
    public class LocalizationManagerService : ILocalizationService
    {
        private Dictionary<string, ResourceManager> _providers = new Dictionary<string, ResourceManager>();

        /// <inheritdoc/>
        public string this[string provider, string key]
        {
            get
            {
                if (_providers.ContainsKey(provider))
                {
                    return _providers[provider].GetString(key);
                }

                return "ResourceError";
            }
        }

        /// <summary>
        /// Registers a new provider.
        /// </summary>
        /// <param name="name">The name of the provider.</param>
        /// <param name="assembly">The assembly of the provider.</param>
        public void RegisterProvider(string name, Assembly assembly)
        {
            if (_providers.ContainsKey(name))
            {
                return;
            }

            ResourceManager manager = new ResourceManager(name, assembly);
            _providers.Add(name, manager);
        }
    }
}
