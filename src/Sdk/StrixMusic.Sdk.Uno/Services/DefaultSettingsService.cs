// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <summary>
    /// The instance of <see cref="ISettingsService"/> used by default
    /// <remarks>Not used by Cores. User-configurable settings go here.</remarks>
    /// </summary>
    public sealed class DefaultSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettingsService"/> class.
        /// </summary>
        /// <param name="textStorageService">The text storage service to be used by this instance.</param>
        public DefaultSettingsService(ITextStorageService textStorageService)
            : base(textStorageService)
        {
        }

        /// <summary>
        /// Reads a setting value and returns it, casting to right type.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <param name="respectCurrentShell">If true, the returned value will be isolated to the current shell.</param>
        /// <returns>A <see cref="Task{T}"/> that returns the requested value.</returns>
        public async Task<T> GetValue<T>(string key, bool respectCurrentShell)
        {
            if (respectCurrentShell)
            {
                var currentShell = await GetValue<string>(nameof(SettingsKeysUI.PreferredShell));

                return await GetValue<T>(key, $"{Id}IsolatedTo{currentShell}");
            }

            return await GetValue<T>(key);
        }

        /// <summary>
        /// Sets a setting value.
        /// </summary>
        /// <typeparam name="T">The type of the object to store.</typeparam>
        /// <param name="key">The key associated to the requested object.</param>
        /// <param name="value">The data to store.</param>
        /// <param name="respectCurrentShell">If true, the returned value will be isolated to the current shell.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetValue<T>(string key, T value, bool respectCurrentShell)
        {
            if (respectCurrentShell)
            {
                var currentShell = await GetValue<string>(nameof(SettingsKeysUI.PreferredShell));

                await SetValue<T>(key, value, $"{Id}IsolatedTo{currentShell}");
                return;
            }

            await SetValue<T>(key, value);
        }

        /// <inheritdoc/>
        public override string Id => "Default";

        /// <inheritdoc/>
        public override IEnumerable<SettingsKeysBase> SettingsKeys { get; } = new List<SettingsKeysBase>
        {
            new SettingsKeys(),
            new SettingsKeysUI(),
        };
    }
}
