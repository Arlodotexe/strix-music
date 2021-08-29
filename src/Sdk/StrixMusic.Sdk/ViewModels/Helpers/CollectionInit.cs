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
            // TODO Save/get to collection cache. All tracks collections should be cached to disk.
            if (trackCollection.Tracks.Count < trackCollection.TotalTracksCount)
            {
                var allTracks = await APIs.GetAllItemsAsync<ITrack>(trackCollection.TotalTracksCount, async offset => await trackCollection.GetTracksAsync(trackCollection.TotalTracksCount, offset));

                foreach (var track in allTracks)
                {
                    Guard.IsAssignableToType(track, typeof(MergedTrack), nameof(track));

                    trackCollection.Tracks.Add(new TrackViewModel(track));
                }
            }
        }

        /// <summary>
        /// Initialize a album collection view model.
        /// </summary>
        /// <param name="albumCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AlbumCollection(IAlbumCollectionViewModel albumCollection)
        {
            if (albumCollection.Albums.Count < albumCollection.TotalAlbumItemsCount)
            {
                var albumCollectionList = await APIs.GetAllItemsAsync<IAlbumCollectionItem>(albumCollection.TotalAlbumItemsCount, async offset => await albumCollection.GetAlbumItemsAsync(albumCollection.TotalAlbumItemsCount, offset));

                foreach (var item in albumCollectionList)
                {
                    switch (item)
                    {
                        case IAlbum album:
                            albumCollection.Albums.Add(new AlbumViewModel(album));
                            break;
                        case IAlbumCollection collection:
                            albumCollection.Albums.Add(new AlbumCollectionViewModel(collection));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize a artist collection view model.
        /// </summary>
        /// <param name="artistCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ArtistCollection(IArtistCollectionViewModel artistCollection)
        {
            if (artistCollection.Artists.Count < artistCollection.TotalArtistItemsCount)
            {
                var albumCollectionList = await APIs.GetAllItemsAsync<IArtistCollectionItem>(artistCollection.TotalArtistItemsCount, async offset => await artistCollection.GetArtistItemsAsync(artistCollection.TotalArtistItemsCount, offset));

                foreach (var item in albumCollectionList)
                {
                    switch (item)
                    {
                        case IArtist artist:
                            artistCollection.Artists.Add(new ArtistViewModel(artist));
                            break;
                        case IArtistCollection collection:
                            artistCollection.Artists.Add(new ArtistCollectionViewModel(collection));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize a playlists collection view model.
        /// </summary>
        /// <param name="playlistCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task PlaylistCollection(IPlaylistCollectionViewModel playlistCollection)
        {
            if (playlistCollection.Playlists.Count < playlistCollection.TotalPlaylistItemsCount)
            {
                var playlistCollectionList = await APIs.GetAllItemsAsync<IPlaylistCollectionItem>(playlistCollection.TotalPlaylistItemsCount, async offset => await playlistCollection.GetPlaylistItemsAsync(playlistCollection.TotalPlaylistItemsCount, offset));

                foreach (var item in playlistCollectionList)
                {
                    switch (item)
                    {
                        case IPlaylist playlist:
                            playlistCollection.Playlists.Add(new PlaylistViewModel(playlist));
                            break;
                        case IPlaylistCollection collection:
                            playlistCollection.Playlists.Add(new PlaylistCollectionViewModel(collection));
                            break;
                    }
                }
            }
        }
    }
}