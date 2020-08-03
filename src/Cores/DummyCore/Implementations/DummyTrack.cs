using System;
using System.Collections.Generic;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyTrack : ITrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyTrack"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="isExplicit"></param>
        /// <param name="duration"></param>
        /// <param name="device"></param>
        /// <param name="core"></param>
        public DummyTrack(string id, string title, bool isExplicit, long duration, IDevice device, ICore core)
        {
            Id = id;
            Title = title;
            IsExplicit = isExplicit;
            Duration = duration;
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public List<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Type => "song";

        /// <inheritdoc/>
        public string Title { get; }

        /// <inheritdoc/>
        public IArtist Artist => DummyArtist;

        /// <summary>
        /// Full <see cref="DummyArtist"/> to be used within the DummyCore.
        /// </summary>
        public DummyArtist DummyArtist { get; }

        /// <inheritdoc/>
        public IAlbum Album => DummyAlbum;

        /// <summary>
        /// Full <see cref="DummyAlbum"/> to be used within the DummyCore.
        /// </summary>
        public DummyAlbum DummyAlbum { get; }

        /// <inheritdoc/>
        public DateTime? DatePublished => throw new NotImplementedException();

        /// <inheritdoc/>
        public IList<string> Genre => throw new NotImplementedException();

        /// <inheritdoc/>
        public int? TrackNumber => throw new NotImplementedException();

        /// <inheritdoc/>
        public int PlayCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Language => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILyrics Lyrics => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsExplicit { get; }

        /// <inheritdoc/>
        public long Duration { get; }

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IDevice Device { get; private set; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }
    }
}
