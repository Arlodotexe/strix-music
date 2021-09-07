using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Helpers for initializing collection view models.
    /// </summary>
    public static class CollectionInit
    {
        /// <summary>
        /// Initialize a track collection view model.
        /// </summary>
        /// <param name="trackCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task TrackCollection(ITrackCollectionViewModel trackCollection)
        {
            while (trackCollection.Tracks.Count < trackCollection.TotalTrackCount)
            {
                await trackCollection.PopulateMoreTracksAsync(trackCollection.TotalTrackCount);
            }
        }

        /// <summary>
        /// Initialize a album collection view model.
        /// </summary>
        /// <param name="albumCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AlbumCollection(IAlbumCollectionViewModel albumCollection)
        {
            while (albumCollection.Albums.Count < albumCollection.TotalAlbumItemsCount)
            {
                await albumCollection.PopulateMoreAlbumsAsync(albumCollection.TotalAlbumItemsCount);
            }
        }

        /// <summary>
        /// Initialize a artist collection view model.
        /// </summary>
        /// <param name="artistCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ArtistCollection(IArtistCollectionViewModel artistCollection)
        {
            while (artistCollection.Artists.Count < artistCollection.TotalArtistItemsCount)
            {
                await artistCollection.PopulateMoreArtistsAsync(artistCollection.TotalArtistItemsCount);
            }
        }

        /// <summary>
        /// Initialize a playlists collection view model.
        /// </summary>
        /// <param name="playlistCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task PlaylistCollection(IPlaylistCollectionViewModel playlistCollection)
        {
            while (playlistCollection.Playlists.Count < playlistCollection.TotalPlaylistItemsCount)
            {
                await playlistCollection.PopulateMorePlaylistsAsync(playlistCollection.TotalPlaylistItemsCount);
            }
        }
    }
}