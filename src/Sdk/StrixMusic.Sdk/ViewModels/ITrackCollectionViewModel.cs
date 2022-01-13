using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;

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
        /// <param name="trackSorting">The <see cref="TrackSortingType"/> by which to sort.</param>
        /// <param name="sortDirection">The direction by which to sort.</param>
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection);

        /// <summary>
        /// Loads the collection of <see cref="ITrack"/> for the first time.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitTrackCollectionAsync();

        /// <summary>
        /// Initializes the list of the <see cref="ITrack"/>.
        /// </summary>
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

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
        /// Adjustes sorting to maintain its direction, with a new type.
        /// </summary>
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <summary>
        /// Sorts adjustes sorting to maintain its type, with a new direction.
        /// </summary>
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }
    }
}
