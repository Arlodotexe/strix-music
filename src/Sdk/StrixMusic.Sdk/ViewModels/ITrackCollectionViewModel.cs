using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="ITrackCollection" />. This is needed so because multiple view models implement <see cref="ITrackCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="ITrackCollection"/>.
    /// </summary>
    public interface ITrackCollectionViewModel : ITrackCollection, IAsyncInit
    {
        /// <summary>
        /// The tracks in this collection.
        /// </summary>
        public ObservableCollection<TrackViewModel> Tracks { get; }

        /// <summary>
        /// Populates the next set of tracks into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreTracksAsync(int limit);

        /// <inheritdoc cref="PopulateMoreTracksAsync" />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <summary>
        /// <inheritdoc cref="ITrackCollectionBase.PlayTrackCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <summary>
        /// Plays a single track from this track collection.
        /// </summary>
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="ITrackCollectionBase.PauseTrackCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <summary>
        /// Command to change the name, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Command to change the description, if supported.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Command to change the duration, if supported.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }
    }
}
