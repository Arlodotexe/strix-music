using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IAlbumCollection" />. This is needed so because multiple view models implement <see cref="IAlbumCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IAlbumCollection"/>.
    /// </summary>
    public interface IAlbumCollectionViewModel : IAlbumCollection, IPlayableCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel, IAsyncInit
    {
        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// Keeps the default album collection while sorting.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> UnsortedAlbums { get; }

        /// <inheritdoc />
        public AlbumSortingType CurrentAlbumSortingType { get; }

        /// <inheritdoc />
        public SortDirection CurrentAlbumSortingDirection { get; }

        /// <summary>
        /// Populates the next set of albums into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreAlbumsAsync(int limit);

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
        public Task InitAlbumCollectionAsync();

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
        /// Adjustes sorting to maintain its direction, with a new type.
        /// </summary>
        public IRelayCommand<AlbumSortingType> ChangeAlbumCollectionSortingTypeCommand { get; }

        /// <summary>
        /// Sorts adjustes sorting to maintain its type, with a new direction.
        /// </summary>
        public IRelayCommand<SortDirection> ChangeAlbumCollectionSortingDirectionCommand { get; }
    }
}
