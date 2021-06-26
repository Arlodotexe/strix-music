using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.ViewModels.Helpers.Comparers;

namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Helper to perform operations on a collection of tracks.
    /// </summary>
    public static class TracksHelper
    {
        /// <summary>
        /// Initialize a track collection view model.
        /// </summary>
        /// <param name="trackCollection">The collection to initialize.</param>
        /// <param name="tracksSortType">Sort type of the collection.</param>
        /// <param name="originalOrder">The original order of the trackCollection.</param>
        public static void SortTracks(IList<TrackViewModel> trackCollection, TracksSortType tracksSortType, IList<TrackViewModel> originalOrder)
        {
            if (!originalOrder.Any())
            {
                originalOrder = trackCollection;
            }

            switch (tracksSortType)
            {
                case TracksSortType.Ascending:
                    trackCollection.ToList().Sort(new NameComparision<TrackViewModel>());
                    break;
                case TracksSortType.Descending:
                    trackCollection.ToList().Sort(new NameComparision<TrackViewModel>());
                    trackCollection.ToList().Reverse();
                    break;
                case TracksSortType.Unordered:
                    originalOrder.Clear();
                    break;
                case TracksSortType.TrackNumber:
                    break;
                case TracksSortType.AddedAt:
                    break;
                case TracksSortType.Duration:
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException($"TrackSortType {tracksSortType} is not supported.");
                    break;
            }
        }
    }
}
