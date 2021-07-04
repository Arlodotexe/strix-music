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

            switch (trackSorting)
            {
                case TrackSorting.Ascending:
                    trackCollection.Sort(new NameComparer<TrackViewModel>());
                    break;
                case TrackSorting.Descending:
                    trackCollection.Sort(new ReverseNameComparer<TrackViewModel>());
                    break;
                case TrackSorting.Unordered:
                    trackCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case TrackSorting.TrackNumber:
                    trackCollection.Sort(new TrackNumberComparer<TrackViewModel>());
                    break;
                case TrackSorting.AddedAt:
                    trackCollection.Sort(new AddedAtComparer<TrackViewModel>());
                    break;
                case TrackSorting.Duration:
                    trackCollection.Sort(new DurationComparer<TrackViewModel>());
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

            switch (albumSorting)
            {
                case AlbumSorting.Ascending:
                    albumCollection.Sort(new NameComparer<IAlbumCollectionItem>());
                    break;
                case AlbumSorting.Descending:
                    albumCollection.Sort(new ReverseNameComparer<IAlbumCollectionItem>());
                    break;
                case AlbumSorting.Unordered:
                    albumCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case AlbumSorting.AddedAt:
                    albumCollection.Sort(new AddedAtComparer<IAlbumCollectionItem>());
                    break;
                case AlbumSorting.Duration:
                    albumCollection.Sort(new DurationComparer<IAlbumCollectionItem>());
                    break;
                case AlbumSorting.LastPlayed:
                    albumCollection.Sort(new LastPlayedComparer<IAlbumCollectionItem>());
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

            switch (artistSorting)
            {
                case ArtistSorting.Ascending:
                    artistCollection.Sort(new NameComparer<IArtistCollectionItem>());
                    break;
                case ArtistSorting.Descending:
                    artistCollection.Sort(new ReverseNameComparer<IArtistCollectionItem>());
                    break;
                case ArtistSorting.Unordered:
                    artistCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case ArtistSorting.AddedAt:
                    artistCollection.Sort(new AddedAtComparer<IArtistCollectionItem>());
                    break;
                case ArtistSorting.Duration:
                    artistCollection.Sort(new DurationComparer<IArtistCollectionItem>());
                    break;
                case ArtistSorting.LastPlayed:
                    artistCollection.Sort(new LastPlayedComparer<IArtistCollectionItem>());
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

            switch (playlistSorting)
            {
                case PlaylistSorting.Ascending:
                    playlistCollection.Sort(new NameComparer<IPlaylistCollectionItem>());
                    break;
                case PlaylistSorting.Descending:
                    playlistCollection.Sort(new ReverseNameComparer<IPlaylistCollectionItem>());
                    break;
                case PlaylistSorting.Unordered:
                    playlistCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case PlaylistSorting.AddedAt:
                    playlistCollection.Sort(new AddedAtComparer<IPlaylistCollectionItem>());
                    break;
                case PlaylistSorting.Duration:
                    playlistCollection.Sort(new DurationComparer<IPlaylistCollectionItem>());
                    break;
                case PlaylistSorting.LastPlayed:
                    playlistCollection.Sort(new LastPlayedComparer<IPlaylistCollectionItem>());
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"TrackSortType {playlistCollection} is not supported.");
                    break;
            }
        }
    }
}
