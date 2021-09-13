using System;
using System.Collections.Generic;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.Files.Services
{
    /// <inheritdoc />
    internal class FilesCoreSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="FilesCoreSettingsService"/>.
        /// </summary>
        /// <param name="instanceId">The ID of the current <see cref="FilesCore"/> instance.</param>
        public FilesCoreSettingsService(string instanceId)
        {
            Id = instanceId;
        }

        /// <inheritdoc />
        public override string Id { get; }

        /// <inheritdoc/>
        public override IEnumerable<Type> SettingsKeysTypes => typeof(FilesCoreSettingsKeys).IntoList();
    }
}