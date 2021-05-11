using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.ViewModels.Helpers
{
    /// <summary>
    /// Helpers for initializing collection view models.
    /// </summary>
    public static class CollectionInit
    {
        /// <summary>
        /// Initialize a track collection view model.
        /// </summary>
        /// <param name="trackCollection">The collection to initialize.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task TrackCollection(ITrackCollectionViewModel trackCollection)
        {
            // TODO Save/get to collection cache. All tracks collections should be cached to disk.
            if (trackCollection.Tracks.Count < trackCollection.TotalTracksCount)
            {
                var allTracks = await APIs.GetAllItemsAsync<ITrack>(trackCollection.TotalTracksCount, async offset => await trackCollection.GetTracksAsync(trackCollection.TotalTracksCount, offset));

                foreach (var track in allTracks)
                {
                    Guard.IsAssignableToType(track, typeof(MergedTrack), nameof(track));

                    trackCollection.Tracks.Add(new TrackViewModel(track));
                }
            }
        }
    }
}