using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Services;
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
        private int _totalTracksCount;
        private int _totalArtistCount;

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
            TotalArtistItemsCount = _albumMetadata.TotalArtistsCount ?? 0;
            TotalTracksCount = totalTracksCount;
            Genres = new SynchronizedObservableCollection<string>(albumMetadata.Genres);

            _fileMetadataManager.FileMetadataUpdated += FileMetadataManager_FileMetadataUpdated;
        }

        private void FileMetadataManager_FileMetadataUpdated(object sender, FileMetadata e)
        {
            if (e.AlbumMetadata?.Id != Id)
                return;

            TracksUpdated(e);
            ArtistsUpdated(e);
        }

        private void TracksUpdated(FileMetadata e)
        {
            Guard.IsNotNullOrWhiteSpace(e.TrackMetadata?.Id, nameof(Id));

            var fileCoreTrack = InstanceCache.Tracks.GetOrCreate(e.TrackMetadata.Id, SourceCore, e.TrackMetadata);

            var addedItems = new List<CollectionChangedItem<ICoreTrack>>
                {
                    new CollectionChangedItem<ICoreTrack>(fileCoreTrack, e.TrackMetadata.TrackNumber != null ? (int)e.TrackMetadata.TrackNumber - 1 : 0),
                };

            TrackItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedItem<ICoreTrack>>());
            TotalTracksCount++;
        }

        private void ArtistsUpdated(FileMetadata e)
        {
            TotalArtistItemsCount++;

            if (e.ArtistMetadata?.Id == null)
                return;

            var filesCoreArtist = InstanceCache.Artists.GetOrCreate(
                e.ArtistMetadata.Id,
                SourceCore,
                e.ArtistMetadata,
                e.ArtistMetadata.TrackIds?.Count ?? 0,
                e.ArtistMetadata.ImagePath == null
                    ? null
                    : InstanceCache.Images.GetOrCreate(
                        e.ArtistMetadata.Id,
                        SourceCore,
                        e.ArtistMetadata.ImagePath));

            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>
            {
                new CollectionChangedItem<ICoreArtistCollectionItem>(filesCoreArtist, 0),
            };

            ArtistItemsChanged?.Invoke(
                this,
                addedItems,
                new List<CollectionChangedItem<ICoreArtistCollectionItem>>());
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
        public int TotalTracksCount
        {
            get => _totalTracksCount;
            set
            {
                _totalTracksCount = value;
                TrackItemsCountChanged?.Invoke(this, _totalTracksCount);
            }
        }

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
        public int TotalArtistItemsCount
        {
            get => _totalArtistCount;
            set
            {
                _totalArtistCount = value;
                ArtistItemsCountChanged?.Invoke(this, _totalArtistCount);
            }
        }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; }

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
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Updates the number of tracks for <see cref="LocalFilesCoreAlbum"/>.
        /// </summary>
        /// <param name="newTrackCount">The new count.</param>
        public void ChangeTotalTrackCount(int newTrackCount)
        {
            TotalTracksCount = newTrackCount;

            TrackItemsCountChanged?.Invoke(this, TotalTracksCount);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            var tracksList = _fileMetadataManager.Tracks;

            var tracks = await tracksList.GetTracksByAlbumId(Id, offset, limit);

            foreach (var track in tracks)
            {
                if (track.Id != null)
                    yield return InstanceCache.Tracks.GetOrCreate(track.Id, SourceCore, track);
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
                if (artist.Id != null)
                {
                    yield return InstanceCache.Artists.GetOrCreate(
                        artist.Id,
                        SourceCore,
                        artist,
                        artist.TrackIds?.Count ?? 0,
                        artist.ImagePath == null
                            ? null
                            : InstanceCache.Images.GetOrCreate(artist.Id, SourceCore, artist.ImagePath));
                }
            }
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }
    }
}
