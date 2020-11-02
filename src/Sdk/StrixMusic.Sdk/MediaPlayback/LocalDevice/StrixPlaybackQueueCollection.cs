using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.MediaPlayback.LocalDevice
{
    /// <summary>
    /// The playback queue for the local <see cref="StrixDevice"/>.
    /// </summary>
    public class StrixPlaybackQueueCollection : ITrackCollection
    {
        private readonly IPlaybackHandlerService _playbackHandler = Ioc.Default.GetService<IPlaybackHandlerService>();

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; } = new List<ICore>();

        /// <inheritdoc />
        public int TotalTracksCount { get; }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            // solve me:
            // 1. convert SdkMember back to CoreMember (un-merge)
            // 2. 
            track.SourceCores.First().GetMediaSource()
            _playbackHandler.InsertNext(0, );
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
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }
    }
}