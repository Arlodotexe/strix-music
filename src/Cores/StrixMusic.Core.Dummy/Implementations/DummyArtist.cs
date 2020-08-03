using System;
using System.Collections.Generic;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyArtist : IArtist
    {
        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<IAlbum> Albums => (IList<IAlbum>)DummyAlbums;

        /// <summary>
        /// List of full <see cref="DummyAlbum"/>s to be used within the DummyCore.
        /// </summary>
        public List<DummyAlbum> DummyAlbums { get; set; }

        /// <summary>
        /// List of the Ids of <see cref="DummyAlbum"/>s to the <see cref="DummyArtist"/>
        /// </summary>
        public List<string> AlbumIds { get; set; }

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
        public IList<IPlayableCollectionBase> Items => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyArtist"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="core"></param>
        public DummyArtist(string id, string name, ICore core)
        {
            Id = id;
            Name = name;
            DummyAlbums = new List<DummyAlbum>();
            AlbumIds = new List<string>();
            SourceCore = core;
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
