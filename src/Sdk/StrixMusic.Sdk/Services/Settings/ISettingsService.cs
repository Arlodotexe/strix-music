// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Services
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
        /// <returns>A <see cref="Task{T}"/> that represents the value for the storage <paramref name="key"/>.</returns>
        Task SetValue<T>(string key, object? value, bool overwrite = true);

        /// <summary>
        /// Reads a setting value from the current <see cref="IServiceProvider"/> instance and returns its casting in the right type.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <returns>A <see cref="Task{T}"/> that represents the value for the storage <paramref name="key"/>.</returns>
        Task<T> GetValue<T>(string key);

        /// <summary>
        /// Assigns a value to a settings key.
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key.</typeparam>
        /// <param name="key">The key to check.</param>
        /// <param name="value">The value to assign to the setting key.</param>
        /// <param name="identifier">Identifies a unique version of this settings key.</param>
        /// <param name="overwrite">Indicates whether or not to overwrite the setting, if it already exists.</param>
        /// <returns>A <see cref="Task{T}"/> that represents the value for the storage <paramref name="key"/>.</returns>
        Task SetValue<T>(string key, object? value, string identifier, bool overwrite = true);

        /// <summary>
        /// Reads a value from the current <see cref="IServiceProvider"/> instance and returns its casting in the right type.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <param name="identifier">Identifies a unique version of this settings key.</param>
        /// <returns>A <see cref="Task{T}"/> that represents the value for the storage <paramref name="key"/> with the <paramref name="identifier"/>.</returns>
        Task<T> GetValue<T>(string key, string identifier);

        /// <summary>
        /// Deletes all the existing setting values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ResetToDefaults();

        /// <summary>
        /// Deletes all the existing setting values for a given <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">Identifies a unique settings store.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ResetToDefaults(string identifier);

        /// <summary>
        /// Fires when a setting has changed.
        /// </summary>
        event EventHandler<SettingChangedEventArgs>? SettingChanged;
    }
}
