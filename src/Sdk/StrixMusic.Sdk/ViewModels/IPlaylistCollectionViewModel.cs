using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IPlaylistCollection" />. This is needed so because multiple view models implement <see cref="IPlaylistCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IPlaylistCollection"/>.
    /// </summary>
    public interface IPlaylistCollectionViewModel : IPlaylistCollection
    {

        /// <summary>
        /// Keeps the default track collection while sorting.
        /// </summary>
        public ObservableCollection<IPlaylistCollectionItem> UnsortedPlaylists { get; }

        /// <summary>
        /// The playlists in this collection
        /// </summary>
        public ObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <summary>
        /// The current sorting of the playlists.
        /// </summary>
        public PlaylistSorting CurrentPlaylistSorting { get; }

        /// <summary>
        /// Populates the next set of playlists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMorePlaylistsAsync(int limit);

        /// <inheritdoc cref="PopulateMorePlaylistsAsync" />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <summary>
        /// Sorts the playlist collection by <see cref="PlaylistSorting"/>.
        /// </summary>
        /// <param name="playlistSorting">The <see cref="PlaylistSorting"/> according to which the order should be changed.</param>
        public void SortPlaylistCollection(PlaylistSorting playlistSorting);

        /// <summary>
        /// <inheritdoc cref="IPlaylistCollectionBase.PlayPlaylistCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayPlaylistCollectionAsyncCommand { get; }

        /// <summary>
        /// Plays a single playlist from this playlist collection.
        /// </summary>
        public IAsyncRelayCommand<IPlaylistCollectionItem> PlayPlaylistAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IPlaylistCollectionBase.PausePlaylistCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PausePlaylistCollectionAsyncCommand { get; }

        /// <summary>
        /// Sorts playlists collection by <see cref="PlaylistSorting"/>.
        /// </summary>
        public RelayCommand<PlaylistSorting> SortPlaylistCollectionCommand { get; }
    }
}