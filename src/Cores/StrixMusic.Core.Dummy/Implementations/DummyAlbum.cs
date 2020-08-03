using System;
using System.Collections.Generic;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyAlbum : IAlbum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyAlbum"/> class.
        /// </summary>
        public DummyAlbum(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<ITrack> Tracks => (IList<ITrack>)DummyTracks;

        /// <summary>
        /// List of full <see cref="DummyTrack"/>s to be used within the DummyCore.
        /// </summary>
        public List<DummyTrack> DummyTracks { get; set; } = new List<DummyTrack>();

        /// <inheritdoc/>
        public IArtist Artist { get; private set; }

        /// <inheritdoc/>
        public IList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState State => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITrack PlayingTrack => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public void Play()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            throw new NotImplementedException();
        }
    }
}
