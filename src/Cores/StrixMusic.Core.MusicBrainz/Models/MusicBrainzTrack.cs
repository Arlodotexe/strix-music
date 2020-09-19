using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzTrack : ITrack
    {
        private readonly Recording _recording;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IArtist> _artists;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzTrack"/> class.
        /// </summary>
        /// <param name="recording"></param>
        /// <param name="sourceCore"></param>
        public MusicBrainzTrack(ICore sourceCore, Recording recording)
        {
            // TODO: Refactor to pass in Track instead of recording.
            SourceCore = sourceCore;
            _recording = recording;
            _artists = new List<IArtist>();

            var release = recording.Releases.FirstOrDefault(x => x.Status == "Official") ?? recording.Releases.First();

            // TODO: Figure out which medium/disc this track belongs to
            Album = new MusicBrainzAlbum(sourceCore, release, release.Media.First());

            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _artists;

        /// <inheritdoc />
        public int TotalArtistsCount => _recording.Credits.Count;

        /// <inheritdoc/>
        public IAlbum Album { get; }

        /// <inheritdoc/>
        public IReadOnlyList<string> Genres => throw new NotImplementedException();

        /// <inheritdoc/>
        /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
        public int? TrackNumber { get; internal set; }

        /// <inheritdoc/>
        public int? PlayCount => null;

        /// <inheritdoc/>
        public CultureInfo Language => CultureInfo.CreateSpecificCulture("TODO");

        /// <inheritdoc/>
        public ILyrics? Lyrics => null;

        /// <inheritdoc/>
        public bool IsExplicit => _recording.Disambiguation.Contains("explicit");

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri? Url => new Uri($"https://musicbrainz.org/track/{Id}");

        /// <inheritdoc/>
        public string Name => _recording.Title;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => CreateImagesForRelease(_recording.Releases);

        /// <inheritdoc/>
        public string? Description => null;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration => _recording.Length != null
                ? TimeSpan.FromMilliseconds((double)_recording.Length)
                : TimeSpan.Zero;

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsChangeArtistsAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncSupported => false;

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
            var recording = await _musicBrainzClient.Recordings.GetAsync(Id, RelationshipQueries.Recordings);

            foreach (var item in recording.Credits)
            {
                _artists.Add(new MusicBrainzArtist(SourceCore, item.Artist));
            }

            return _artists;
        }

        private IReadOnlyList<IImage> CreateImagesForRelease(IEnumerable<Release> releases)
        {
            var list = new List<IImage>();

            foreach (var release in releases)
            {
                foreach (var item in (MusicBrainzImageSize[])Enum.GetValues(typeof(MusicBrainzImageSize)))
                {
                    list.Add(new MusicBrainzImage(release.Id, item));
                }
            }

            return list;
        }
    }
}
