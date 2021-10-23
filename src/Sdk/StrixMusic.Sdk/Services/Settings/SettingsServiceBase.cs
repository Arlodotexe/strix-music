﻿// Special thanks to Sergio Pedri for this class from Legere.
// Modified to work with "identifiers" (setting folders).
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// A <see langword="class"/> that handles the local app settings.
    /// </summary>
    public abstract class SettingsServiceBase : ISettingsService
    {
        private readonly ITextStorageService _textStorageService;

        /// <summary>
        /// A service to store and retrieve settings throughout the app.
        /// </summary>
        protected SettingsServiceBase()
        {
            _textStorageService = Ioc.Default.GetService<ITextStorageService>() ?? ThrowHelper.ThrowInvalidOperationException<ITextStorageService>();
        }

        /// <summary>
        /// A service to store and retrieve settings throughout the app.
        /// </summary>
        protected SettingsServiceBase(ITextStorageService textStorageService)
        {
            _textStorageService = textStorageService;
        }

        /// <inheritdoc/>
        public virtual async Task SetValue<T>(string key, object? value, bool overwrite = true)
        {
            await SetValue<T>(key, value, Id, overwrite);
        }

        /// <inheritdoc/>
        public virtual Task<T> GetValue<T>(string key)
        {
            return GetValue<T>(key, Id);
        }

        /// <inheritdoc/>
        public virtual Task ResetToDefaults()
        {
            return _textStorageService.RemoveAll();
        }

        /// <inheritdoc/>
        public virtual Task ResetToDefaults(string identifier)
        {
            return _textStorageService.RemoveByPathAsync(identifier);
        }

        /// <inheritdoc/>
        public virtual async Task SetValue<T>(string key, object? value, string identifier, bool overwrite = true)
        {
            // Serialize the value

            // Convert the value
            object serializable;
            if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));
                serializable = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                // It's fine to be null
                serializable = value!;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                serializable = Unsafe.As<object?, DateTime>(ref value).ToBinary();
            }
            else
            {
                serializable = (T)value!;
            }

            var serialized = JsonConvert.SerializeObject(serializable);

            // Store the new value
            if (!await _textStorageService.FileExistsAsync(key, identifier))
            {
                await _textStorageService.SetValueAsync(key, serialized, identifier);
                SettingChanged?.Invoke(this, new SettingChangedEventArgs() { Key = key, Value = value });
            }
            else if (overwrite)
            {
                await _textStorageService.SetValueAsync(key, serialized, identifier);
                SettingChanged?.Invoke(this, new SettingChangedEventArgs() { Key = key, Value = value });
            }
        }

        /// <inheritdoc/>
        public virtual async Task<T> GetValue<T>(string key, string identifier)
        {
            string? result = await _textStorageService.GetValueAsync(key, identifier);

            T? obj;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(result!);
            }
            catch (JsonException)
            {
                return GetDefaultSettingValue();
            }

            // Try to get the default setting value
            if (obj == null)
            {
                return GetDefaultSettingValue();
            }

            return obj;

            T GetDefaultSettingValue()
            {
                foreach (var settingsKey in SettingsKeys)
                {
                    // TODO: better check for if a key exists without reflection. Dictionary instead of method?
                    try
                    {
                        return (T) settingsKey.GetDefaultValue(key);
                    }
                    catch
                    {
                        // Ignore and try next key.
                    }
                }

                return ThrowHelper.ThrowArgumentOutOfRangeException<T>(nameof(key), $"{key} not found in the provided defaults.");
            }
        }

        /// <summary>
        /// Identifies this settings instance.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// The settings keys that are used with this service. Should implement GetDefaultValue().
        /// </summary>
        public abstract IEnumerable<SettingsKeysBase> SettingsKeys { get; }

        /// <inheritdoc cref="ISettingsService.SettingChanged"/>
        public event EventHandler<SettingChangedEventArgs>? SettingChanged;
    }
}
