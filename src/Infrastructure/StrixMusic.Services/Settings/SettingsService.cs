// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OwlCore.ArchTools;
using StrixMusic.Services.StorageService;

namespace StrixMusic.Services.Settings
{
    /// <summary>
    /// A <see langword="class"/> that handles the local app settings.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// A service to store and retrieve settings throughout the app.
        /// </summary>
        public SettingsService()
        {
        }

        private LazyService<IStorageService> _settingsStorageService;

        /// <inheritdoc/>
        public void SetValue<T>(string key, object? value, bool overwrite = true)
        {
            SetValue<T>(key, value, typeof(SettingsService), overwrite);
        }

        /// <inheritdoc/>
        public Task<T> GetValue<T>(string key, bool fallback = true)
        {
            return GetValue<T>(key, typeof(SettingsService), fallback);
        }

        /// <inheritdoc/>
        public void ResetToDefaults()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ResetToDefaults(Type identifier)
        {
            foreach (var prop in identifier.GetProperties())
            {
                SetValue<object>(prop.Name, null);
            }
        }

        /// <inheritdoc/>
        public void SetValue<T>(string key, object? value, Type identifier, bool overwrite = true)
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
                if (!await _settingsStorageService.Value.FileExistsAsync(key))
                {
                    await _settingsStorageService.Value.SetValueAsync(key, serialized, nameof(identifier));
                    SettingChanged?.Invoke(this, new SettingChangedEventArgs() { Key = key, Value = value });
                }
                else if (overwrite)
                {
                    await _settingsStorageService.Value.SetValueAsync(key, serialized, nameof(identifier));
                    SettingChanged?.Invoke(this, new SettingChangedEventArgs() { Key = key, Value = value });
                }
            });
        }

        /// <inheritdoc/>
        public async Task<T> GetValue<T>(string key, Type identifier, bool fallback = false)
        {
            string result = await _settingsStorageService.Value.GetValueAsync(key);

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

        /// <inheritdoc cref="ISettingsService.SettingChanged"/>
        public event EventHandler<SettingChangedEventArgs>? SettingChanged;
    }
}
