using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.Settings;

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
        /// <returns></returns>
        public async Task ResetAllAsync()
        {
            await SetValue<string>(nameof(OneDriveCoreSettingsKeys.ClientId), OneDriveCoreSettingsKeys.ClientId);
            await SetValue<string>(nameof(OneDriveCoreSettingsKeys.SelectedFolderId), OneDriveCoreSettingsKeys.SelectedFolderId);
            await SetValue<string>(nameof(OneDriveCoreSettingsKeys.TenantId), OneDriveCoreSettingsKeys.TenantId);
        }

        /// <inheritdoc/>
        public override IEnumerable<Type> SettingsKeysTypes => typeof(OneDriveCoreSettingsKeys).IntoList();
    }
}
