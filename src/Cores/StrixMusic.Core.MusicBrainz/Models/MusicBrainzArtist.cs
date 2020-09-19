using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzArtist : IArtist
    {
        private readonly Artist _artist;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IAlbum> _albums;
        private readonly List<ITrack> _tracks;
        private readonly MusicBrainzArtistHelpersService _artistHelperService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzArtist"/> class.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="sourceCore"></param>
        public MusicBrainzArtist(ICore sourceCore, Artist artist)
        {
            SourceCore = sourceCore;
            _artist = artist;
            _albums = new List<IAlbum>();
            _tracks = new List<ITrack>();
            _musicBrainzClient = SourceCore.GetService<MusicBrainzClient>();
            _artistHelperService = SourceCore.GetService<MusicBrainzArtistHelpersService>();
        }

        /// <inheritdoc/>
        public string Id => _artist.Id;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _artist.Releases.Count;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _tracks;

        /// <inheritdoc/>
        public int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri Url => new Uri($"https://musicbrainz.org/artist/{Id}");

        /// <inheritdoc/>
        public string Name => _artist.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => new List<IImage>();

        /// <inheritdoc/>
        public string Description => _artist.SortName;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration => TimeSpan.Zero;

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
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
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
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
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

        /// <inheritdoc/>
        public async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset)
        {
            var releasesList = await _musicBrainzClient.Releases.BrowseAsync("artist", Id, limit, offset);

            var releases = releasesList.Items;

            foreach (var release in releases)
            {
                var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                {
                    TotalTracksCount = await _artistHelperService.GetTotalTracksCount(release.Credits[0].Artist),
                };

                foreach (var medium in release.Media)
                {
                    _albums.Add(new MusicBrainzAlbum(SourceCore, release, medium, artist));
                }
            }

            return _albums;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset)
        {
            var recordings = await _musicBrainzClient.Recordings.BrowseAsync("artist", Id, limit, offset, RelationshipQueries.Recordings);

            var releasesList =
                await _musicBrainzClient.Releases.BrowseAsync("artist", Id, 100, 0, RelationshipQueries.Releases);

            var releases = releasesList.Items.ToList();

            for (var i = 100; i < releases.Count; i += 100)
            {
                var nextReleasesPage = await _musicBrainzClient.Releases.BrowseAsync("artist", Id, 100, 0, RelationshipQueries.Releases);

                releases.AddRange(nextReleasesPage.Items);
            }

            foreach (var recording in recordings.Items)
            {
                foreach (var release in releases)
                {
                    var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                    {
                        TotalTracksCount = await _artistHelperService.GetTotalTracksCount(release.Credits[0].Artist),
                    };

                    // Iterate through each physical medium for this release.
                    foreach (var medium in release.Media)
                    {
                        // Iterate the tracks and find a matching ID for this recording
                        foreach (var track in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                        {
                            _tracks.Add(new MusicBrainzTrack(SourceCore, track, new MusicBrainzAlbum(SourceCore, release, medium, artist)));
                        }
                    }
                }
            }

            return _tracks;
        }
    }
}
