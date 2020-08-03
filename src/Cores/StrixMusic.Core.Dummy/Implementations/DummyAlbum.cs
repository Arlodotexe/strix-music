using System;
using System.Collections.Generic;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyAlbum : IAlbum
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public IList<ITrack> Tracks => (IList<ITrack>)DummyTracks;

        /// <summary>
        /// List of full <see cref="DummyTrack"/>s to be used within the DummyCore.
        /// </summary>
        public List<DummyTrack> DummyTracks { get; set; } = new List<DummyTrack>();

        /// <summary>
        /// List of the Ids of <see cref="DummyTrack"/>s on the <see cref="DummyAlbum"/>.
        /// </summary>
        public List<string> TrackIds { get; set; }

        /// <inheritdoc/>
        public IArtist Artist => DummyArtist;

        /// <summary>
        /// The full <see cref="DummyArtist"/> of the album.
        /// </summary>
        public DummyArtist DummyArtist { get; set; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyAlbum"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public DummyAlbum(string id, string name, DummyArtist artist)
        {
            Id = id;
            Name = name;
            TrackIds = new List<string>();
            DummyArtist = artist;
        }

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
