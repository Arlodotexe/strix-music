// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the main app.
    /// </summary>
    /// <remarks>
    /// The StrixMusic.Sdk contains the keys that don't return anything UI-dependent, while StrixMusic.Sdk.Uno contains a partial that has the UI-dependent keys.
    /// </remarks>
    public sealed class SettingsKeys : SettingsKeysBase
    {
        /// <summary>
        /// Stored information about all core instances that the user has configured.
        /// </summary>
        public static Dictionary<string, CoreMetadata> CoreInstanceRegistry => new Dictionary<string, CoreMetadata>();

        /// <summary>
        /// The user's preferred ranking for each core, stored as the core's instance ID. Highest ranking first.
        /// </summary>
        public static IReadOnlyList<string> CoreRanking => new List<string>();

        /// <summary>
        /// If true, the app will log information about the app while its running.
        /// </summary>
#if DEBUG
        public static bool IsLoggingEnabled => true;
#else
        public static bool IsLoggingEnabled => false;
#endif

        /// <summary>
        /// The user's preference for how items in a collection from multiple sources are sorted. 
        /// </summary>
        public static MergedCollectionSorting MergedCollectionSorting => MergedCollectionSorting.Ranked;

        /// <inheritdoc cref="SiblingCollectionPlaybackPreferences"/>
        public static SiblingCollectionPlaybackPreferences PlayCollectionBehavior => new SiblingCollectionPlaybackPreferences();

        /// <inheritdoc/>
        public override object GetDefaultValue(string settingKey)
        {
            return settingKey switch
            {
                nameof(CoreInstanceRegistry) => CoreInstanceRegistry,
                nameof(CoreRanking) => CoreRanking,
                nameof(MergedCollectionSorting) => MergedCollectionSorting,
                nameof(IsLoggingEnabled) => IsLoggingEnabled,
                nameof(PlayCollectionBehavior) => PlayCollectionBehavior,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(settingKey)),
            };
        }
    }
}
