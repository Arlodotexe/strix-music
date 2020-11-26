using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzCoreTrack : ICoreTrack
    {
        private readonly Track _track;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelpersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreTrack"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="track">The MusicBrainz track to wrap around.</param>
        /// <param name="musicBrainzCoreAlbum">A fully constructed <see cref="MusicBrainzCoreAlbum"/> that this track belongs to.</param>
        /// <param name="discNumber">The disc number (<see cref="Medium"/>) that this track belongs to.</param>
        /// <remarks>
        /// Normally we don't pass in a fully constructed implementation of one of our classes,
        /// instead opting for passing API-level information around,
        /// but the Album is complex enough that implicitly passing the constructed object is preferred over manually
        /// passing what is needed to create one inside the MusicBrainzTrack.
        /// </remarks>
        public MusicBrainzCoreTrack(ICore sourceCore, Track track, MusicBrainzCoreAlbum musicBrainzCoreAlbum, int discNumber)
        {
            SourceCore = sourceCore;
            _track = track;

            Language = CultureInfoExtensions.FromIso636_3(musicBrainzCoreAlbum.Release?.TextRepresentation?.Language);

            // The genres should come from the release tag list. The API returns it, but the library we use doesn't deserialize it. For now, we have no genres.
            // Genres = musicBrainzAlbum.Release.Tag;
            Album = musicBrainzCoreAlbum;

            DiscNumber = discNumber;

            _musicBrainzClient = SourceCore.GetService<MusicBrainzClient>();
            _artistHelpersService = SourceCore.GetService<MusicBrainzArtistHelpersService>();
        }

        /// <inheritdoc/>
        public event EventHandler<ICoreAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ICoreLyrics?>? LyricsChanged;

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
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public string Id => _track.Id;

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _track.Recording.Credits?.Count ?? 0;

        /// <inheritdoc />
        public int TotalImageCount { get; } = 0;

        /// <inheritdoc/>
        public ICoreAlbum? Album { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres => new SynchronizedObservableCollection<string>();

        /// <inheritdoc/>
        /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
        public int? TrackNumber => _track.Position;

        /// <inheritdoc />
        public int? DiscNumber { get; }

        /// <inheritdoc/>
        public CultureInfo? Language { get; }

        /// <inheritdoc/>
        public ICoreLyrics? Lyrics => null;

        /// <inheritdoc/>
        public bool IsExplicit => _track.Recording.Disambiguation.Contains("explicit");

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri? Url => new Uri($"https://musicbrainz.org/track/{Id}");

        /// <inheritdoc/>
        public string Name => _track.Recording.Title;

        /// <inheritdoc/>
        public string? Description => null;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration => _track.Length != null
                ? TimeSpan.FromMilliseconds((double)_track.Length)
                : TimeSpan.Zero;

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncSupported => false;

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
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(ICoreAlbum? albums)
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
        public Task ChangeLyricsAsync(ICoreLyrics? lyrics)
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

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var recording = await _musicBrainzClient.Recordings.GetAsync(Id, RelationshipQueries.Recordings);

            foreach (var item in recording.Credits)
            {
                var totalTracksCount = await _artistHelpersService.GetTotalTracksCount(item.Artist);
                yield return new MusicBrainzCoreArtist(SourceCore, item.Artist, totalTracksCount);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
