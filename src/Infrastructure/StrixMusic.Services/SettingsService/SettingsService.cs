// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using OwlCore.ArchTools;
using StrixMusic.Services.SettingsStorage;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StrixMusic.Services
{
    /// <summary>
    /// A simple <see langword="class"/> that handles the local app settings
    /// </summary>
    public class SettingsService : ISettingsService
    {
        /// <summary>
        /// A service to store and retrieve settings throughout the app
        /// </summary>
        /// <param name="identifier">Indentifies this source of the settings</param>
        /// <param name="keysClassType"></param>
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

        public void ResetToDefaults(Type identifier)
        {
            foreach (var prop in identifier.GetProperties())
            {
                SetValue<object>(prop.Name, null);
            }
        }

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
                // We don't care if it's null
                serializable = value!;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                serializable = Unsafe.As<object?, DateTime>(ref value).ToBinary();
            }
            else
                throw new ArgumentException($"Invalid setting of type {typeof(T)}", nameof(value));

            // It's fine
            Task.Run(async () =>
            {
                // Store the new value
                if (!await _settingsStorageService.Value.FileExistsAsync(key))
                {
                    await _settingsStorageService.Value.SetValueAsync(key, serializable, nameof(identifier));
                }
                else if (overwrite)
                {
                    await _settingsStorageService.Value.SetValueAsync(key, serializable, nameof(identifier));
                }
            });
        }

        public async Task<T> GetValue<T>(string key, Type identifier, bool fallback = false)
        {
            object? obj = await _settingsStorageService.Value.GetValueAsync<T>(key);

            // Try to get the setting value
            if (obj is T)
            {
                if (fallback) return default!;
                throw new InvalidOperationException($"The setting {key} doesn't exist");
            }

            // Cast and return the retrieved setting
            if (typeof(T) == typeof(DateTime))
                obj = DateTime.FromBinary((long)obj!);

            return (T)obj!;
        }

    }

}
