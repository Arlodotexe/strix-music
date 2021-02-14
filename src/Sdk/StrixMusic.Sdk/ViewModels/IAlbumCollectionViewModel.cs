using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IAlbumCollection" />. This is needed so because multiple view models implement <see cref="IAlbumCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IAlbumCollection"/>.
    /// </summary>
    public interface IAlbumCollectionViewModel : IAlbumCollection, IPlayableCollectionViewModel, IImageCollectionViewModel
    {
        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// Populates the next set of albums into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreAlbumsAsync(int limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreAlbumsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IAlbumCollectionBase.PlayAlbumCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IAlbumCollectionBase.PauseAlbumCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }
    }
}