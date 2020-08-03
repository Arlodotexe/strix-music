using System;
using System.Collections.Generic;
using StrixMusic.Core.Dummy.Implementations;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Models
{
    /// <summary>
    /// A generic collection of <see cref="DummyAlbum"/>s.
    /// </summary>
    public class AlbumCollection : IPlayableCollectionGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumCollection"/> class.
        /// </summary>
        /// <param name="albums"></param>
        /// <param name="core"></param>
        public AlbumCollection(List<DummyAlbum> albums, DummyCore core)
        {
            SourceCore = core;
            Items = albums;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionBase> Items { get; }

        /// <inheritdoc/>
        public string Id => string.Empty;

        /// <inheritdoc/>
        public string Name => "Albums";

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile? Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState State => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITrack? PlayingTrack => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore { get; }

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
