using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            var lastItemCount = trackCollection.Tracks.Count;

            while (trackCollection.Tracks.Count < trackCollection.TotalTrackCount)
            {
                await trackCollection.PopulateMoreTracksAsync(trackCollection.TotalTrackCount);

                // nothing was returned
                if (lastItemCount == trackCollection.Tracks.Count)
                {
                    Debug.WriteLine($"Warning: Collection init for {nameof(trackCollection)} {trackCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = trackCollection.Tracks.Count;
            }
        }

        /// <summary>
        /// Initialize a album collection view model.
        /// </summary>
        /// <param name="albumCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AlbumCollection(IAlbumCollectionViewModel albumCollection)
        {
            var lastItemCount = albumCollection.Albums.Count;

            while (albumCollection.Albums.Count < albumCollection.TotalAlbumItemsCount)
            {
                await albumCollection.PopulateMoreAlbumsAsync(albumCollection.TotalAlbumItemsCount);

                // nothing was returned
                if (lastItemCount == albumCollection.Albums.Count)
                {
                    Debug.WriteLine($"Warning: Collection init for {nameof(albumCollection)} {albumCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = albumCollection.Albums.Count;
            }
        }

        /// <summary>
        /// Initialize a artist collection view model.
        /// </summary>
        /// <param name="artistCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ArtistCollection(IArtistCollectionViewModel artistCollection)
        {
            var lastItemCount = artistCollection.Artists.Count;

            while (artistCollection.Artists.Count < artistCollection.TotalArtistItemsCount)
            {
                await artistCollection.PopulateMoreArtistsAsync(artistCollection.TotalArtistItemsCount);

                // nothing was returned
                if (lastItemCount == artistCollection.Artists.Count)
                {
                    Debug.WriteLine($"Warning: Collection init for {nameof(artistCollection)} {artistCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = artistCollection.Artists.Count;
            }
        }

        /// <summary>
        /// Initialize a playlists collection view model.
        /// </summary>
        /// <param name="playlistCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task PlaylistCollection(IPlaylistCollectionViewModel playlistCollection)
        {
            var lastItemCount = playlistCollection.Playlists.Count;

            while (playlistCollection.Playlists.Count < playlistCollection.TotalPlaylistItemsCount)
            {
                await playlistCollection.PopulateMorePlaylistsAsync(playlistCollection.TotalPlaylistItemsCount);

                // nothing was returned
                if (lastItemCount == playlistCollection.Playlists.Count)
                {
                    Debug.WriteLine($"Warning: Collection init for {nameof(playlistCollection)} {playlistCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = playlistCollection.Playlists.Count;
            }
        }

        /// <summary>
        /// Initialize a genre collection view model.
        /// </summary>
        /// <param name="genreCollectionViewModel">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task GenreCollection(IGenreCollectionViewModel genreCollectionViewModel)
        {
            var lastItemCount = genreCollectionViewModel.Genres.Count;

            while (genreCollectionViewModel.Genres.Count < genreCollectionViewModel.TotalGenreCount)
            {
                await genreCollectionViewModel.PopulateMoreGenresAsync(genreCollectionViewModel.TotalGenreCount);

                // nothing was returned
                if (lastItemCount == genreCollectionViewModel.Genres.Count)
                {
                    Debug.WriteLine($"Warning: Collection init for {nameof(genreCollectionViewModel)} {genreCollectionViewModel.Genres} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = genreCollectionViewModel.Genres.Count;
            }
        }

        /// <summary>
        /// Initialize a image collection view model.
        /// </summary>
        /// <param name="imageCollectionViewModel">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ImageCollection(IImageCollectionViewModel imageCollectionViewModel)
        {
            var lastItemCount = imageCollectionViewModel.Images.Count;

            while (imageCollectionViewModel.Images.Count < imageCollectionViewModel.TotalImageCount)
            {
                await imageCollectionViewModel.PopulateMoreImagesAsync(imageCollectionViewModel.TotalImageCount);

                // nothing was returned
                if (lastItemCount == imageCollectionViewModel.Images.Count)
                {
                    Debug.WriteLine($"Warning: Collection init for {nameof(imageCollectionViewModel)} {imageCollectionViewModel.Images} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = imageCollectionViewModel.Images.Count;
            }
        }
    }
}