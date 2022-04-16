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
    /// An interfaced ViewModel for <see cref="IArtistCollection" />.
    /// This is needed so because multiple view models implement <see cref="IArtistCollection"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IArtistCollection"/>.
    /// </summary>
    public interface IArtistCollectionViewModel : ISdkViewModel, IArtistCollection, IPlayableCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel, IAsyncInit
    {
        /// <summary>
        /// The artist items in this collection.
        /// </summary>
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <summary>
        /// Keeps the default artists collection while sorting.
        /// </summary>
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        /// <summary>
        /// Sorts the artist collection by <see cref="ArtistSortingType"/>.
        /// </summary>
        /// <param name="artistSorting">The <see cref="ArtistSortingType"/> by which to sort.</param>
        /// <param name="sortDirection">The direction by which to sort.</param>
        public void SortArtistCollection(ArtistSortingType artistSorting, SortDirection sortDirection);

        /// <summary>
        /// Populates the next set of artists into the collection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreArtistsAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// The sorting direction of artists in the collection. 
        /// </summary>
        public ArtistSortingType CurrentArtistSortingType { get; }

        /// <summary>
        /// The sorting direction of artists in the collection. 
        /// </summary>
        public SortDirection CurrentArtistSortingDirection { get; }

        /// <summary>
        /// Loads the entire collection of <see cref="IArtistCollectionItem"/>s and ensures all sources are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitArtistCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the list of the <see cref="IArtist"/>.
        /// </summary>
        public IAsyncRelayCommand InitArtistCollectionAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreArtistsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IArtistCollectionBase.PlayArtistCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <summary>
        /// Plays a single artist from this artist collection.
        /// </summary>
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IArtistCollectionBase.PauseArtistCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }
        
        /// <summary>
        /// Sorts the collection with a new type.
        /// </summary>
        public IRelayCommand<ArtistSortingType> ChangeArtistCollectionSortingTypeCommand { get; }
        
        /// <summary>
        /// Sorts the collection with a new direction.
        /// </summary>
        public IRelayCommand<SortDirection> ChangeArtistCollectionSortingDirectionCommand { get; }
    }
}
