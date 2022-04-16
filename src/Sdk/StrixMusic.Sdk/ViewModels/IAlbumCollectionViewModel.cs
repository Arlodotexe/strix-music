// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IAlbumCollection" />.
    /// This is needed so because multiple view models implement <see cref="IAlbumCollection"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IAlbumCollection"/>.
    /// </summary>
    public interface IAlbumCollectionViewModel : ISdkViewModel, IAlbumCollection, IPlayableCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel, IAsyncInit
    {
        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// Keeps the default album collection while sorting.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> UnsortedAlbums { get; }

        /// <summary>
        /// The type of sorting used for <see cref="Albums"/>.
        /// </summary>
        public AlbumSortingType CurrentAlbumSortingType { get; }

        /// <summary>
        /// The direction to sort <see cref="Albums"/>.
        /// </summary>
        public SortDirection CurrentAlbumSortingDirection { get; }

        /// <summary>
        /// Populates the next set of albums into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreAlbumsAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorts the track collection by <see cref="AlbumSortingType"/>.
        /// </summary>
        /// <param name="albumSorting">The <see cref="AlbumSortingType"/> by which to sort.</param>
        /// <param name="sortDirection">The direction by which to sort.</param>
        public void SortAlbumCollection(AlbumSortingType albumSorting, SortDirection sortDirection);

        /// <summary>
        /// Loads the entire collection of <see cref="IAlbumCollectionItem"/> s and ensures all sources are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAlbumCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the list of the <see cref="IAlbum"/>.
        /// </summary>
        public IAsyncRelayCommand InitAlbumCollectionAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreAlbumsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IAlbumCollectionBase.PlayAlbumCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <summary>
        /// Plays a single album from this album collection.
        /// </summary>
        public IAsyncRelayCommand<IAlbumCollectionItem> PlayAlbumAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IAlbumCollectionBase.PauseAlbumCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        /// <summary>
        /// Sorts the collection with a new type.
        /// </summary>
        public IRelayCommand<AlbumSortingType> ChangeAlbumCollectionSortingTypeCommand { get; }

        /// <summary>
        /// Sorts the collection with a new direction.
        /// </summary>
        public IRelayCommand<SortDirection> ChangeAlbumCollectionSortingDirectionCommand { get; }
    }
}
