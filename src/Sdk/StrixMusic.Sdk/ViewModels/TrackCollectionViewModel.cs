using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ITrackCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class TrackCollectionViewModel : ObservableObject, ITrackCollectionBaseViewModel
    {
        private readonly ITrackCollectionBase _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITrackCollectionViewModel"/> class.
        /// </summary>
        /// <param name="collection">The base <see cref="ITrackCollection"/> containing properties about this class.</param>
        public TrackCollectionViewModel(ITrackCollectionBase collection)
        {
            _collection = collection;

            SourceCore = collection?.SourceCore != null ? MainViewModel.GetLoadedCore(collection.SourceCore) : null;

            Tracks = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<TrackViewModel>());

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            await foreach (var item in _collection.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public ICore? SourceCore { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _collection.TotalTracksCount;

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index) => _collection.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collection.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _collection.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _collection.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset) => _collection.GetTracksAsync(limit, offset);
    }
}