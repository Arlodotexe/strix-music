// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using System;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// The instance of <see cref="ISettingsService"/> used by default
    /// <remarks>Not used by Cores. User-configurable settings go here.</remarks>
    /// </summary>
    public class DefaultSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettingsService"/> class.
        /// </summary>
        /// <param name="textStorageService">The text storage service to be used by this instance.</param>
        public DefaultSettingsService(ITextStorageService textStorageService)
            : base(textStorageService)
        {
        }

        /// <inheritdoc/>
        public override string Id => "Default";
    }
}
