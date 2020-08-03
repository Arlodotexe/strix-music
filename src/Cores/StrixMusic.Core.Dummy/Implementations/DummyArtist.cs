using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public IList<IAlbum> Albums { get; }

        /// <summary>
        /// List of full <see cref="DummyAlbum"/>s to be used within the DummyCore.
        /// </summary>
        public List<DummyAlbum> DummyAlbums { get; set; }

        /// <summary>
        /// List of the Ids of <see cref="DummyAlbum"/>s to the <see cref="DummyArtist"/>
        /// </summary>
        [JsonProperty("album_ids")]
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
