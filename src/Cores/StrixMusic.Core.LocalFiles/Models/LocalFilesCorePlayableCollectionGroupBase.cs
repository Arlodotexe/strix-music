using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Events;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.MetadataScanner;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc/>
    public abstract class LocalFilesCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup, IAsyncInit
    {
        private FileMetadataScanner _fileMetadataScanner;
        private IList<ArtistMetadata>? _artistMetadatas;
        private IList<AlbumMetadata>? _albumMetadatas;
        private IList<TrackMetadata>? _trackMetadatas;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected LocalFilesCorePlayableCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;

            _artistMetadatas = new List<ArtistMetadata>();
            _albumMetadatas = new List<AlbumMetadata>();
            _trackMetadatas = new List<TrackMetadata>();
        }

        private void MetadataScanner_RelatedMetadataChanged(object sender, Backing.Models.RelatedMetadata e)
        {
            // Its not complete yet, some data is forcefully given for testing.

            LocalFilesCoreAlbum fileCoreAlbum;
            LocalFilesCoreTrack filesCoreTrack;
            LocalFilesCoreArtist filesCoreArtist;

            if (e.AlbumMetadata != null)
            {
                if (!_albumMetadatas?.Any(c => c.Title?.Contains(e.AlbumMetadata.Title) ?? false) ?? false)
                {
                    fileCoreAlbum = new LocalFilesCoreAlbum(SourceCore, e.AlbumMetadata, 1000); // track count is temporary

                    var addedItems = new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>
                      {
                            new CollectionChangedEventItem<ICoreAlbumCollectionItem>(fileCoreAlbum, 0),
                      };

                    _albumMetadatas?.Add(e.AlbumMetadata);
                    AlbumItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>()); // nothing is being removed for now.
                }
            }

            if (e.ArtistMetadata != null)
            {
                if (!_artistMetadatas?.Any(c => c.Name?.Contains(e.ArtistMetadata.Name) ?? false) ?? false)
                {
                    filesCoreArtist = new LocalFilesCoreArtist(SourceCore, e.ArtistMetadata, 1000); // track count is temporary

                    var addedItems = new List<CollectionChangedEventItem<ICoreArtistCollectionItem>>
                    {
                        new CollectionChangedEventItem<ICoreArtistCollectionItem>(filesCoreArtist, 0),
                    };

                    _artistMetadatas?.Add(e.ArtistMetadata);
                    ArtistItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreArtistCollectionItem>>());
                }
            }

            if (e.TrackMetadata != null)
            {
                if (!_trackMetadatas?.Contains(e.TrackMetadata) ?? false)
                {
                    filesCoreTrack = new LocalFilesCoreTrack(SourceCore, e.TrackMetadata);

                    var addedItems = new List<CollectionChangedEventItem<ICoreTrack>>
                {
                    new CollectionChangedEventItem<ICoreTrack>(filesCoreTrack, 0),
                };

                    _trackMetadatas?.Add(e.TrackMetadata);
                    TrackItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreTrack>>());  // nothing is being removed for now.
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />?
        public event EventHandler<int>? TotalChildrenCountChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public abstract string Id { get; protected set; }

        /// <inheritdoc />
        public abstract Uri? Url { get; protected set; }

        /// <inheritdoc />
        public abstract string Name { get; protected set; }

        /// <inheritdoc />
        public abstract string? Description { get; protected set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc />
        public TimeSpan Duration => TimeSpan.Zero;

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public abstract int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalTracksCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; } = 0;

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => true;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => true;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => true;

        /// <summary>
        /// Determines if collection base is initialized or not.
        /// </summary>
        public bool IsInitialized => throw new NotImplementedException();

        /// <inheritdoc/>
        public Task<bool> IsAddChildAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddPlaylistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumItemAvailable(int index)
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
        public Task<bool> IsRemovePlaylistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveChildAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes the collection group base.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task InitAsync()
        {
            _fileMetadataScanner = SourceCore.GetService<FileMetadataScanner>();

            _fileMetadataScanner.RelatedMetadataChanged += MetadataScanner_RelatedMetadataChanged;

            return Task.CompletedTask;
        }
    }
}
