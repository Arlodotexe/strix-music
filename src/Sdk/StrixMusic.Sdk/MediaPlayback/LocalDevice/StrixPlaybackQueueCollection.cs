using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Extensions;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.MediaPlayback.LocalDevice
{
    /// <summary>
    /// The playback queue for the local <see cref="StrixDevice"/>.
    /// </summary>
    public class StrixPlaybackQueueCollection : ITrackCollectionBase
    {
        private readonly IPlaybackHandlerService _playbackHandler = Ioc.Default.GetService<IPlaybackHandlerService>();

        /// <inheritdoc />
        public ICore? SourceCore => IsPlaybackQueueSameSource() ? _playbackHandler.CurrentItem?.Track.SourceCore : null;

        /// <inheritdoc />
        public int TotalTracksCount { get; }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => Task.FromResult(true);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => Task.FromResult(true);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        private bool IsPlaybackQueueSameSource()
        {
            var prevSources = _playbackHandler.PreviousItems.DistinctBy(x => x.Track.SourceCore);
            var nextSources = _playbackHandler.NextItems.DistinctBy(x => x.Track.SourceCore);

            // Handle possible multiple enumeration
            var prevMediaSources = prevSources as IMediaSourceConfig[] ?? prevSources.ToArray();
            var nextMediaSources = nextSources as IMediaSourceConfig[] ?? nextSources.ToArray();

            return !(prevMediaSources.Length > 1 ||
                     nextMediaSources.Length > 1 ||
                     !nextMediaSources.Contains(_playbackHandler.CurrentItem) ||
                     !prevMediaSources.Contains(_playbackHandler.CurrentItem));
        }
    }
}