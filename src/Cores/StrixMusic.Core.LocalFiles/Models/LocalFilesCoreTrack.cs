using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc />
    public class LocalFilesCoreTrack : ICoreTrack
    {
        private readonly TrackMetadata _trackMetadata;
        private readonly IFileMetadataManager _fileMetadataManager;
        private int _totalArtistItemsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreTrack"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="trackMetadata">The track metadata to wrap around</param>
        public LocalFilesCoreTrack(ICore sourceCore, TrackMetadata trackMetadata)
        {
            SourceCore = sourceCore;
            _trackMetadata = trackMetadata;
            Genres = new SynchronizedObservableCollection<string>(trackMetadata.Genres);

            if (trackMetadata.ImagePath != null)
                TotalImageCount = 1;

            _totalArtistItemsCount = trackMetadata.ArtistIds?.Count ?? 0;

            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();
            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataManager.FileMetadataUpdated += FileMetadataManager_FileMetadataUpdated;
        }

        private void FileMetadataManager_FileMetadataUpdated(object sender, FileMetadata e)
        {
            if (e.TrackMetadata?.Id == Id)
                return;

            if (e.ArtistMetadata == null)
                return;

            TotalArtistItemsCount++;

            Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata.Id, nameof(e.ArtistMetadata.Id));

            var localFilesCoreArtist = InstanceCache.Artists.GetOrCreate(
                e.ArtistMetadata.Id, 
                SourceCore,
                e.ArtistMetadata,
                e.ArtistMetadata.TrackIds?.Count ?? 0,
                e.ArtistMetadata.ImagePath == null ? null : InstanceCache.Images.GetOrCreate(e.ArtistMetadata.Id, SourceCore, e.ArtistMetadata.ImagePath));

            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>
            {
                new CollectionChangedItem<ICoreArtistCollectionItem>(localFilesCoreArtist, TotalArtistItemsCount - 1),
            };

            var removedItems = Array.Empty<CollectionChangedItem<ICoreArtistCollectionItem>>();

            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc/>
        public event EventHandler<ICoreAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ICoreLyrics?>? LyricsChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsExplicitChanged;

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
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public string Id => _trackMetadata.Id ?? string.Empty;

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc />
        public int TotalArtistItemsCount
        {
            get => _totalArtistItemsCount;
            private set
            {
                _totalArtistItemsCount = value;
                ArtistItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        public int TotalImageCount { get; }

        /// <inheritdoc/>
        public ICoreAlbum? Album { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; }

        /// <inheritdoc/>
        /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
        public int? TrackNumber => Convert.ToInt32(_trackMetadata.TrackNumber);

        /// <inheritdoc />
        public int? DiscNumber { get; }

        /// <inheritdoc/>
        public CultureInfo? Language { get; }

        /// <inheritdoc/>
        public ICoreLyrics? Lyrics => null;

        /// <inheritdoc/>
        public bool IsExplicit => false;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <summary>
        /// Image uri for <see cref="LocalFilesCoreTrack"/>
        /// </summary>
        public Uri? ImageUri => _trackMetadata.ImagePath;

        /// <summary>
        /// The path to the playable music file on disk.
        /// </summary>
        public Uri? LocalTrackPath => _trackMetadata.Source;

        /// <inheritdoc/>
        public Uri? Url => null;

        /// <inheritdoc/>
        public string Name => _trackMetadata.Title ?? string.Empty;

        /// <inheritdoc/>
        public string? Description => _trackMetadata.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc/>
        public TimeSpan Duration => _trackMetadata.Duration ?? new TimeSpan(0, 0, 0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPlayArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(ICoreAlbum? albums)
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
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ICoreLyrics? lyrics)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
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

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index)
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

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artistRepo = _fileMetadataManager.Artists;

            var artists = await artistRepo.GetArtistsByTrackId(Id, offset, limit);

            foreach (var artist in artists)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));

                if (artist.ImagePath != null)
                {
                    yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist, artist.TrackIds?.Count ?? 0, InstanceCache.Images.GetOrCreate(artist.Id, SourceCore, artist.ImagePath));
                }

                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist, artist.TrackIds?.Count ?? 0);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            if (ImageUri != null)
                yield return InstanceCache.Images.GetOrCreate(Id, SourceCore, ImageUri, 250, 250);

            await Task.CompletedTask;
        }
    }
}
