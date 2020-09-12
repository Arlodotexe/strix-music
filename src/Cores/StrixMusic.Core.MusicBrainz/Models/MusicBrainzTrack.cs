using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzTrack : ITrack
    {
        private readonly Recording _recording;
        private readonly IAlbum _album;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IArtist> _artists;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzTrack"/> class.
        /// </summary>
        /// <param name="recording"></param>
        /// <param name="sourceCore"></param>
        public MusicBrainzTrack(ICore sourceCore, Recording recording)
        {
            SourceCore = sourceCore;
            _recording = recording;
            _album = new MusicBrainzAlbum(sourceCore, recording.Releases[0]);
            _artists = new List<IArtist>();

            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc/>
        public TrackType Type => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _artists;

        /// <inheritdoc />
        public int TotalArtistsCount => _recording.Credits.Count;

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
        public string Name => _recording.Title;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => _recording.Title;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => throw new NotImplementedException();

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
        public string Id => _recording.Id;

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
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeArtistsAsync(IReadOnlyList<IArtist>? artists)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeGenresAsync(IReadOnlyList<string>? genres)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ILyrics? lyrics)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var recording = await _musicBrainzClient.Recordings.GetAsync(Id, "artist-credits");

            foreach (var item in recording.Credits)
            {
                _artists.Add(new MusicBrainzArtist(SourceCore, item.Artist));
            }

            return _artists;
        }
    }
}
