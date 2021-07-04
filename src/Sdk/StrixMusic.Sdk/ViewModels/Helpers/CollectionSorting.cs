using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Comparers;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;

namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Helper to perform operations on a collection of playables.
    /// </summary>
    public static class CollectionSorting
    {
        /// <summary>
        /// Sorts the tracks.
        /// </summary>
        /// <param name="trackCollection">The collection to initialize.</param>
        /// <param name="trackSorting">Sort type of the collection.</param>
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortTracks(ObservableCollection<TrackViewModel> trackCollection, TrackSorting trackSorting, ObservableCollection<TrackViewModel> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = trackCollection;
            }

            bool isDescending = trackSorting.HasFlag(TrackSorting.Descending);

            // Remove the Descending flag from the enum.
            TrackSorting deflaggedSorting = trackSorting
                & ~TrackSorting.Descending;

            switch (deflaggedSorting)
            {
                case TrackSorting.Unordered:
                    trackCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case TrackSorting.Alphanumerical:
                    trackCollection.Sort(new NameComparer<TrackViewModel>(isDescending));
                    break;
                case TrackSorting.TrackNumber:
                    trackCollection.Sort(new TrackNumberComparer<TrackViewModel>(isDescending));
                    break;
                case TrackSorting.DateAdded:
                    trackCollection.Sort(new AddedAtComparer<TrackViewModel>(isDescending));
                    break;
                case TrackSorting.Duration:
                    trackCollection.Sort(new DurationComparer<TrackViewModel>(isDescending));
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"TrackSortType {trackSorting} is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Sorts the albums.
        /// </summary>
        /// <param name="albumCollection">The collection to sort.</param>
        /// <param name="albumSorting">Sort type of the collection.</param>
        /// <param name="originalOrder">The original order of the albumCollection.</param>
        public static void SortAlbums(ObservableCollection<IAlbumCollectionItem> albumCollection, AlbumSorting albumSorting, ObservableCollection<IAlbumCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = albumCollection;
            }

            bool isDescending = albumSorting.HasFlag(AlbumSorting.Descending);

            // Remove the Descending flag from the enum.
            AlbumSorting deflaggedSorting = albumSorting
                & ~AlbumSorting.Descending;

            switch (deflaggedSorting)
            {
                case AlbumSorting.Unordered:
                    albumCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case AlbumSorting.Descending:
                    albumCollection.Sort(new NameComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSorting.DateAdded:
                    albumCollection.Sort(new AddedAtComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSorting.Duration:
                    albumCollection.Sort(new DurationComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSorting.LastPlayed:
                    albumCollection.Sort(new LastPlayedComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSorting.DatePublished:
                    throw new NotImplementedException(); // IAlbumCollectionItem don't have DatePublished property. IAlbumBase has it. The potential solution will be is to move those IAlbumBase items to IAlbumCollectionItem.
                default:
                    ThrowHelper.ThrowNotSupportedException($"TrackSortType {albumSorting} is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Sorts the artists.
        /// </summary>
        /// <param name="artistCollection">The collection to sort.</param>
        /// <param name="artistSorting">Sort type of the collection.</param>
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortArtists(ObservableCollection<IArtistCollectionItem> artistCollection, ArtistSorting artistSorting, ObservableCollection<IArtistCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = artistCollection;
            }

            bool isDescending = artistSorting.HasFlag(ArtistSorting.Descending);

            // Remove the Descending flag from the enum.
            ArtistSorting deflaggedSorting = artistSorting
                & ~ArtistSorting.Descending;

            switch (deflaggedSorting)
            {
                case ArtistSorting.Unordered:
                    artistCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case ArtistSorting.Alphanumerical:
                    artistCollection.Sort(new NameComparer<IArtistCollectionItem>(isDescending));
                    break;
                case ArtistSorting.DateAdded:
                    artistCollection.Sort(new AddedAtComparer<IArtistCollectionItem>(isDescending));
                    break;
                case ArtistSorting.Duration:
                    artistCollection.Sort(new DurationComparer<IArtistCollectionItem>(isDescending));
                    break;
                case ArtistSorting.LastPlayed:
                    artistCollection.Sort(new LastPlayedComparer<IArtistCollectionItem>(isDescending));
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"TrackSortType {artistSorting} is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Sorts the playlists.
        /// </summary>
        /// <param name="playlistCollection">The collection to sort.</param>
        /// <param name="playlistSorting">Sort type of the collection.</param>
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortPlaylists(ObservableCollection<IPlaylistCollectionItem> playlistCollection, PlaylistSorting playlistSorting, ObservableCollection<IPlaylistCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = playlistCollection;
            }

            bool isDescending = playlistSorting.HasFlag(PlaylistSorting.Descending);

            // Remove the Descending flag from the enum.
            PlaylistSorting deflaggedSorting = playlistSorting
                & ~PlaylistSorting.Descending;
            
            // Switch the remaining value
            switch (deflaggedSorting)
            {
                case PlaylistSorting.Unordered:
                    playlistCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case PlaylistSorting.Alphanumerical:
                    playlistCollection.Sort(new NameComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                case PlaylistSorting.DateAdded:
                    playlistCollection.Sort(new AddedAtComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                case PlaylistSorting.Duration:
                    playlistCollection.Sort(new DurationComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                case PlaylistSorting.LastPlayed:
                    playlistCollection.Sort(new LastPlayedComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"PlaylistSortings type {playlistSorting} is not supported.");
                    break;
            }
        }
    }
}
