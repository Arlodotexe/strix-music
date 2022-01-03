using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
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
        /// <param name="sortDirection">The direction to sort the collection.</param>
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortTracks(ObservableCollection<TrackViewModel> trackCollection, TrackSortingType trackSorting, SortDirection sortDirection, ObservableCollection<TrackViewModel> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = trackCollection;
            }

            if (sortDirection == SortDirection.Unsorted ||
                trackSorting == TrackSortingType.Unsorted)
            {
                trackCollection = originalOrder;
                originalOrder.Clear();
                return;
            }

            bool isDescending = sortDirection == SortDirection.Descending;

            switch (trackSorting)
            {
                case TrackSortingType.Alphanumerical:
                    trackCollection.Sort(new NameComparer<TrackViewModel>(isDescending));
                    break;
                case TrackSortingType.TrackNumber:
                    trackCollection.Sort(new TrackNumberComparer<TrackViewModel>(isDescending));
                    break;
                case TrackSortingType.DateAdded:
                    trackCollection.Sort(new AddedAtComparer<TrackViewModel>(isDescending));
                    break;
                case TrackSortingType.Duration:
                    trackCollection.Sort(new DurationComparer<TrackViewModel>(isDescending));
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"{nameof(TrackSortingType)}: {trackSorting} is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Sorts the albums.
        /// </summary>
        /// <param name="albumCollection">The collection to sort.</param>
        /// <param name="albumSorting">Sort type of the collection.</param>
        /// <param name="sortDirection">The direction to sort the collection.</param>
        /// <param name="originalOrder">The original order of the albumCollection.</param>
        public static void SortAlbums(ObservableCollection<IAlbumCollectionItem> albumCollection, AlbumSortingType albumSorting, SortDirection sortDirection, ObservableCollection<IAlbumCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = albumCollection;
            }

            if (sortDirection == SortDirection.Unsorted ||
                albumSorting == AlbumSortingType.Unsorted)
            {
                albumCollection = originalOrder;
                originalOrder.Clear();
                return;
            }

            bool isDescending = sortDirection == SortDirection.Descending;

            switch (albumSorting)
            {
                case AlbumSortingType.Alphanumerical:
                    albumCollection.Sort(new NameComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSortingType.DateAdded:
                    albumCollection.Sort(new AddedAtComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSortingType.Duration:
                    albumCollection.Sort(new DurationComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSortingType.LastPlayed:
                    albumCollection.Sort(new LastPlayedComparer<IAlbumCollectionItem>(isDescending));
                    break;
                case AlbumSortingType.DatePublished:
                    throw new NotImplementedException(); // IAlbumCollectionItem don't have DatePublished property. IAlbumBase has it. The potential solution will be is to move those IAlbumBase items to IAlbumCollectionItem.
                default:
                    ThrowHelper.ThrowNotSupportedException($"{nameof(AlbumSortingType)}: {albumSorting} is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Sorts the artists.
        /// </summary>
        /// <param name="artistCollection">The collection to sort.</param>
        /// <param name="artistSorting">Sort type of the collection.</param>
        /// <param name="sortDirection">The direction to sort the collection.</param>
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortArtists(ObservableCollection<IArtistCollectionItem> artistCollection, ArtistSortingType artistSorting, SortDirection sortDirection, ObservableCollection<IArtistCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = artistCollection;
            }

            if (sortDirection == SortDirection.Unsorted ||
                artistSorting == ArtistSortingType.Unsorted)
            {
                artistCollection = originalOrder;
                originalOrder.Clear();
                return;
            }

            bool isDescending = sortDirection == SortDirection.Descending;

            switch (artistSorting)
            {
                case ArtistSortingType.Alphanumerical:
                    artistCollection.Sort(new NameComparer<IArtistCollectionItem>(isDescending));
                    break;
                case ArtistSortingType.DateAdded:
                    artistCollection.Sort(new AddedAtComparer<IArtistCollectionItem>(isDescending));
                    break;
                case ArtistSortingType.Duration:
                    artistCollection.Sort(new DurationComparer<IArtistCollectionItem>(isDescending));
                    break;
                case ArtistSortingType.LastPlayed:
                    artistCollection.Sort(new LastPlayedComparer<IArtistCollectionItem>(isDescending));
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"{nameof(ArtistSortingType)}: {artistSorting} is not supported.");
                    break;
            }
        }

        /// <summary>
        /// Sorts the playlists.
        /// </summary>
        /// <param name="playlistCollection">The collection to sort.</param>
        /// <param name="playlistSorting">Sort type of the collection.</param>
        /// <param name="sortDirection">The direction to sort the collection.</param>
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortPlaylists(ObservableCollection<IPlaylistCollectionItem> playlistCollection, PlaylistSortingType playlistSorting, SortDirection sortDirection, ObservableCollection<IPlaylistCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = playlistCollection;
            }

            if (sortDirection == SortDirection.Unsorted ||
                playlistSorting == PlaylistSortingType.Unsorted)
            {
                playlistCollection = originalOrder;
                originalOrder.Clear();
                return;
            }

            bool isDescending = sortDirection == SortDirection.Descending;

            switch (playlistSorting)
            {
                case PlaylistSortingType.Alphanumerical:
                    playlistCollection.Sort(new NameComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                case PlaylistSortingType.DateAdded:
                    playlistCollection.Sort(new AddedAtComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                case PlaylistSortingType.Duration:
                    playlistCollection.Sort(new DurationComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                case PlaylistSortingType.LastPlayed:
                    playlistCollection.Sort(new LastPlayedComparer<IPlaylistCollectionItem>(isDescending));
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"{nameof(PlaylistSortingType)}: {playlistSorting} is not supported.");
                    break;
            }
        }
    }
}
