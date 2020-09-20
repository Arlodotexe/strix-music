using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzArtist : IArtist
    {
        private readonly Artist _artist;
        private readonly MusicBrainzClient _musicBrainzClient;
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
            Albums = new ObservableCollection<IAlbum>();
            Tracks = new ObservableCollection<ITrack>();

            _musicBrainzClient = SourceCore.GetService<MusicBrainzClient>();
            _artistHelperService = SourceCore.GetService<MusicBrainzArtistHelpersService>();
        }

        /// <inheritdoc/>
        public string Id => _artist.Id;

        /// <inheritdoc/>
        public ObservableCollection<IAlbum> Albums { get; }

        /// <inheritdoc/>
        public int TotalAlbumsCount => _artist.Releases.Count;

        /// <inheritdoc/>
        public ObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc/>
        public int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri Url => new Uri($"https://musicbrainz.org/artist/{Id}");

        /// <inheritdoc/>
        public string Name => _artist.Name;

        /// <inheritdoc/>
        public ObservableCollection<IImage> Images => new ObservableCollection<IImage>();

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
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public ObservableCollection<string>? Genres { get; }

        /// <inheritdoc/>
        public ObservableCollection<bool> IsRemoveImageSupportedMap { get; } = new ObservableCollection<bool>();

        /// <inheritdoc/>
        public ObservableCollection<bool> IsRemoveTrackSupportedMap { get; } = new ObservableCollection<bool>();

        /// <inheritdoc/>
        public ObservableCollection<bool> IsRemoveAlbumSupportedMap { get; } = new ObservableCollection<bool>();

        /// <inheritdoc/>
        public ObservableCollection<bool> IsRemoveGenreSupportedMap { get; } = new ObservableCollection<bool>();

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

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index)
        {
            throw new NotImplementedException();
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
        public async IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset)
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
                    var album = new MusicBrainzAlbum(SourceCore, release, medium, artist);

                    Albums.Add(album);
                    IsRemoveAlbumSupportedMap.Add(false);
                    yield return album;
                }
            }
        }

        /// <inheritdoc/>
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            var offset = Albums.Count;
            await foreach (var item in GetAlbumsAsync(limit, offset))
            {
                IsRemoveAlbumSupportedMap.Add(false);
                Albums.Add(item);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset)
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
                        foreach (var trackData in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                        {
                            var track = new MusicBrainzTrack(SourceCore, trackData, new MusicBrainzAlbum(SourceCore, release, medium, artist));

                            Tracks.Add(track);
                            IsRemoveTrackSupportedMap.Add(false);
                            yield return track;
                        }
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
    }
}
