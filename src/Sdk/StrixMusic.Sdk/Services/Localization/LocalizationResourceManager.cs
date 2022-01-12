using StrixMusic.Sdk.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace StrixMusic.Sdk.Services.Localization
{
    /// <summary>
    /// A service for getting localized strings from <see cref="ResourceManager"/> providers in a .NET Standard project.
    /// </summary>
    public sealed class LocalizationResourceManager : ILocalizationService
    {
        private readonly Dictionary<string, ResourceManager> _providers = new Dictionary<string, ResourceManager>();

        /// <inheritdoc/>
        public string this[string provider, string key] => _providers.ContainsKey(provider) ? _providers[provider].GetString(key, CultureInfo.CurrentUICulture) : "ResourceError";

        /// <inheritdoc/>
        public string LocalizeIfNullOrEmpty(string value, string provider, string key)
        {
            if (string.IsNullOrEmpty(value))
                return this[provider, key];
            else
                return value;
        }

        /// <inheritdoc/>
        public string LocalizeIfNullOrEmpty<T>(string value, T sender)
        {
            return sender switch
            {
                IAlbum _ => LocalizeIfNullOrEmpty(value, Sdk.Helpers.Constants.Localization.MusicResource, "UnknownAlbum"),
                IArtist _ => LocalizeIfNullOrEmpty(value, Sdk.Helpers.Constants.Localization.MusicResource, "UnknownArtist"),

                // Default to unknown name
                _ => LocalizeIfNullOrEmpty(value, Sdk.Helpers.Constants.Localization.MusicResource, "UnknownName"),
            };
        }

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
