using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Services.Localization;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace StrixMusic.Sdk.Uno.Services.Localization
{
    /// <summary>
    /// A Service for getting localized strings from <see cref="ResourceLoader"/> providers.
    /// </summary>
    public sealed class LocalizationResourceLoader : ILocalizationService
    {
        private readonly Dictionary<string, ResourceLoader> _providers = new Dictionary<string, ResourceLoader>();

        /// <inheritdoc/>
        public string this[string provider, string key] => _providers.ContainsKey(provider) ? _providers[provider].GetString(key) : "ResourceError";

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
        /// Registers a new <see cref="ResourceLoader"/> as a provider.
        /// </summary>
        /// <param name="path">The path of the provider.</param>
        public void RegisterProvider(string path)
        {
            if (_providers.ContainsKey(path))
                return;

            var loader = ResourceLoader.GetForCurrentView(path);
            _providers.Add(path, loader);
        }
    }
}
