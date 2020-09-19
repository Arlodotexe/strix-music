using System;
using System.Collections.Generic;
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
    /// <inheritdoc cref="IAlbum"/>
    public class MusicBrainzAlbum : IAlbum
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<ITrack> _tracks;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="release">The release to wrap around.</param>
        /// <param name="medium">The physical medium (album) for this release.</param>
        public MusicBrainzAlbum(ICore sourceCore, Release release, Medium medium)
        {
            _musicBrainzClient = sourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();

            Release = release;
            Medium = medium;
            _tracks = new List<ITrack>();
            Images = CreateImagesForRelease();

            SourceCore = sourceCore;
            Artist = new MusicBrainzArtist(SourceCore, release.Relations[0].Artist);
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
        public IReadOnlyList<IImage> Images { get; }

        /// <inheritdoc/>
        public string? Description => Release.TextRepresentation?.Script;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _tracks;

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <summary>
        /// Fires when <see cref="ITrackCollection.Tracks"/> changes.
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

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
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

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
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
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
        public async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset)
        {
            var releaseData = await _musicBrainzClient.Releases.GetAsync(Id, RelationshipQueries.Releases);

            foreach (var recording in releaseData.Media.First(x => x.Position == Medium.Position).Tracks)
            {
                // Iterate through each physical medium for this release.
                foreach (var medium in Release.Media)
                {
                    // Iterate the tracks and find a matching ID for this recording
                    foreach (var track in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                    {
                        _tracks.Add(new MusicBrainzTrack(SourceCore, track, new MusicBrainzAlbum(SourceCore, Release, medium)));
                    }
                }
            }

            return _tracks;
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

        private IReadOnlyList<IImage> CreateImagesForRelease()
        {
            var list = new List<IImage>();

            foreach (var item in (MusicBrainzImageSize[])Enum.GetValues(typeof(MusicBrainzImageSize)))
            {
                list.Add(new MusicBrainzImage(Release.Id, item));
            }

            return list;
        }
    }
}
