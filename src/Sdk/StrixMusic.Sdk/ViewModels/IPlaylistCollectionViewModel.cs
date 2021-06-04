using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IPlaylistCollection" />. This is needed so because multiple view models implement <see cref="IPlaylistCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IPlaylistCollection"/>.
    /// </summary>
    public interface IPlaylistCollectionViewModel : IPlaylistCollection
    {
        /// <summary>
        /// The playlists in this collection
        /// </summary>
        public ObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <summary>
        /// Populates the next set of playlists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMorePlaylistsAsync(int limit);

        /// <inheritdoc cref="PopulateMorePlaylistsAsync" />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

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
    }
}