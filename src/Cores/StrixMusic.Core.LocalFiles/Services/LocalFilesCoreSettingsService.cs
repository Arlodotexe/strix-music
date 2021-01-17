using System;
using System.Collections.Generic;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles.Services
{
    /// <inheritdoc />
    internal class LocalFilesCoreSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocalFilesCoreSettingsService"/>.
        /// </summary>
        /// <param name="instanceId">The ID of the current <see cref="LocalFilesCore"/> instance.</param>
        public LocalFilesCoreSettingsService(string instanceId)
        {
            Id = $"{nameof(LocalFilesCore)}.{instanceId}";
        }

        /// <inheritdoc />
        public override string Id { get; }

        /// <inheritdoc/>
        public override IEnumerable<Type> SettingsKeysTypes => typeof(LocalFilesCoreSettingsKeys).IntoList();
    }
}