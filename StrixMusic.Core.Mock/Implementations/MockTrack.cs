using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock.Implementations
{
    public class MockTrack : ITrack
    {
        public string Type => throw new NotImplementedException();

        public IReadOnlyList<IArtist> Artists => throw new NotImplementedException();

        public IAlbum Album => Album;

        [JsonProperty("album_id")]
        public string AlbumId { get; set; }

        public MockAlbum MockAlbum { get; set; }

        public DateTime? DatePublished => throw new NotImplementedException();

        public IReadOnlyList<string> Genres => throw new NotImplementedException();

        public int? TrackNumber => throw new NotImplementedException();

        public int? PlayCount => throw new NotImplementedException();

        public string Language => throw new NotImplementedException();

        public ILyrics Lyrics => throw new NotImplementedException();

        public bool IsExplicit => throw new NotImplementedException();

        public ICore SourceCore => throw new NotImplementedException();

        public string Id => throw new NotImplementedException();

        public Uri Url => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public PlaybackState PlaybackState => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        public int TotalRelatedItemsCount => throw new NotImplementedException();

        TrackType ITrack.Type => throw new NotImplementedException();

        CultureInfo ITrack.Language => throw new NotImplementedException();

        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged;
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged;
        public event EventHandler<IAlbum> AlbumChanged;
        public event EventHandler<DateTime?> DatePublishedChanged;
        public event EventHandler<int?> TrackNumberChanged;
        public event EventHandler<int?> PlayCountChanged;
        public event EventHandler<CultureInfo> LanguageChanged;
        public event EventHandler<ILyrics> LyricsChanged;
        public event EventHandler<bool> IsExplicitChanged;
        public event EventHandler<PlaybackState> PlaybackStateChanged;
        public event EventHandler<string> NameChanged;
        public event EventHandler<string> DescriptionChanged;
        public event EventHandler<Uri> UrlChanged;
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
