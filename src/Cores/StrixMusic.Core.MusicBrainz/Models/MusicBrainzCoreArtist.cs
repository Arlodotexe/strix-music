using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="ICoreArtist"/>.
    /// </summary>
    public class MusicBrainzCoreArtist : ICoreArtist
    {
        private readonly Artist _artist;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelperService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreArtist"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="artist">The MusicBrainz artist to wrap.</param>
        /// <param name="totalTracksCount"><inheritdoc cref="TotalTracksCount"/></param>
        public MusicBrainzCoreArtist(ICore sourceCore, Artist artist, int totalTracksCount)
        {
            SourceCore = sourceCore;
            TotalTracksCount = totalTracksCount;

            _artist = artist;

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

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc/>
        public string Id => _artist.Id;

        /// <inheritdoc/>
        public int TotalAlbumItemsCount => _artist.Releases?.Count ?? 0;

        /// <inheritdoc />
        public int TotalImageCount { get; } = 0;

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri Url => new Uri($"https://musicbrainz.org/artistViewModel/{Id}");

        /// <inheritdoc/>
        public string Name => _artist.Name;

        /// <inheritdoc/>
        public string Description => _artist.SortName;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc/>
        public TimeSpan Duration => TimeSpan.Zero;

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems => null;

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
        public Task<bool> IsAddAlbumItemSupported(int index)
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
        public Task<bool> IsRemoveAlbumItemSupported(int index)
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
        public async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            var releasesList = await _musicBrainzClient.Releases.BrowseAsync("artist", Id, limit, offset);

            var releases = releasesList.Items;

            foreach (var release in releases)
            {
                yield return new MusicBrainzCoreAlbum(SourceCore, release, this);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
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

                    var artist = new MusicBrainzCoreArtist(SourceCore, release.Credits[0].Artist, totalTracksForArtist);
                    var album = new MusicBrainzCoreAlbum(SourceCore, release, artist);

                    // Iterate through each physical medium for this release.
                    foreach (var medium in release.Media)
                    {
                        // Iterate the tracks and find a matching ID for this recording
                        foreach (var trackData in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                        {
                            if (trackData != null)
                            {
                                yield return new MusicBrainzCoreTrack(SourceCore, trackData, album, medium.Position);
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
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

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
