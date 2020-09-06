using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API.Entities;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzTrack : ITrack
    {
        private readonly Recording _recording;
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzTrack"/> class.
        /// </summary>
        /// <param name="recording"></param>
        /// <param name="sourceCore"></param>
        public MusicBrainzTrack(Recording recording, ICore sourceCore, IAlbum album)
        {
            SourceCore = sourceCore;
            _recording = recording;
            _album = album;
        }

        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("title")]
        public string TitleJson { get; set; } = string.Empty;


        /// <inheritdoc/>
        public TrackType Type => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => throw new NotImplementedException();

        /// <inheritdoc/>
        public IAlbum Album => _album;

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
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => TitleJson;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeArtistsAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeGenresAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<string>>? GenresChanged;

        /// <inheritdoc/>
        public event EventHandler<IAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<DateTime?>? DatePublishedChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? PlayCountChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ILyrics?>? LyricsChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsExplicitChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(IAlbum? albums)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeArtistsAsync(IReadOnlyList<IArtist>? artists)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeGenresAsync(IReadOnlyList<string>? genres)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ILyrics? lyrics)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            throw new NotImplementedException();
        }

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
    }
}
