using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;

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
        /// Keeps the default track collection while sorting.
        /// </summary>
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <summary>
        /// Populates the next set of tracks into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreTracksAsync(int limit);

        /// <summary>
        /// The current sorting type of tracks in the collection.
        /// </summary>
        public TrackSortingType CurrentTracksSortingType { get; }

        /// <summary>
        /// The current sorting direction of tracks in the collection. 
        /// </summary>
        public SortDirection CurrentTracksSortingDirection { get; }

        /// <summary>
        /// Sorts the track collection by <see cref="TrackSortingType"/>.
        /// </summary>
        /// <param name="trackSorting">The <see cref="TrackSortingType"/> according to which the order should be changed.</param>
        public void SortTrackCollection(TrackSortingType trackSorting);

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
        /// Sorts track collection by <see cref="TrackSortingType"/>.
        /// </summary>
        public RelayCommand<TrackSortingType> SortTrackCollectionCommand { get; }

        /// <summary>
        /// Adjustes sorting to maintain its direction, with a new type.
        /// </summary>
        public RelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <summary>
        /// Sorts adjustes sorting to maintain its type, with a new direction.
        /// </summary>
        public RelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }
    }
}
