using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable <see cref="ICorePlaylistCollection"/>.
    /// </summary>
    public interface IPlaylistCollectionViewModel : ICorePlaylistCollection
    {
        /// <summary>
        /// The playlists in this collection
        /// </summary>
        public SynchronizedObservableCollection<ICorePlaylistCollectionItem> Playlists { get; }

        /// <summary>
        /// Populates the next set of playlists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMorePlaylistsAsync(int limit);

        /// <inheritdoc cref="PopulateMorePlaylistsAsync" />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }
    }
}