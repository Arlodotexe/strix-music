using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using OwlCore.Extensions;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc cref="IAlbum"/>
    public class MusicBrainzAlbum : IAlbum
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelpersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="release">The release to wrap around.</param>
        /// <param name="medium">The physical medium (album) for this release.</param>
        /// <param name="artist">A fully constructed <see cref="MusicBrainzArtist"/>.</param>
        /// <remarks>
        /// Normally we don't pass in a fully constructed implementation of one of our classes,
        /// instead opting for passing API-level information around,
        /// but the TotalTracksCount property needs to be filled in before object creation, and to get that info we must make asynchronous API calls.
        /// To follow convention of Strix Music's Sdk interfaces, we're asynchronously filling that info in before creating the album,
        /// so it's available as soon as soon as the album is.
        /// </remarks>
        public MusicBrainzAlbum(ICore sourceCore, Release release, Medium medium, MusicBrainzArtist artist)
        {
            _musicBrainzClient = sourceCore.GetService<MusicBrainzClient>();
            _artistHelpersService = sourceCore.GetService<MusicBrainzArtistHelpersService>();

            Release = release;
            Medium = medium;
            Tracks = new SynchronizedObservableCollection<ITrack>();
            Images = CreateImagesForRelease();

            SourceCore = sourceCore;
            Artist = artist;
        }

        /// <summary>
        /// The physical medium (album) for this release.
        /// </summary>
        /// <remarks>In MusicBrainz, a <see cref="Release"/> can contain multiple physical mediums. Only one of these <see cref="Medium"/> should be used per Album.</remarks>
        public Medium Medium { get; }

        /// <summary>
        /// The <see cref="Release"/> for this album.
        /// </summary>
        public Release Release { get; }

        /// <inheritdoc/>
        public IArtist Artist { get; }

        /// <inheritdoc/>
        public int TotalTracksCount => Medium.TrackCount;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id => Release.Id;

        /// <inheritdoc/>
        public Uri? Url => new Uri($"https://musicbrainz.org/release/{Id}");

        /// <inheritdoc/>
        public string Name => $"{Release.Title} {(Release.Media.Count == 1 ? string.Empty : $"({Medium.Format} {Medium.Position})")}";

        /// <inheritdoc/>
        public DateTime? DatePublished => CreateReleaseDate(Release.Date);

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc/>
        public string? Description => Release.TextRepresentation?.Script;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration { get; } = TimeSpan.Zero;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; } = new SynchronizedObservableCollection<string>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveGenreSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveTrackSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<DateTime?>? DatePublishedChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

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
        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
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
        public async IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset)
        {
            var releaseData = await _musicBrainzClient.Releases.GetAsync(Id, RelationshipQueries.Releases);

            var artist = new MusicBrainzArtist(SourceCore, Release.Credits[0].Artist)
            {
                TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(Release.Credits[0].Artist),
            };

            foreach (var recording in releaseData.Media.First(x => x.Position == Medium.Position).Tracks.GetRange(offset, limit))
            {
                // Iterate through each physical medium for this release.
                foreach (var medium in Release.Media)
                {
                    // Iterate the tracks and find a matching ID for this recording
                    foreach (var trackData in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                    {
                        var album = new MusicBrainzAlbum(SourceCore, Release, medium, artist);
                        var track = new MusicBrainzTrack(SourceCore, trackData, album);

                        yield return track;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task PopulateMoreTracksAsync(int limit)
        {
            var offset = Tracks.Count;
            await foreach (var item in GetTracksAsync(limit, offset))
            {
                IsRemoveTrackSupportedMap.Add(false);
                Tracks.Add(item);
            }
        }

        private DateTime CreateReleaseDate(string musicBrainzDate)
        {
            var dateParts = musicBrainzDate.Split('-');

            var date = default(DateTime);

            foreach (var (item, index) in dateParts.Select((value, index) => (value, index)))
            {
                date = index switch
                {
                    0 => date.ChangeYear(Convert.ToInt32(item)),
                    1 => date.ChangeMonth(Convert.ToInt32(item)),
                    2 => date.ChangeDay(Convert.ToInt32(item)),
                    _ => date
                };
            }

            return date;
        }

        private SynchronizedObservableCollection<IImage> CreateImagesForRelease()
        {
            var returnData = new SynchronizedObservableCollection<IImage>();

            foreach (var item in (MusicBrainzImageSize[])Enum.GetValues(typeof(MusicBrainzImageSize)))
            {
                returnData.Add(new MusicBrainzImage(Release.Id, item));
            }

            return returnData;
        }
    }
}
