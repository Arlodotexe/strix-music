using System;
using System.Collections.Generic;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.External.Services
{
    /// <inheritdoc />
    internal class ExternalCoreSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExternalCoreSettingsService"/>.
        /// </summary>
        /// <param name="instanceId">The ID of the current <see cref="ExternalCore"/> instance.</param>
        public ExternalCoreSettingsService(string instanceId)
        {
            Id = instanceId;
        }

        /// <inheritdoc />
        public override string Id { get; }

        /// <inheritdoc/>
        public override IEnumerable<Type> SettingsKeysTypes => typeof(ExternalCoreSettingsKeys).IntoList();
    }
}