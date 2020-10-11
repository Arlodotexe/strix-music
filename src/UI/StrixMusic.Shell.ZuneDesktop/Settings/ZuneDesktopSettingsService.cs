using System;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Shell.ZuneDesktop.Settings
{
    /// <summary>
    /// The instance of <see cref="ISettingsService"/> used by the ZuneShell
    /// <remarks>Not used by Cores. User-configurable settings go here.</remarks>
    /// </summary>
    public class ZuneDesktopSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettingsService"/> class.
        /// </summary>
        /// <param name="textStorageService">The text storage service to be used by this instance.</param>
        public ZuneDesktopSettingsService(ITextStorageService textStorageService)
            : base(textStorageService)
        {
        }

        /// <inheritdoc/>
        public override string Id => "ZuneDesktop";

        /// <inheritdoc/>
        public override Type SettingsKeysType => typeof(ZuneDesktopSettingsKeys);
    }
}
