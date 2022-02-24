using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <inheritdoc />
    public class OneDriveCoreSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="OneDriveCoreSettingsService"/>.
        /// </summary>
        /// <param name="instanceId">The ID of the current <see cref="OneDriveCore"/> instance.</param>
        public OneDriveCoreSettingsService(string instanceId)
        {
            Id = instanceId;
        }

        /// <inheritdoc />
        public override string Id { get; }

        /// <summary>
        /// Resets all settings back to their default values.
        /// </summary>
        public async Task ResetAllAsync()
        {
            await SetValue<string>(nameof(OneDriveCoreSettingsKeys.SelectedFolderId), OneDriveCoreSettingsKeys.SelectedFolderId);
        }

        /// <inheritdoc/>
        public override IEnumerable<SettingsKeysBase> SettingsKeys => new OneDriveCoreSettingsKeys().IntoList();
    }
}
