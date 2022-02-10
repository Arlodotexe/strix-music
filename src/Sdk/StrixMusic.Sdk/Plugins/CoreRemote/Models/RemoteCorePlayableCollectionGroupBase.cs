using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around a <see cref="ICorePlayableCollectionGroup"/> to host data, or receives data given some identifiers.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public abstract partial class RemoteCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup
    {
        private readonly MemberRemote _memberRemote;
        private readonly ICorePlayableCollectionGroup? _playableCollectionGroup;
        private readonly ICoreArtistCollection? _artistCollection;
        private readonly ICoreAlbumCollection? _albumCollection;
        private readonly ICorePlaylistCollection? _playlistCollection;
        private readonly ICoreTrackCollection? _trackCollection;
        private readonly ICoreImageCollection? _imageCollection;
        private readonly ICoreUrlCollection? _urlCollection;
        private readonly IPlayableBase? _playableBase;

        private string _name = string.Empty;
        private string? _description;

        private int _totalAlbumItemsCount;
        private int _totalArtistItemsCount;
        private int _totalTrackCount;
        private int _totalPlaylistItemsCount;
        private int _totalChildrenCount;
        private int _totalImageCount;
        private int _totalUrlCount;

        private bool _isChangeNameAsyncAvailable;
        private bool _isChangeDescriptionAsyncAvailable;
        private bool _isChangeDurationAsyncAvailable;
        private bool _isPlayAlbumCollectionAsyncAvailable;
        private bool _isPauseAlbumCollectionAsyncAvailable;
        private bool _isPlayArtistCollectionAsyncAvailable;
        private bool _isPauseArtistCollectionAsyncAvailable;
        private bool _isPlayPlaylistCollectionAsyncAvailable;
        private bool _isPausePlaylistCollectionAsyncAvailable;
        private bool _isPlayTrackCollectionAsyncAvailable;
        private bool _isPauseTrackCollectionAsyncAvailable;

        private PlaybackState _playbackState;
        private TimeSpan _duration;
        private DateTime? _lastPlayed;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroupBase"/>, for receiving data.
        /// </summary>
        protected RemoteCorePlayableCollectionGroupBase(string sourceCoreInstanceId, string id)
        {
            // These properties are set remotely in the other ctor
            SourceCoreInstanceId = sourceCoreInstanceId;
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client);
            Id = id;

            _memberRemote = new MemberRemote(this, $"{SourceCoreInstanceId}.{GetType().Name}.{Id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroupBase"/> and wraps around a <paramref name="corePlayableCollection"/> for sending data.
        /// </summary>
        /// <param name="corePlayableCollection">The collection to wrap around and remotely interact with.</param>
        protected RemoteCorePlayableCollectionGroupBase(ICorePlayableCollectionGroup corePlayableCollection)
        {
            _playableCollectionGroup = corePlayableCollection;
            _artistCollection = corePlayableCollection;
            _albumCollection = corePlayableCollection;
            _playlistCollection = corePlayableCollection;
            _trackCollection = corePlayableCollection;
            _imageCollection = corePlayableCollection;
            _urlCollection = corePlayableCollection;
            _playableBase = corePlayableCollection;

            SourceCoreInstanceId = corePlayableCollection.SourceCore.InstanceId;
            SourceCore = RemoteCore.GetInstance(SourceCoreInstanceId, RemotingMode.Host);
            Id = corePlayableCollection.Id;

            var fullRemoteId = $"{SourceCoreInstanceId}.{GetType().Name}.{Id}";
            _memberRemote = new MemberRemote(this, fullRemoteId, RemoteCoreMessageHandler.SingletonHost);

            Name = corePlayableCollection.Name;
            Description = corePlayableCollection.Description;
            PlaybackState = corePlayableCollection.PlaybackState;
            Duration = corePlayableCollection.Duration;
            LastPlayed = corePlayableCollection.LastPlayed;
            AddedAt = corePlayableCollection.AddedAt;

            TotalTrackCount = corePlayableCollection.TotalTrackCount;
            TotalArtistItemsCount = corePlayableCollection.TotalArtistItemsCount;
            TotalAlbumItemsCount = corePlayableCollection.TotalAlbumItemsCount;
            TotalPlaylistItemsCount = corePlayableCollection.TotalPlaylistItemsCount;
            TotalChildrenCount = corePlayableCollection.TotalChildrenCount;
            TotalImageCount = corePlayableCollection.TotalImageCount;
            TotalUrlCount = corePlayableCollection.TotalUrlCount;

            IsChangeNameAsyncAvailable = corePlayableCollection.IsChangeNameAsyncAvailable;
            IsChangeDescriptionAsyncAvailable = corePlayableCollection.IsChangeDescriptionAsyncAvailable;
            IsChangeDurationAsyncAvailable = corePlayableCollection.IsChangeDurationAsyncAvailable;

            IsPlayAlbumCollectionAsyncAvailable = corePlayableCollection.IsPlayAlbumCollectionAsyncAvailable;
            IsPauseAlbumCollectionAsyncAvailable = corePlayableCollection.IsPauseAlbumCollectionAsyncAvailable;
            IsPlayArtistCollectionAsyncAvailable = corePlayableCollection.IsPlayArtistCollectionAsyncAvailable;
            IsPauseArtistCollectionAsyncAvailable = corePlayableCollection.IsPauseArtistCollectionAsyncAvailable;
            IsPlayPlaylistCollectionAsyncAvailable = corePlayableCollection.IsPlayPlaylistCollectionAsyncAvailable;
            IsPausePlaylistCollectionAsyncAvailable = corePlayableCollection.IsPausePlaylistCollectionAsyncAvailable;
            IsPlayTrackCollectionAsyncAvailable = corePlayableCollection.IsPlayTrackCollectionAsyncAvailable;
            IsPauseTrackCollectionAsyncAvailable = corePlayableCollection.IsPauseTrackCollectionAsyncAvailable;

            AttachEvents(corePlayableCollection);
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
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

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
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ChildrenCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <summary>
        /// The instance ID of the <see cref="SourceCore" />
        /// </summary>
        public string SourceCoreInstanceId { get; set; }

        /// <inheritdoc />
        public ICore SourceCore { get; set; }

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
        public int TotalAlbumItemsCount
        {
            get => _totalAlbumItemsCount;
            set
            {
                _totalAlbumItemsCount = value;
                AlbumItemsCountChanged?.Invoke(this, value);
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
        public int TotalPlaylistItemsCount
        {
            get => _totalPlaylistItemsCount;
            set
            {
                _totalPlaylistItemsCount = value;
                PlaylistItemsCountChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public int TotalChildrenCount
        {
            get => _totalChildrenCount;
            internal set
            {
                _totalChildrenCount = value;
                ChildrenCountChanged?.Invoke(this, value);
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
        [RemoteProperty]
        public bool IsPlayAlbumCollectionAsyncAvailable
        {
            get => _isPlayAlbumCollectionAsyncAvailable;
            set
            {
                _isPlayAlbumCollectionAsyncAvailable = value;
                IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPauseAlbumCollectionAsyncAvailable
        {
            get => _isPauseAlbumCollectionAsyncAvailable;
            set
            {
                _isPauseAlbumCollectionAsyncAvailable = value;
                IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
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
        public bool IsPlayPlaylistCollectionAsyncAvailable
        {
            get => _isPlayPlaylistCollectionAsyncAvailable;
            set
            {
                _isPlayPlaylistCollectionAsyncAvailable = value;
                IsPlayPlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public bool IsPausePlaylistCollectionAsyncAvailable
        {
            get => _isPausePlaylistCollectionAsyncAvailable;
            set
            {
                _isPausePlaylistCollectionAsyncAvailable = value;
                IsPausePlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
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
        [RemoteMethod]
        public Task<bool> IsAddChildAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddChildAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                var res = await _playableCollectionGroup.IsAddChildAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddPlaylistItemAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                var res = await _playlistCollection.IsAddPlaylistItemAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddTrackAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddTrackAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                var res = await _trackCollection.IsAddTrackAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddArtistItemAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                var res = await _artistCollection.IsAddArtistItemAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddAlbumItemAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddAlbumItemAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                var res = await _albumCollection.IsAddAlbumItemAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddImageAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddImageAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_imageCollection, nameof(_imageCollection));

                var res = await _imageCollection.IsAddImageAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public Task<bool> IsAddUrlAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsAddUrlAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_urlCollection, nameof(_urlCollection));

                var res = await _urlCollection.IsAddUrlAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemoveTrackAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                var res = await _trackCollection.IsRemoveTrackAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveImageAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemoveImageAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_imageCollection, nameof(_imageCollection));

                var res = await _imageCollection.IsRemoveImageAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemoveUrlAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_urlCollection, nameof(_urlCollection));

                var res = await _urlCollection.IsRemoveUrlAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemovePlaylistItemAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                var res = await _playlistCollection.IsRemovePlaylistItemAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemoveAlbumItemAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                var res = await _albumCollection.IsRemoveAlbumItemAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemoveArtistItemAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                var res = await _artistCollection.IsRemoveArtistItemAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task<bool> IsRemoveChildAvailableAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(IsRemoveChildAvailableAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                var res = await _playableCollectionGroup.IsRemoveChildAvailableAsync(index);
                return await _memberRemote.PublishDataAsync(methodCallToken, res);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
                return await _memberRemote.ReceiveDataAsync<bool>(methodCallToken);

            return ThrowHelper.ThrowArgumentOutOfRangeException<bool>("Invalid remoting mode");
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDescriptionAsync(string? description) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(ChangeDescriptionAsync)}.{description}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableBase, nameof(_playableBase));

                await _playableBase.ChangeDescriptionAsync(description);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeDurationAsync(TimeSpan duration) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(ChangeDurationAsync)}.{duration}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableBase, nameof(_playableBase));

                await _playableBase.ChangeDurationAsync(duration);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task ChangeNameAsync(string name) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(ChangeNameAsync)}.{name}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableBase, nameof(_playableBase));

                await _playableBase.ChangeNameAsync(name);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseAlbumCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PauseAlbumCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                await _albumCollection.PauseAlbumCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayAlbumCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                await _albumCollection.PlayAlbumCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseArtistCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PauseArtistCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                await _artistCollection.PauseArtistCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayArtistCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                await _artistCollection.PlayArtistCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlaylistCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PausePlaylistCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                await _playlistCollection.PausePlaylistCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayPlaylistCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                await _playlistCollection.PlayPlaylistCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PauseTrackCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PauseTrackCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                await _trackCollection.PauseTrackCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayTrackCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                await _trackCollection.PlayTrackCollectionAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayAlbumCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                await _albumCollection.PlayAlbumCollectionAsync(albumItem);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayPlayableCollectionGroupAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                await _playableCollectionGroup.PlayPlayableCollectionGroupAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PausePlayableCollectionGroupAsync() => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PausePlayableCollectionGroupAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                await _playableCollectionGroup.PausePlayableCollectionGroupAsync();
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayPlaylistCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                await _playlistCollection.PlayPlaylistCollectionAsync(playlistItem);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayArtistCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                await _artistCollection.PlayArtistCollectionAsync(artistItem);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayPlayableCollectionGroupAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                await _playableCollectionGroup.PlayPlayableCollectionGroupAsync(collectionGroup);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task PlayTrackCollectionAsync(ICoreTrack track) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(PlayTrackCollectionAsync)}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                await _trackCollection.PlayTrackCollectionAsync(track);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            var callToken = $"{nameof(GetChildrenAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICorePlayableCollectionGroup[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                await foreach (var item in _playableCollectionGroup.GetChildrenAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            var callToken = $"{nameof(GetPlaylistItemsAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICorePlaylistCollectionItem[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                await foreach (var item in _playlistCollection.GetPlaylistItemsAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            var callToken = $"{nameof(GetAlbumItemsAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICoreAlbumCollectionItem[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                await foreach (var item in _albumCollection.GetAlbumItemsAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var callToken = $"{nameof(GetArtistItemsAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICoreArtistCollectionItem[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                await foreach (var item in _artistCollection.GetArtistItemsAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            var callToken = $"{nameof(GetTracksAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICoreTrack[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                await foreach (var item in _trackCollection.GetTracksAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            var callToken = $"{nameof(GetImagesAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICoreImage[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_imageCollection, nameof(_imageCollection));

                await foreach (var item in _imageCollection.GetImagesAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc />
        [RemoteMethod]
        public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            var callToken = $"{nameof(GetUrlsAsync)}.{limit}.{offset}";

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                var res = await _memberRemote.ReceiveDataAsync<ICoreUrl[]>(callToken);
                if (res is null)
                    yield break;

                foreach (var item in res)
                    yield return item;

                yield break;
            }

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_urlCollection, nameof(_urlCollection));

                await foreach (var item in _urlCollection.GetUrlsAsync(limit, offset))
                    yield return item;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICoreTrack"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddTrackAsync(ICoreTrack track, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddTrackAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                await _trackCollection.AddTrackAsync(track, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICoreArtistCollectionItem"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddArtistItemAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                await _artistCollection.AddArtistItemAsync(artist, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICoreAlbumCollectionItem"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddAlbumItemAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                await _albumCollection.AddAlbumItemAsync(album, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICoreTrack"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddPlaylistItemAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                await _playlistCollection.AddPlaylistItemAsync(playlist, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICorePlayableCollectionGroup"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddChildAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                await _playableCollectionGroup.AddChildAsync(child, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICoreImage"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddImageAsync(ICoreImage image, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddImageAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_imageCollection, nameof(_imageCollection));

                await _imageCollection.AddImageAsync(image, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        /// <remarks>
        /// Implementations of <see cref="ICoreUrl"/> passed to this method must be in the SDK for deserialization to work.
        /// Typically this is a RemoteCore model or an implementation of <see cref="Sdk.Models.IInitialData"/>.
        /// </remarks>
        [RemoteMethod]
        public Task AddUrlAsync(ICoreUrl url, int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(AddUrlAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_urlCollection, nameof(_urlCollection));

                await _urlCollection.AddUrlAsync(url, index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveTrackAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemoveTrackAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_trackCollection, nameof(_trackCollection));

                await _trackCollection.RemoveTrackAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveArtistItemAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemoveArtistItemAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_artistCollection, nameof(_artistCollection));

                await _artistCollection.RemoveArtistItemAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveAlbumItemAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemoveAlbumItemAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_albumCollection, nameof(_albumCollection));

                await _albumCollection.RemoveAlbumItemAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemovePlaylistItemAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemovePlaylistItemAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playlistCollection, nameof(_playlistCollection));

                await _playlistCollection.RemovePlaylistItemAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveChildAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemoveChildAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_playableCollectionGroup, nameof(_playableCollectionGroup));

                await _playableCollectionGroup.RemoveChildAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveImageAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemoveImageAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_imageCollection, nameof(_imageCollection));

                await _imageCollection.RemoveImageAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public Task RemoveUrlAsync(int index) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(RemoveUrlAsync)}.{index}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_urlCollection, nameof(_urlCollection));

                await _urlCollection.RemoveUrlAsync(index);
                await _memberRemote.RemoteReleaseAsync(methodCallToken);
            }

            if (_memberRemote.Mode == RemotingMode.Client)
            {
                await _memberRemote.RemoteWaitAsync(methodCallToken);
            }
        });

        /// <inheritdoc />
        [RemoteMethod]
        public ValueTask DisposeAsync() => new ValueTask(Task.Run(async () =>
        {
            await _memberRemote.RemoteWaitAsync(nameof(DisposeAsync));
            _memberRemote.Dispose();
        }));
    }
}
