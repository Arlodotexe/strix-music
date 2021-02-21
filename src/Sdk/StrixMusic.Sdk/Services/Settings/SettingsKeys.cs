using System.Collections.Generic;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Services.Settings.Containers;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the main app.
    /// </summary>
    /// <remarks>
    /// The StrixMusic.Sdk contains the keys that don't return anything UI-dependent, while StrixMusic.Sdk.Uno contains a partial that has the UI-dependent keys.
    /// </remarks>
    public static partial class SettingsKeys
    {
        /// <summary>
        /// Stores the display name of the user's preferred shell.
        /// </summary>
        public static readonly string PreferredShell = "Default";

        /// <summary>
        /// Stored assembly information about all available cores.
        /// </summary>
        public static readonly IReadOnlyList<CoreAssemblyInfo> CoreRegistry = new List<CoreAssemblyInfo>();

        /// <summary>
        /// Stored assembly information about all cores that the user has configured.
        /// </summary>
        public static readonly Dictionary<string, CoreAssemblyInfo> ConfiguredCores = new Dictionary<string, CoreAssemblyInfo>();

        /// <summary>
        /// The user's preferred ranking for each core, stored as the core's instance ID. Highest ranking first.
        /// </summary>
        public static readonly IReadOnlyList<string> CoreRanking = new List<string>();

        /// <summary>
        /// The user's preference for how items in a collection from multiple sources are sorted. 
        /// </summary>
        public static readonly MergedCollectionSorting MergedCollectionSorting = MergedCollectionSorting.Ranked;

        /// <inheritdoc cref="SiblingCollectionPlaybackPreferences"/>
        public static readonly SiblingCollectionPlaybackPreferences PlayCollectionBehavior = new SiblingCollectionPlaybackPreferences();
    }
}
