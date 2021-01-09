using System;
using System.Collections.Generic;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles.Services
{
    /// <inheritdoc />
    internal class LocalFilesCoreSettingsService : SettingsServiceBase
    {
        /// <inheritdoc />
        public override string Id { get; } = nameof(LocalFilesCoreSettingsService);

        /// <inheritdoc/>
        public override IEnumerable<Type> SettingsKeysTypes => typeof(LocalFilesCoreSettingsKeys).IntoList();
    }
}