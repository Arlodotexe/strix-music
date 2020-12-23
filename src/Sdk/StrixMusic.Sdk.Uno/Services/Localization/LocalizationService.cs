using StrixMusic.Sdk.Services.Localization;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace StrixMusic.Sdk.Uno.Services.Localization
{
    /// <summary>
    /// A Service for getting localized strings from <see cref="ResourceLoader"/> providers.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, ResourceLoader> _providers = new Dictionary<string, ResourceLoader>();

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

        /// <inheritdoc/>
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
