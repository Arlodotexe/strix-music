using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Models.Enums;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="IArtist"/>.
    /// </summary>
    public class MusicBrainzArtist : IArtist
    {
        private readonly Artist _artist;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelperService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzArtist"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="artist">The MusicBrainz artist to wrap.</param>
        /// <param name="totalTracksCount"><inheritdoc cref="TotalTracksCount"/></param>
        public MusicBrainzArtist(ICore sourceCore, Artist artist, int totalTracksCount)
        {
            SourceCore = sourceCore;
            TotalTracksCount = totalTracksCount;

            _artist = artist;
            Images = null!;

            _musicBrainzClient = SourceCore.GetService<MusicBrainzClient>();
            _artistHelperService = SourceCore.GetService<MusicBrainzArtistHelpersService>();
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
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public string Id => _artist.Id;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _artist.Releases.Count;

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri Url => new Uri($"https://musicbrainz.org/artist/{Id}");

        /// <inheritdoc/>
        public string Name => _artist.Name;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc/>
        public string Description => _artist.SortName;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration => TimeSpan.Zero;

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; }

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
        public Task<bool> IsAddTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
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
                var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(release.Credits[0].Artist);
                var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist, totalTracksForArtist);

                yield return new MusicBrainzAlbum(SourceCore, release, artist);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset)
        {
            var recordings = await _musicBrainzClient.Recordings.BrowseAsync("artist", Id, limit, offset, RelationshipQueries.Recordings);

            // This API call will include releases for this artist, with all track and recording data.
            var firstPage = await _musicBrainzClient.Releases.BrowseAsync("artist", Id, 100, 0, RelationshipQueries.Releases);

            var releaseDataForArtist = await OwlCore.Helpers.APIs.GetAllItemsAsync(firstPage.Count, firstPage.Items, async currentOffset =>
            {
                return (await _musicBrainzClient.Releases.BrowseAsync("artist", Id, 100, 0, RelationshipQueries.Releases))?.Items;
            });

            foreach (var recording in recordings.Items)
            {
                foreach (var release in releaseDataForArtist)
                {
                    var totalTracksForArtist =
                    await _artistHelperService.GetTotalTracksCount(release.Credits[0].Artist);

                    var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist, totalTracksForArtist);
                    var album = new MusicBrainzAlbum(SourceCore, release, artist);

                    // Iterate through each physical medium for this release.
                    foreach (var medium in release.Media)
                    {
                        // Iterate the tracks and find a matching ID for this recording
                        foreach (var trackData in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                        {
                            if (trackData != null)
                            {
                                yield return new MusicBrainzTrack(SourceCore, trackData, album, medium.Position);
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddAlbumAsync(IAlbum album, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumAsync(int index)
        {
            throw new NotImplementedException();
        }
    }
}
