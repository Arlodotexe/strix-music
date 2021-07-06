using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        public PlaylistSortingType CurrentPlaylistSortingType { get; }

        /// <summary>
        /// The current sorting of the playlists.
        /// </summary>
        public SortDirection CurrentPlaylistSortingDirection { get; }

        /// <summary>
        /// Populates the next set of playlists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMorePlaylistsAsync(int limit);

        /// <inheritdoc cref="PopulateMorePlaylistsAsync" />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <summary>
        /// Sorts the playlist collection by <see cref="PlaylistSortingType"/>.
        /// </summary>
        /// <param name="playlistSorting">The <see cref="PlaylistSortingType"/> by which to sort.</param>
        /// <param name="sortDirection">The direction by which to sort.</param>
        public void SortPlaylistCollection(PlaylistSortingType playlistSorting, SortDirection sortDirection);

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
        /// Adjustes sorting to maintain its direction, with a new type.
        /// </summary>
        public RelayCommand<PlaylistSortingType> ChangePlaylistCollectionSortingTypeCommand { get; }

        /// <summary>
        /// Adjustes sorting to maintain its type, with a new direction.
        /// </summary>
        public RelayCommand<SortDirection> ChangePlaylistCollectionSortingDirectionCommand { get; }
    }
}
