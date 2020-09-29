using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// An observable <see cref="IArtistCollection"/>.
    /// </summary>
    public interface IObservableArtistCollection : IArtistCollection
    {
        /// <summary>
        /// The artists in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ObservableArtist> Artists { get; }

        /// <summary>
        /// Populates the next set of artists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreArtistsAsync(int limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreArtistsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }
    }
}