using System.Collections.Generic;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="ICoreBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IAppCore : ICoreBase, ISdkMember, IMerged<ICore>
    {
        /// <summary>
        /// The available devices.
        /// </summary>
        public IReadOnlyList<IDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ILibrary Library { get; }

        /// <summary>
        /// A list of pinned playable items.
        /// </summary>
        public IPlayableCollectionGroup? Pins { get; }

        /// <summary>
        /// Contains various search-related data and activities.
        /// </summary>
        public ISearch? Search { get; }

        /// <summary>
        /// Gets the recently played items for this .
        /// </summary>
        public IRecentlyPlayed? RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public IDiscoverables? Discoverables { get; }

        /// <summary>
        /// Raised when the contents of <see cref="Devices"/> is changed.
        /// </summary>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;
    }
}