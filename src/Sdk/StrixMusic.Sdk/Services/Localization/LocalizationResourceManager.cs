using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace StrixMusic.Sdk.Services.Localization
{
    /// <summary>
    /// A service for getting localized strings from <see cref="ResourceManager"/> providers in a .NET Standard project.
    /// </summary>
    public class LocalizationResourceManager : ILocalizationService
    {
        private readonly Dictionary<string, ResourceManager> _providers = new Dictionary<string, ResourceManager>();

        /// <inheritdoc/>
        public string this[string provider, string key] => _providers.ContainsKey(provider) ? _providers[provider].GetString(key, CultureInfo.CurrentUICulture) : "ResourceError";

        /// <summary>
        /// Registers a new provider.
        /// </summary>
        /// <param name="name">The name of the provider.</param>
        /// <param name="assembly">The assembly of the provider.</param>
        public void RegisterProvider(string name, Assembly assembly)
        {
            if (_providers.ContainsKey(name))
                return;

            var manager = new ResourceManager(name, assembly);
            _providers.Add(name, manager);
        }
    }
}
