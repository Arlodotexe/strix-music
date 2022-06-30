using System.Collections.Generic;
using OwlCore.AbstractStorage;
using OwlCore.Services;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Services;
using StrixMusic.Shells.Default;

namespace StrixMusic.Services
{
    /// <summary>
    /// A container for keys of all settings throughout the main app.
    /// </summary>
    public class AppSettings : SettingsBase
    {
        private static bool _isDebug =
#if DEBUG
            true;
#else
            false;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        /// <param name="folder">The folder where app settings are stored.</param>
        public AppSettings(IFolderData folder)
            : base(folder, AppSettingsSerializer.Singleton)
        {
        }

        /// <summary>
        /// Stored information about all core instances that the user has configured.
        /// </summary>
        public Dictionary<string, CoreMetadata> CoreInstanceRegistry
        {
            get => GetSetting(() => new Dictionary<string, CoreMetadata>());
            set => SetSetting(value);
        }

        /// <summary>
        /// The user's preferred ranking for each core, stored as the core's instance ID. Highest ranking first.
        /// </summary>
        public List<string> CoreRanking
        {
            get => GetSetting(() => new List<string>());
            set => SetSetting(value);
        }

        /// <summary>
        /// If true, the app will log information about the app while its running.
        /// </summary>
        public bool IsLoggingEnabled
        {
            get => GetSetting(() => _isDebug);
            set => SetSetting(value);
        }

        /// <inheritdoc cref="SiblingCollectionPlaybackPreferences"/>
        public SiblingCollectionPlaybackPreferences PlayCollectionBehavior
        {
            get => GetSetting(() => new SiblingCollectionPlaybackPreferences());
            set => SetSetting(value);
        }

        /// <summary>
        /// Stores the registry id of the user's preferred shell.
        /// </summary>
        public string PreferredShell
        {
            get => GetSetting(() => DefaultShell.Metadata.Id);
            set => SetSetting(value);
        }

        /// <summary>
        /// The registry id of the user's current fallback shell. Used to cover display sizes that the <see cref="PreferredShell"/> doesn't support. 
        /// </summary>
        public string FallbackShell
        {
            get => GetSetting(() => DefaultShell.Metadata.Id);
            set => SetSetting(value);
        }

        /// <summary>
        /// The user's preference for how items in a collection from multiple sources are sorted. 
        /// </summary>
        public MergedCollectionSorting MergedCollectionSorting
        {
            get => GetSetting(() => MergedCollectionSorting.Ranked);
            set => SetSetting(value);
        }
    }
}
