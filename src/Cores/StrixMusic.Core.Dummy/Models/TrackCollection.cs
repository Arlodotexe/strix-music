using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.Core.Dummy.Implementations;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Models
{
    /// <summary>
    /// A generic collection of <see cref="DummyTrack"/>s.
    /// </summary>
    public class TrackCollection : IPlaylist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackCollection"/> class.
        /// </summary>
        /// <param name="tracks">The <see cref="DummyTrack"/>s in the <see cref="TrackCollection"/>.</param>
        /// <param name="core">The <see cref="DummyCore"/>.</param>
        public TrackCollection(List<DummyTrack> tracks, DummyCore core)
        {
            SourceCore = core;
            Tracks = tracks;
        }

        /// <inheritdoc/>
        public string Id => string.Empty;

        /// <inheritdoc/>
        public string Name => "Tracks";

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile? Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TrackCount { get => Tracks?.Count() ?? 0; set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks { get; }

        /// <inheritdoc/>
        public ITrack? PlayingTrack => null;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public int TotalCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
