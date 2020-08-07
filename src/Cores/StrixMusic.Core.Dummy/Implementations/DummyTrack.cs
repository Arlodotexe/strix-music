using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyTrack : ITrack
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Type => "song";

        /// <inheritdoc/>
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IArtist Artist => DummyArtist!;

        /// <summary>
        /// Full <see cref="DummyArtist"/> to be used within the DummyCore.
        /// </summary>
        public DummyArtist? DummyArtist { get; set; }

        /// <summary>
        /// The Id of the <see cref="DummyArtist"/>.
        /// </summary>
        [JsonProperty("artist_id")]
        public string ArtistId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IAlbum Album => DummyAlbum!;

        /// <summary>
        /// Full <see cref="DummyAlbum"/> to be used within the DummyCore.
        /// </summary>
        public DummyAlbum? DummyAlbum { get; set; }

        /// <summary>
        /// The Id of the <see cref="DummyAlbum"/>.
        /// </summary>
        [JsonProperty("album_id")]
        public string AlbumId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public DateTime? DatePublished => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<string> Genre => throw new NotImplementedException();

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
        public TimeSpan Duration { get; }

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore Core => DummyCore!;

        /// <summary>
        /// The <see cref="DummyCore"/> where the <see cref="DummyTrack"/> is from.
        /// </summary>
        public DummyCore? DummyCore { get; set; }
    }
}
