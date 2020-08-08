// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;
using System.Threading.Tasks;

namespace StrixMusic.Services.Settings
{
    /// <summary>
    /// Manages interactions with app settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Assigns a value to a settings key.
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key.</typeparam>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value to assign to the setting key.</param>
        /// <param name="overwrite">Indicates whether or not to overwrite the setting, if it already exists.</param>
        void SetValue<T>(string key, object? value, bool overwrite = true);

        /// <summary>
        /// Reads a value from the current <see cref="IServiceProvider"/> instance and returns its casting in the right type.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <param name="fallback">If true, the method returns the default <typeparamref name="T"/> value in case of failure.</param>
        /// <returns>A <see cref="Task{T}"/> that represents the value for the storage <paramref name="key"/>.</returns>
        Task<T> GetValue<T>(string key, bool fallback = false);

        /// <summary>
        /// Assigns a value to a settings key.
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key.</typeparam>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value to assign to the setting key.</param>
        /// <param name="identifier">Identifies a unique version of this settings key.</param>
        /// <param name="overwrite">Indicates whether or not to overwrite the setting, if it already exists.</param>
        void SetValue<T>(string key, object? value, Type identifier, bool overwrite = true);

        /// <summary>
        /// Reads a value from the current <see cref="IServiceProvider"/> instance and returns its casting in the right type.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <param name="identifier">Identifies a unique version of this settings key.</param>
        /// <param name="fallback">If true, the method returns the default <typeparamref name="T"/> value in case of failure.</param>
        /// <returns>A <see cref="Task{T}"/> that represents the value for the storage <paramref name="key"/> with the <paramref name="identifier"/>.</returns>
        Task<T> GetValue<T>(string key, Type identifier, bool fallback = false);

        /// <summary>
        /// Deletes all the existing setting values.
        /// </summary>
        void ResetToDefaults();

        /// <summary>
        /// Deletes all the existing setting values.
        /// </summary>
        /// <param name="identifier">Identifies a unique settings store.</param>
        void ResetToDefaults(Type identifier);

        /// <summary>
        /// Fires when a setting has changed.
        /// </summary>
        event EventHandler<SettingChangedEventArgs>? SettingChanged;
    }
}
