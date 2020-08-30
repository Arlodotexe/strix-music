using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc />
    public class MockTrack : ITrack
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("title")]
        public string TitleStr { get; set; } = string.Empty;

        /// <inheritdoc/>
        public TrackType Type => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => throw new NotImplementedException();

        /// <inheritdoc/>
        public IAlbum Album => throw new NotImplementedException();

        /// <inheritdoc/>
        public DateTime? DatePublished => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<string> Genres => throw new NotImplementedException();

        /// <inheritdoc/>
        public int? TrackNumber => throw new NotImplementedException();

        /// <inheritdoc/>
        public int? PlayCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public CultureInfo Language => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILyrics Lyrics => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsExplicit => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalRelatedItemsCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged;

        /// <inheritdoc/>
        public event EventHandler<IAlbum> AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<DateTime?> DatePublishedChanged;

        /// <inheritdoc/>
        public event EventHandler<int?> TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<int?> PlayCountChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo> LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ILyrics> LyricsChanged;

        /// <inheritdoc/>
        public event EventHandler<bool> IsExplicitChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string> NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string> DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri> UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
