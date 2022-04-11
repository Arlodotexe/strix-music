// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Helpers for initializing collection view models.
    /// </summary>
    public sealed class CollectionInit
    {
        private static SemaphoreSlim _imagesMutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initialize a track collection view model.
        /// </summary>
        /// <param name="trackCollection">The collection to initialize.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task TrackCollection(ITrackCollectionViewModel trackCollection, CancellationToken cancellationToken)
        {
            var lastItemCount = trackCollection.Tracks.Count;

            while (trackCollection.Tracks.Count < trackCollection.TotalTrackCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await trackCollection.PopulateMoreTracksAsync(trackCollection.TotalTrackCount);

                // nothing was returned
                if (lastItemCount == trackCollection.Tracks.Count)
                {
                    var logger = Ioc.Default.GetRequiredService<ILogger<CollectionInit>>();

                    logger.LogError($"Collection init for {nameof(trackCollection)} {trackCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = trackCollection.Tracks.Count;
            }
        }

        /// <summary>
        /// Initialize a album collection view model.
        /// </summary>
        /// <param name="albumCollection">The collection to initialize.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AlbumCollection(IAlbumCollectionViewModel albumCollection, CancellationToken cancellationToken)
        {
            var lastItemCount = albumCollection.Albums.Count;

            while (albumCollection.Albums.Count < albumCollection.TotalAlbumItemsCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await albumCollection.PopulateMoreAlbumsAsync(albumCollection.TotalAlbumItemsCount);

                // nothing was returned
                if (lastItemCount == albumCollection.Albums.Count)
                {
                    var logger = Ioc.Default.GetRequiredService<ILogger<CollectionInit>>();

                    logger.LogError($"Collection init for {nameof(albumCollection)} {albumCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = albumCollection.Albums.Count;
            }
        }

        /// <summary>
        /// Initialize a artist collection view model.
        /// </summary>
        /// <param name="artistCollection">The collection to initialize.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ArtistCollection(IArtistCollectionViewModel artistCollection, CancellationToken cancellationToken)
        {
            var lastItemCount = artistCollection.Artists.Count;

            while (artistCollection.Artists.Count < artistCollection.TotalArtistItemsCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await artistCollection.PopulateMoreArtistsAsync(artistCollection.TotalArtistItemsCount);

                // nothing was returned
                if (lastItemCount == artistCollection.Artists.Count)
                {
                    var logger = Ioc.Default.GetRequiredService<ILogger<CollectionInit>>();

                    logger.LogError($"Warning: Collection init for {nameof(artistCollection)} {artistCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = artistCollection.Artists.Count;
            }
        }

        /// <summary>
        /// Initialize a playlists collection view model.
        /// </summary>
        /// <param name="playlistCollection">The collection to initialize.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task PlaylistCollection(IPlaylistCollectionViewModel playlistCollection, CancellationToken cancellationToken)
        {
            var lastItemCount = playlistCollection.Playlists.Count;

            while (playlistCollection.Playlists.Count < playlistCollection.TotalPlaylistItemsCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await playlistCollection.PopulateMorePlaylistsAsync(playlistCollection.TotalPlaylistItemsCount);

                // nothing was returned
                if (lastItemCount == playlistCollection.Playlists.Count)
                {
                    var logger = Ioc.Default.GetRequiredService<ILogger<CollectionInit>>();

                    logger.LogError($"Collection init for {nameof(playlistCollection)} {playlistCollection.Name} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = playlistCollection.Playlists.Count;
            }
        }

        /// <summary>
        /// Initialize a genre collection view model.
        /// </summary>
        /// <param name="genreCollectionViewModel">The collection to initialize.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task GenreCollection(IGenreCollectionViewModel genreCollectionViewModel, CancellationToken cancellationToken)
        {
            var lastItemCount = genreCollectionViewModel.Genres.Count;

            while (genreCollectionViewModel.Genres.Count < genreCollectionViewModel.TotalGenreCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await genreCollectionViewModel.PopulateMoreGenresAsync(genreCollectionViewModel.TotalGenreCount);

                // nothing was returned
                if (lastItemCount == genreCollectionViewModel.Genres.Count)
                {
                    var logger = Ioc.Default.GetRequiredService<ILogger<CollectionInit>>();

                    logger.LogError($"Collection init for {nameof(genreCollectionViewModel)} {genreCollectionViewModel.Genres} failed. Not all items were returned.");
                    return;
                }

                lastItemCount = genreCollectionViewModel.Genres.Count;
            }
        }

        /// <summary>
        /// Initialize a image collection view model.
        /// </summary>
        /// <param name="imageCollectionViewModel">The collection to initialize.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ImageCollection(IImageCollectionViewModel imageCollectionViewModel, CancellationToken cancellationToken)
        {
            await _imagesMutex.WaitAsync();

            if (imageCollectionViewModel.Images.Count == imageCollectionViewModel.TotalImageCount)
            {
                _imagesMutex.Release();
                return;
            }

            var lastItemCount = imageCollectionViewModel.Images.Count;

            while (imageCollectionViewModel.Images.Count < imageCollectionViewModel.TotalImageCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await imageCollectionViewModel.PopulateMoreImagesAsync(imageCollectionViewModel.TotalImageCount);

                // nothing was returned
                if (lastItemCount == imageCollectionViewModel.Images.Count)
                {
                    var logger = Ioc.Default.GetRequiredService<ILogger<CollectionInit>>();

                    logger.LogError($"Collection init for {nameof(imageCollectionViewModel)} {imageCollectionViewModel.Images} failed. Not all items were returned.");

                    _imagesMutex.Release();
                    return;
                }

                lastItemCount = imageCollectionViewModel.Images.Count;
            }

            _imagesMutex.Release();
        }
    }
}
