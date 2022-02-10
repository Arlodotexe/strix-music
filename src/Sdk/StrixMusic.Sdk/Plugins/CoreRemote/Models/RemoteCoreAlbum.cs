using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OwlCore;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICoreAlbum"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    public sealed class RemoteCoreAlbum : ICoreAlbum
    {
        private readonly MemberRemote _memberRemote;
        private readonly ICoreAlbum? _album;

        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalImageCount;
        private int _totalUrlCount;
        private int _totalGenreCount;

        private string _name;
        private string? _description;
        private PlaybackState _playbackState;
        private TimeSpan _duration;
        private DateTime? _lastPlayed;
        private DateTime? _datePublished;

        private bool _isChangeDurationAsyncAvailable;
        private bool _isChangeDescriptionAsyncAvailable;
        private bool _isChangeNameAsyncAvailable;
        private bool _isPauseTrackCollectionAsyncAvailable;
        private bool _isPlayTrackCollectionAsyncAvailable;
        private bool _isPauseArtistCollectionAsyncAvailable;
        private bool _isPlayArtistCollectionAsyncAvailable;
        private bool _isChangeDatePublishedAsyncAvailable;

        private SemaphoreSlim _getTracksMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getArtistsMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getImagesMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getUrlsMutex = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _getGenresMutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreAlbum"/>.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The instance ID of the core that created this object.</param>
        /// <param name="id">A unique identifier for this instance.</param>
        /// <param name="name">The name of the data.</param>
        [JsonConstructor]
        internal RemoteCoreAlbum(string sourceCoreInstanceId, string name, string id)
        {
            _name = name;
            Id = id;
            SourceCoreInstanceId = sourceCoreInstanceId;

            // Properties assigned before MemberRemote is created won't be set remotely.
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client); // should be set remotely by the ctor.

            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{nameof(RemoteCoreAlbum)}.{id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Wraps around and remotely relays events, property changes and method calls (with return data) from an album instance.
        /// </summary>
        /// <param name="coreAlbum"></param>
        internal RemoteCoreAlbum(ICoreAlbum coreAlbum)
        {
            _album = coreAlbum;
            _name = coreAlbum.Name;
            Id = coreAlbum.Id;
            SourceCoreInstanceId = coreAlbum.SourceCore.InstanceId;

            SourceCore = RemoteCore.GetInstance(SourceCoreInstanceId, RemotingMode.Host);

            _memberRemote = new MemberRemote(this, $"{coreAlbum.SourceCore.InstanceId}.{nameof(RemoteCoreAlbum)}.{coreAlbum.Id}", RemoteCoreMessageHandler.SingletonHost);
        }

        [RemoteMethod]
        private void OnTracksChanged(IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
        {
            TracksChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnArtistItemsChanged(IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnImagesChanged(IReadOnlyList<CollectionChangedItem<ICoreImage>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnUrlsChanged(IReadOnlyList<CollectionChangedItem<ICoreUrl>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        [RemoteMethod]
        private void OnGenresChanged(IReadOnlyList<CollectionChangedItem<ICoreGenre>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreGenre>> removedItems)
        {
            GenresChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
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
        public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? GenresCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? DatePublishedChanged;

        /// <inheritdoc />
        public ICore SourceCore { get; set; }

        /// <summary>
        /// The instance ID of the <see cref="SourceCore" />
        /// </summary>
        public string SourceCoreInstanceId { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                DescriptionChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            set
            {
                _playbackState = value;
                PlaybackStateChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                DurationChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? LastPlayed
        {
            get => _lastPlayed;
            set
            {
                _lastPlayed = value;
                LastPlayedChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? AddedAt { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public ICorePlayableCollectionGroup? RelatedItems { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public DateTime? DatePublished
        {
            get => _datePublished;
            set
            {
                _datePublished = value;
                DatePublishedChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalArtistItemsCount
        {
            get => _totalArtistItemsCount;
            set
            {
                _totalArtistItemsCount = value;
                ArtistItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalTrackCount
        {
            get => _totalTrackCount;
            set
            {
                _totalTrackCount = value;
                TracksCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalImageCount
        {
            get => _totalImageCount;
            internal set
            {
                _totalImageCount = value;
                ImagesCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalUrlCount
        {
            get => _totalUrlCount;
            internal set
            {
                _totalUrlCount = value;
                UrlsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        public int TotalGenreCount
        {
            get => _totalGenreCount;
            internal set
            {
                _totalGenreCount = value;
                GenresCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayArtistCollectionAsyncAvailable
        {
            get => _isPlayArtistCollectionAsyncAvailable;
            set
            {
                _isPlayArtistCollectionAsyncAvailable = value;
                IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseArtistCollectionAsyncAvailable
        {
            get => _isPauseArtistCollectionAsyncAvailable;
            set
            {
                _isPauseArtistCollectionAsyncAvailable = value;
                IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPlayTrackCollectionAsyncAvailable
        {
            get => _isPlayTrackCollectionAsyncAvailable;
            set
            {
                _isPlayTrackCollectionAsyncAvailable = value;
                IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseTrackCollectionAsyncAvailable
        {
            get => _isPauseTrackCollectionAsyncAvailable;
            set
            {
                _isPauseTrackCollectionAsyncAvailable = value;
                IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeNameAsyncAvailable
        {
            get => _isChangeNameAsyncAvailable;
            set
            {
                _isChangeNameAsyncAvailable = value;
                IsChangeNameAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsChangeDescriptionAsyncAvailable
        {
            get => _isChangeDescriptionAsyncAvailable;
            set
            {
                _isChangeDescriptionAsyncAvailable = value;
                IsChangeDescriptionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc/>
        [RemoteProperty]
        public bool IsChangeDurationAsyncAvailable
        {
            get => _isChangeDurationAsyncAvailable;
            set
            {
                _isChangeDurationAsyncAvailable = value;
                IsChangeDurationAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc/>
        [RemoteProperty]
        public bool IsChangeDatePublishedAsyncAvailable
        {
            get => _isChangeDatePublishedAsyncAvailable;
            set
            {
                _isChangeDatePublishedAsyncAvailable = value;
                IsChangeDatePublishedAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddTrackAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddTrackAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddArtistItemAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddImageAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddImageAvailableAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddUrlAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddUrlAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveTrackAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveImageAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveUrlAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveArtistItemAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsAddGenreAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsAddGenreAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveGenreAvailableAsync(int index) => _memberRemote.ReceiveDataAsync<bool>(nameof(IsRemoveGenreAvailableAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDescriptionAsync(string? description) => _memberRemote.ReceiveDataAsync<object>(nameof(ChangeDescriptionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDurationAsync(TimeSpan duration) => _memberRemote.RemoteWaitAsync(nameof(ChangeDurationAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeNameAsync(string name) => _memberRemote.RemoteWaitAsync(nameof(ChangeNameAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDatePublishedAsync(DateTime datePublished) => _memberRemote.RemoteWaitAsync(nameof(ChangeDatePublishedAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseArtistCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PlayArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem) => _memberRemote.RemoteWaitAsync(nameof(PlayArtistCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync(ICoreTrack track) => _memberRemote.RemoteWaitAsync(nameof(PlayTrackCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseTrackCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseTrackCollectionAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync() => _memberRemote.RemoteWaitAsync(nameof(PauseTrackCollectionAsync));

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getArtistsMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreArtistCollectionItem>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            using (await Flow.EasySemaphore(_getTracksMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreTrack>>(nameof(GetTracksAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getImagesMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreImage>>(nameof(GetImagesAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getUrlsMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IEnumerable<ICoreUrl>>(nameof(GetUrlsAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset)
        {
            using (await Flow.EasySemaphore(_getGenresMutex))
            {
                var res = await _memberRemote.ReceiveDataAsync<IReadOnlyList<ICoreGenre>>(nameof(GetGenresAsync));

                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddTrackAsync(ICoreTrack track, int index) => _memberRemote.RemoteWaitAsync(nameof(AddTrackAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index) => _memberRemote.RemoteWaitAsync(nameof(AddArtistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddImageAsync(ICoreImage image, int index) => _memberRemote.RemoteWaitAsync(nameof(AddImageAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddUrlAsync(ICoreUrl image, int index) => _memberRemote.RemoteWaitAsync(nameof(AddUrlAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task AddGenreAsync(ICoreGenre genre, int index) => _memberRemote.RemoteWaitAsync(nameof(AddGenreAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveTrackAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveTrackAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveArtistItemAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveArtistItemAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveImageAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveImageAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveUrlAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveUrlAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveGenreAsync(int index) => _memberRemote.RemoteWaitAsync(nameof(RemoveGenreAsync));

        /// <inheritdoc />
        [RemoteMethod]
        public ValueTask DisposeAsync() => new ValueTask(Task.Run(async () =>
        {
            await _memberRemote.RemoteWaitAsync(nameof(DisposeAsync));
            _memberRemote.Dispose();
        }));
    }
}
