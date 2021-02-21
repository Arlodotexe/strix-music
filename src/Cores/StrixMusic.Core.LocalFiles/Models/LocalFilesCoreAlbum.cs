using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreAlbum"/>.
    /// </summary>
    public class LocalFilesCoreAlbum : ICoreAlbum
    {
        private readonly AlbumMetadata _albumMetadata;
        private readonly LocalFilesCoreImage? _image;
        private readonly IFileMetadataManager _fileMetadataManager;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="albumMetadata">The source album metadata to wrap around.</param>
        /// <param name="totalTracksCount">The total number of tracks in this album.</param>
        /// <param name="image">A cover image to use with this album, if applicable.</param>
        public LocalFilesCoreAlbum(ICore sourceCore, AlbumMetadata albumMetadata, int totalTracksCount, LocalFilesCoreImage? image)
        {
            SourceCore = sourceCore;
            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();

            _albumMetadata = albumMetadata;
            _image = image;
            TotalImageCount = image == null ? 0 : 1;
            TotalArtistItemsCount = _albumMetadata.TotalTracksCount ?? 0;
            TotalTracksCount = totalTracksCount;
            Genres = new SynchronizedObservableCollection<string>(albumMetadata.Genres);
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
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

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
        public int TotalTracksCount { get; private set; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id => _albumMetadata.Id ?? throw new InvalidOperationException($"Missing {nameof(_albumMetadata.Id)}");

        /// <inheritdoc/>
        public Uri? Url => null;

        /// <inheritdoc/>
        public string Name => _albumMetadata.Title ?? "No Title";

        /// <inheritdoc/>
        public DateTime? DatePublished => _albumMetadata.DatePublished;

        /// <inheritdoc/>
        public string? Description => _albumMetadata.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc/>
        public TimeSpan Duration => _albumMetadata.Duration ?? new TimeSpan(0, 0, 0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public int TotalImageCount { get; }

        /// <inheritdoc />
        public int TotalArtistItemsCount { get; private set; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; } = new SynchronizedObservableCollection<string>();

        /// <inheritdoc/>
        public bool IsPlayTrackCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseTrackCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPlayArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
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
        public Task PauseArtistCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates the number of tracks for <see cref="LocalFilesCoreAlbum"/>.
        /// </summary>
        /// <param name="newTrackCount">The new count.</param>
        public void ChangeTotalTrackCount(int newTrackCount)
        {
            TotalTracksCount = newTrackCount;

            TrackItemsCountChanged?.Invoke(this,TotalTracksCount);
        }

        /// <summary>
        /// Updates the number of artists for <see cref="LocalFilesCoreArtist"/>.
        /// </summary>
        /// <param name="newArtistCount">The new count.</param>
        public void ChangeTotalArtistCount(int newArtistCount)
        {
            TotalArtistItemsCount = newArtistCount;

            ArtistItemsCountChanged?.Invoke(this,TotalArtistItemsCount);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            var tracksList = _fileMetadataManager.Tracks;

            var tracks = await tracksList.GetTracksByAlbumId(Id, offset, limit);

            foreach (var track in tracks.OrderBy(c => c.TrackNumber))
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
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            if (_image != null)
                yield return _image;

            await Task.CompletedTask;
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
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artistRepository = _fileMetadataManager.Artists;

            var artists = await artistRepository.GetArtistsByAlbumId(Id, offset, limit);

            foreach (var artist in artists)
            {
                if (artist.ImagePath != null)
                    yield return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0, new LocalFilesCoreImage(SourceCore, artist.ImagePath));

                yield return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0, null);
            }
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }
    }
}
