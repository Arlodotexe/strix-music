using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A playlist collection.
    /// </summary>
    public interface IPlaylistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The playlists in the library.
        /// </summary>
        IReadOnlyList<IPlaylist> Playlists { get; }

        /// <summary>
        /// The total number of available <see cref="Playlists"/>.
        /// </summary>
        int TotalPlaylistCount { get; }

        /// <summary>
        /// Populates the <see cref="IPlaylist"/> in the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0);

        /// <summary>
        /// Fires when <see cref="Playlists"/> changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;
    }
}
