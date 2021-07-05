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
        public static void SortTracks(ObservableCollection<TrackViewModel> trackCollection, TrackSortingType trackSorting, ObservableCollection<TrackViewModel> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = trackCollection;
            }

            bool isDescending = trackSorting.HasFlag(TrackSortingType.Descending);

            // Remove the Descending flag from the enum.
            TrackSortingType deflaggedSorting = trackSorting
                & ~TrackSortingType.Descending;

            switch (deflaggedSorting)
            {
                case TrackSortingType.Unordered:
                    trackCollection = originalOrder;
                    originalOrder.Clear();
                    break;
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
        /// <param name="originalOrder">The original order of the albumCollection.</param>
        public static void SortAlbums(ObservableCollection<IAlbumCollectionItem> albumCollection, AlbumSortingType albumSorting, ObservableCollection<IAlbumCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = albumCollection;
            }

            bool isDescending = albumSorting.HasFlag(AlbumSortingType.Descending);

            // Remove the Descending flag from the enum.
            AlbumSortingType deflaggedSorting = albumSorting
                & ~AlbumSortingType.Descending;

            switch (deflaggedSorting)
            {
                case AlbumSortingType.Unordered:
                    albumCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case AlbumSortingType.Descending:
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
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortArtists(ObservableCollection<IArtistCollectionItem> artistCollection, ArtistSortingType artistSorting, ObservableCollection<IArtistCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = artistCollection;
            }

            bool isDescending = artistSorting.HasFlag(ArtistSortingType.Descending);

            // Remove the Descending flag from the enum.
            ArtistSortingType deflaggedSorting = artistSorting
                & ~ArtistSortingType.Descending;

            switch (deflaggedSorting)
            {
                case ArtistSortingType.Unordered:
                    artistCollection = originalOrder;
                    originalOrder.Clear();
                    break;
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
        /// <param name="originalOrder">The original order of the playlistCollection.</param>
        public static void SortPlaylists(ObservableCollection<IPlaylistCollectionItem> playlistCollection, PlaylistSortingType playlistSorting, ObservableCollection<IPlaylistCollectionItem> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = playlistCollection;
            }

            bool isDescending = playlistSorting.HasFlag(PlaylistSortingType.Descending);

            // Remove the Descending flag from the enum.
            PlaylistSortingType deflaggedSorting = playlistSorting
                & ~PlaylistSortingType.Descending;
            
            // Switch the remaining value
            switch (deflaggedSorting)
            {
                case PlaylistSortingType.Unordered:
                    playlistCollection = originalOrder;
                    originalOrder.Clear();
                    break;
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
