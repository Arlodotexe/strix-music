using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// An observable <see cref="IPlaylistCollection"/>.
    /// </summary>
    public interface IPlaylistCollectionViewModel : IPlaylistCollection
    {
        /// <summary>
        /// The playlists in this collection
        /// </summary>
        public SynchronizedObservableCollection<IPlaylistCollectionItem> Playlists { get; }

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