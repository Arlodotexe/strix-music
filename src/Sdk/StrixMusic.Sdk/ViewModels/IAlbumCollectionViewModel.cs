using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable <see cref="ICoreAlbumCollection" />
    /// </summary>
    public interface IAlbumCollectionViewModel : ICoreAlbumCollection
    {
        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ICoreAlbumCollectionItem> Albums { get; }

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
    }
}