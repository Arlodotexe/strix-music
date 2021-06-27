using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.ViewModels.Helpers.Comparers;

namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Helper to perform operations on a collection of tracks.
    /// </summary>
    public static class CollectionSorting
    {
        /// <summary>
        /// Initialize a track collection view model.
        /// </summary>
        /// <param name="trackCollection">The collection to initialize.</param>
        /// <param name="trackSorting">Sort type of the collection.</param>
        /// <param name="originalOrder">The original order of the trackCollection.</param>
        public static void SortTracks(ObservableCollection<TrackViewModel> trackCollection, TrackSorting trackSorting, ObservableCollection<TrackViewModel> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = trackCollection;
            }

            switch (trackSorting)
            {
                case TrackSorting.Ascending:
                    trackCollection.Sort(new NameComparer<TrackViewModel>());
                    break;
                case TrackSorting.Descending:
                    trackCollection.Sort(new ReverseNameComparer<TrackViewModel>());
                    break;
                case TrackSorting.Unordered:
                    trackCollection = originalOrder;
                    originalOrder.Clear();
                    break;
                case TrackSorting.TrackNumber:
                    trackCollection.Sort(new TrackNumberComparer<TrackViewModel>());
                    break;
                case TrackSorting.AddedAt:
                    trackCollection.Sort(new AddedAtComparer<TrackViewModel>());
                    break;
                case TrackSorting.Duration:
                    trackCollection.Sort(new DurationComparer<TrackViewModel>());
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"TrackSortType {trackSorting} is not supported.");
                    break;
            }
        }
    }
}
