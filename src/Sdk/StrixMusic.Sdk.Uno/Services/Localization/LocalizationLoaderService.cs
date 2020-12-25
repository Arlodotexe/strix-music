using StrixMusic.Sdk.Services.Localization;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace StrixMusic.Sdk.Uno.Services.Localization
{
    /// <summary>
    /// A Service for getting localized strings from <see cref="ResourceLoader"/> providers.
    /// </summary>
    public class LocalizationLoaderService : ILocalizationService
    {
        private readonly Dictionary<string, ResourceLoader> _providers = new Dictionary<string, ResourceLoader>();

        /// <inheritdoc/>
        public string this[string provider, string key] => _providers.ContainsKey(provider) ? _providers[provider].GetString(key) : "ResourceError";

        /// <summary>
        /// Registers a new provider.
        /// </summary>
        /// <param name="path">The path of the provider.</param>
        public void RegisterProvider(string path)
        {
            if (_providers.ContainsKey(path))
            {
                return;
            }

            ResourceLoader loader = ResourceLoader.GetForCurrentView(path);
            _providers.Add(path, loader);
        }
    }
}
