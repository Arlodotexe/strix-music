using System;
using System.Collections.Generic;
using System.Text;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
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

        /// <inheritdoc/>
        public override IEnumerable<Type> SettingsKeysTypes => typeof(OneDriveCoreSettingsKeys).IntoList();
    }
}
