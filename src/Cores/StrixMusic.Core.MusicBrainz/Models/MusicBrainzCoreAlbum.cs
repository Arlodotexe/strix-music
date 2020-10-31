using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using OwlCore.Extensions;
using StrixMusic.Core.MusicBrainz.Models.Enums;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="ICoreAlbum"/>.
    /// </summary>
    public class MusicBrainzCoreAlbum : ICoreAlbum
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelpersService;
        private readonly MusicBrainzCoreArtist _coreArtist;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="release">The release to wrap around.</param>
        /// <param name="coreArtist">A fully constructed <see cref="MusicBrainzCoreArtist"/>.</param>
        /// <remarks>
        /// Normally we don't pass in a fully constructed implementation of one of our classes,
        /// instead opting for passing API-level information around,
        /// but the TotalTracksCount property needs to be filled in before object creation, and to get that info we must make asynchronous API calls.
        /// To follow convention of Strix Music's Sdk interfaces, we're asynchronously filling that info in before creating the album,
        /// so it's available as soon as soon as the album is.
        /// </remarks>
        public MusicBrainzCoreAlbum(ICore sourceCore, Release release, MusicBrainzCoreArtist coreArtist)
        {
            _musicBrainzClient = sourceCore.GetService<MusicBrainzClient>();
            _artistHelpersService = sourceCore.GetService<MusicBrainzArtistHelpersService>();

            if (release.Media != null)
            {
                TotalTracksCount = release.Media.Select(x => x.TrackCount).Sum();
            }

            Release = release;
            Images = CreateImagesForRelease();

            SourceCore = sourceCore;
            _coreArtist = coreArtist;
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

        /// <summary>
        /// The <see cref="Release"/> for this album.
        /// </summary>
        public Release Release { get; }

        /// <inheritdoc/>
        public ICoreArtist Artist => _coreArtist;

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id => Release.Id;

        /// <inheritdoc/>
        public Uri? Url => new Uri($"https://musicbrainz.org/release/{Id}");

        /// <inheritdoc/>
        public string Name => Release.Title;

        /// <inheritdoc/>
        public DateTime? DatePublished => CreateReleaseDate(Release?.Date);

        /// <inheritdoc/>
        public SynchronizedObservableCollection<ICoreImage> Images { get; }

        /// <inheritdoc/>
        public string? Description => Release.TextRepresentation?.Script;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration { get; } = TimeSpan.Zero;

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; } = new SynchronizedObservableCollection<string>();

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

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index)
        {
            return Task.FromResult(false);
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
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            var releaseData = await _musicBrainzClient.Releases.GetAsync(Id, RelationshipQueries.Releases);

            Artist? releaseArtist = Release.Credits?[0].Artist;

            if (releaseArtist != null && releaseArtist.Id != _coreArtist.Id)
            {
                var sourceArtist = Release.Credits.FirstOrDefault().Artist;

                if (sourceArtist is null)
                    yield break;

                var totalTrackCountForArtist = await _artistHelpersService.GetTotalTracksCount(sourceArtist);

                // TODO: ???
                var artist = new MusicBrainzCoreArtist(SourceCore, sourceArtist, totalTrackCountForArtist);
            }

            // Get full list of tracks from all sources
            IEnumerable<Track> recordingList = releaseData.Media.SelectMany(x => x.Tracks);

            // limit + offset cannot be greater than the number of items in the list
            var enumerable = recordingList as Track[] ?? recordingList.ToArray();
            limit = Math.Min(limit, enumerable.Count() - offset);

            foreach (var recording in enumerable.ToList().GetRange(offset, limit))
            {
                var album = new MusicBrainzCoreAlbum(SourceCore, Release, _coreArtist);
                var track = new MusicBrainzCoreTrack(SourceCore, recording, album, recording.Position);

                yield return track;

                // TODO: Check for duplicates between sources
                /*
                // Iterate through each physical medium for this release.
                foreach (var medium in Release.Media)
                {
                    // Iterate the tracks and find a matching ID for this recording
                    foreach (var trackData in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                    {
                        var album = new MusicBrainzAlbum(SourceCore, Release, artistViewModel);
                        var track = new MusicBrainzTrack(SourceCore, trackData, album, medium.Position);

                        yield return track;
                    }
                }*/
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        private DateTime? CreateReleaseDate(string? musicBrainzDate)
        {
            if (string.IsNullOrEmpty(musicBrainzDate) || musicBrainzDate == null)
            {
                return null;
            }

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

        private SynchronizedObservableCollection<ICoreImage> CreateImagesForRelease()
        {
            var returnData = new SynchronizedObservableCollection<ICoreImage>();

            foreach (var item in (MusicBrainzImageSize[])Enum.GetValues(typeof(MusicBrainzImageSize)))
            {
                returnData.Add(new MusicBrainzCoreImage(SourceCore, Release.Id, item));
            }

            return returnData;
        }
    }
}
