// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using StrixMusic.Services.StorageService;

namespace StrixMusic.Services.Settings
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
        public SettingsServiceBase()
        {
            _textStorageService = Ioc.Default.GetService<ITextStorageService>();
        }

        /// <summary>
        /// A service to store and retrieve settings throughout the app.
        /// </summary>
        /// <param name="textStorageService">The text storage service to be used by this instance.</param>
        public SettingsServiceBase(ITextStorageService textStorageService)
        {
            _textStorageService = textStorageService;
        }

        /// <inheritdoc/>
        public virtual void SetValue<T>(string key, object? value, bool overwrite = true)
        {
            SetValue<T>(key, value, Id, overwrite);
        }

        /// <inheritdoc/>
        public virtual Task<T> GetValue<T>(string key, bool fallback = true)
        {
            return GetValue<T>(key, Id, fallback);
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
        public virtual void SetValue<T>(string key, object? value, string identifier, bool overwrite = true)
        {
            // Serialize the value

            // Convert the value
            object serializable;
            if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));
                serializable = Convert.ChangeType(value, type);
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
                throw new ArgumentException($"Invalid setting of type {typeof(T)}", nameof(value));
            }

            var serialized = JsonConvert.SerializeObject(serializable);

            // It's fine
            Task.Run(async () =>
            {
                // Store the new value
                if (!await _textStorageService.FileExistsAsync(key))
                {
                    await _textStorageService.SetValueAsync(key, serialized, identifier);
                    SettingChanged?.Invoke(this, new SettingChangedEventArgs() { Key = key, Value = value });
                }
                else if (overwrite)
                {
                    await _textStorageService.SetValueAsync(key, serialized, identifier);
                    SettingChanged?.Invoke(this, new SettingChangedEventArgs() { Key = key, Value = value });
                }
            });
        }

        /// <inheritdoc/>
        public virtual async Task<T> GetValue<T>(string key, string identifier, bool fallback = false)
        {
            string result = await _textStorageService.GetValueAsync(key);

            T obj;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception)
            {
                return default!;
            }

            // Try to get the setting value
            if (!(obj is T))
            {
                if (fallback) return default!;
                throw new InvalidOperationException($"The setting {key} doesn't exist");
            }

            return obj!;
        }

        /// <summary>
        /// Identifies this settings instance.
        /// </summary>
        public abstract string Id { get; }

        /// <inheritdoc cref="ISettingsService.SettingChanged"/>
        public virtual event EventHandler<SettingChangedEventArgs>? SettingChanged;
    }
}
