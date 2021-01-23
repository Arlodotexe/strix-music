using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Models
{
    ///NOTE: There are some methods set to NotSupported temporarily although they are supported, so the playback can be implemented.
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreAlbum"/>.
    /// </summary>
    public class LocalFilesCoreAlbum : ICoreAlbum
    {
        private AlbumMetadata _albumMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        public LocalFilesCoreAlbum(ICore sourceCore, AlbumMetadata albumMetadata, int totalTracksCount)
        {
            SourceCore = sourceCore;
            _albumMetadata = albumMetadata;
            TotalArtistItemsCount = _albumMetadata.TotalTracksCount ?? 0;
            TotalTracksCount = totalTracksCount;
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

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id => _albumMetadata.Id;

        /// <inheritdoc/>
        public Uri? Url => new Uri("http://google.com");

        /// <inheritdoc/>
        public string Name => _albumMetadata.Title ?? "No Title";

        /// <inheritdoc/>
        public DateTime? DatePublished => _albumMetadata.DatePublished;

        /// <inheritdoc/>
        public string? Description => _albumMetadata.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; private set; }

        /// <inheritdoc/>
        public TimeSpan Duration => _albumMetadata.Duration ?? new TimeSpan(0, 0, 0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public int TotalImageCount { get; } = 3;

        /// <inheritdoc />
        public int TotalArtistItemsCount { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; } = new SynchronizedObservableCollection<string>();

        /// <inheritdoc/>
        public bool IsPlayAsyncAvailable => true;

        /// <inheritdoc/>
        public bool IsPauseAsyncAvailable => true;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            PlaybackState = PlaybackState.Paused;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            PlaybackState = PlaybackState.Playing;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            var tracksList = SourceCore.GetService<TrackService>();

            var tracks = await tracksList.GetTracksByAlbumId(Id, offset, limit);

            foreach (var track in tracks)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artistService = SourceCore.GetService<ArtistService>();

            var artists = await artistService.GetArtistsByAlbumId(Id, offset, limit);

            foreach (var artist in artists)
            {
                // just to test
                var tracks = await SourceCore.GetService<TrackService>().GetTracksByAlbumId(artist.Id, 0, 1000);
                yield return new LocalFilesCoreArtist(SourceCore, artist, tracks.Count);
            }
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }
    }
}
