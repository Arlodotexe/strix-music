using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Extensions;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzTrack : ITrack
    {
        private readonly Track _track;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IArtist> _artists;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzTrack"/> class.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="musicBrainzAlbum"></param>
        /// <param name="sourceCore"></param>
        /// <remarks>
        /// Normally we don't pass in a fully constructed implementation of one of our classes,
        /// instead opting for passing API-level information around,
        /// but the Album is complex enough that implicitly passing the constructed object is preferred over manually
        /// passing what is needed to create one inside the MusicBrainzTrack.
        /// </remarks>
        public MusicBrainzTrack(ICore sourceCore, Track track, MusicBrainzAlbum musicBrainzAlbum)
        {
            SourceCore = sourceCore;
            _track = track;
            _artists = new List<IArtist>();

            Language = CultureInfoExtensions.FromIso636_3(musicBrainzAlbum.Release.TextRepresentation.Language);
            Album = musicBrainzAlbum;

            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _artists;

        /// <inheritdoc />
        public int TotalArtistsCount => _track.Recording.Credits.Count;

        /// <inheritdoc/>
        public IAlbum Album { get; }

        /// <inheritdoc/>
        public IReadOnlyList<string> Genres => throw new NotImplementedException();

        /// <inheritdoc/>
        /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
        public int? TrackNumber => _track.Position;

        /// <inheritdoc/>
        public int? PlayCount => null;

        /// <inheritdoc/>
        public CultureInfo Language { get; }

        /// <inheritdoc/>
        public ILyrics? Lyrics => null;

        /// <inheritdoc/>
        public bool IsExplicit => _track.Recording.Disambiguation.Contains("explicit");

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri? Url => new Uri($"https://musicbrainz.org/track/{Id}");

        /// <inheritdoc/>
        public string Name => _track.Recording.Title;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => CreateImagesForRelease(_track.Recording.Releases);

        /// <inheritdoc/>
        public string? Description => null;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration => _track.Length != null
                ? TimeSpan.FromMilliseconds((double)_track.Length)
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
        public string Id => _track.Id;

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

        private static IReadOnlyList<IImage> CreateImagesForRelease(IEnumerable<Release> releases)
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
