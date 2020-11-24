using OwlCore.Collections;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="ICoreBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IAppCore : ICoreBase, ISdkMember<ICore>
    {
        /// <summary>
        /// The available devices.
        /// </summary>
        public SynchronizedObservableCollection<IDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ILibrary Library { get; }

        /// <summary>
        /// A list of pinned playable items.
        /// </summary>
        public IPlayableCollectionGroup Pins { get; }

        /// <summary>
        /// Gets the recently played items for this .
        /// </summary>
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public IDiscoverables Discoverables { get; }
    }
}