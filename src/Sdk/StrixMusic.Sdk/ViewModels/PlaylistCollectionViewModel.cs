using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ICoreArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class PlaylistCollectionViewModel : ObservableObject, IPlaylistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IPlaylistCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="PlaylistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IPlaylistCollection"/> to wrap around.</param>
        public PlaylistCollectionViewModel(IPlaylistCollection collection)
        {
            _collection = collection;

            using (Threading.PrimaryContext)
            {
                Images = new SynchronizedObservableCollection<IImage>();
                Playlists = new SynchronizedObservableCollection<IPlaylistCollectionItem>();
            }

            PopulateMorePlaylistsCommand = new AsyncRelayCommand<int>(PopulateMorePlaylistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
            LastPlayedChanged += OnLastPlayedChanged;

            PlaylistItemsCountChanged += OnPlaylistItemsCountChanged;
            ImagesCountChanged += PlaylistCollectionViewModel_ImagesCountChanged;

            PlaylistItemsChanged += PlaylistCollectionViewModel_PlaylistItemsChanged;
            ImagesChanged += PlaylistCollectionViewModel_ImagesChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
            LastPlayedChanged -= OnLastPlayedChanged;

            PlaylistItemsCountChanged -= OnPlaylistItemsCountChanged;
            ImagesCountChanged -= PlaylistCollectionViewModel_ImagesCountChanged;

            PlaylistItemsChanged -= PlaylistCollectionViewModel_PlaylistItemsChanged;
            ImagesChanged -= PlaylistCollectionViewModel_ImagesChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void OnNameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void OnDescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void OnPlaylistItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalPlaylistItemsCount));

        private void PlaylistCollectionViewModel_ImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void OnLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

        private void PlaylistCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Images.Insert(item.Index, item.Data);
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IImage>)Images, nameof(Images));
                Images.RemoveAt(item.Index);
            }
        }

        private void PlaylistCollectionViewModel_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IPlaylist playlist:
                        Playlists.Insert(item.Index, new PlaylistViewModel(playlist));
                        break;
                    case IPlaylistCollection collection:
                        Playlists.Insert(item.Index, new PlaylistCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IPlaylistCollectionItem>)Playlists, nameof(Playlists));
                Playlists.RemoveAt(item.Index);
            }
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collection.PlaybackStateChanged += value;
            remove => _collection.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _collection.NameChanged += value;
            remove => _collection.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _collection.DescriptionChanged += value;
            remove => _collection.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged
        {
            add => _collection.UrlChanged += value;
            remove => _collection.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collection.DurationChanged += value;
            remove => _collection.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _collection.LastPlayedChanged += value;
            remove => _collection.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged
        {
            add => _collection.PlaylistItemsCountChanged += value;
            remove => _collection.PlaylistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _collection.ImagesCountChanged += value;
            remove => _collection.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _collection.ImagesChanged += value;
            remove => _collection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => _collection.PlaylistItemsChanged += value;
            remove => _collection.PlaylistItemsChanged -= value;
        }

        /// <inheritdoc />
        public string Id => _collection.Id;

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => _collection.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _collection.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _collection.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _collection.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _collection.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Uri? Url => _collection.Url;

        /// <inheritdoc />
        public string Name => _collection.Name;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collection.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

        /// <inheritdoc />
        public string? Description => _collection.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collection.PlaybackState;

        /// <inheritdoc />
        public TimeSpan Duration => _collection.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collection.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collection.AddedAt;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return _collection.PlayAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return _collection.PauseAsync();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _collection.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collection.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collection.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _collection.GetSourceCores<ICorePlaylistCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => Sources;

        /// <summary>
        /// The merged sources that form this item.
        /// </summary>
        public IReadOnlyList<ICorePlaylistCollection> Sources => _collection.GetSources<ICorePlaylistCollection>();

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index) => _collection.IsAddPlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemSupported(int index) => _collection.IsRemovePlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _collection.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collection.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset) => _collection.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit)
        {
            foreach (var item in await _collection.GetPlaylistItemsAsync(limit, Playlists.Count))
            {
                switch (item)
                {
                    case IPlaylist playlist:
                        Playlists.Add(new PlaylistViewModel(playlist));
                        break;
                    case IPlaylistCollection collection:
                        Playlists.Add(new PlaylistCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await  _collection.GetImagesAsync(limit, Images.Count))
            {
                Images.Add(item);
            }
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection other) => _collection.Equals(other);
    }
}