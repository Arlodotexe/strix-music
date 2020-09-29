using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// An observable <see cref="ITrackCollection"/>.
    /// </summary>
    public interface IObservableTrackCollection : ITrackCollection
    {
        /// <summary>
        /// The tracks in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ObservableTrack> Tracks { get; }

        /// <summary>
        /// Populates the next set of tracks into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreTracksAsync(int limit);

        /// <inheritdoc cref="PopulateMoreTracksAsync" />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }
    }
}