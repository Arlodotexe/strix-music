// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IPlaylistCollection" />.
    /// This is needed so because multiple view models implement <see cref="IPlaylistCollection"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IPlaylistCollection"/>.
    /// </summary>
    public interface IPlaylistCollectionViewModel : ISdkViewModel, IPlaylistCollection, IUrlCollectionViewModel, IAsyncInit
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
        public PlaylistSortingType CurrentPlaylistSortingType { get; }

        /// <summary>
        /// The current sorting of the playlists.
        /// </summary>
        public SortDirection CurrentPlaylistSortingDirection { get; }

        /// <summary>
        /// Populates the next set of playlists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMorePlaylistsAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the entire collection of <see cref="IPlaylistCollectionItem"/>s and ensures all sources are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitPlaylistCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the list of the <see cref="IPlaylist"/>.
        /// </summary>
        public IAsyncRelayCommand InitPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc cref="PopulateMorePlaylistsAsync" />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <summary>
        /// Sorts the playlist collection by <see cref="PlaylistSortingType"/>.
        /// </summary>
        /// <param name="playlistSorting">The <see cref="PlaylistSortingType"/> by which to sort.</param>
        /// <param name="sortDirection">The direction by which to sort.</param>
        public void SortPlaylistCollection(PlaylistSortingType playlistSorting, SortDirection sortDirection);

        /// <inheritdoc cref="IPlaylistCollectionBase.PlayPlaylistCollectionAsync"/>
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
        /// Sorts the collection with a new type.
        /// </summary>
        public IRelayCommand<PlaylistSortingType> ChangePlaylistCollectionSortingTypeCommand { get; }
        
        /// <summary>
        /// Sorts the collection with a new direction.
        /// </summary>
        public IRelayCommand<SortDirection> ChangePlaylistCollectionSortingDirectionCommand { get; }
    }
}
