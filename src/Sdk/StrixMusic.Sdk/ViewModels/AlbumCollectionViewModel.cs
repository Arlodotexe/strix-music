using System;
using System.Collections.Generic;
using System.Linq;
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
    /// A ViewModel for <see cref="IAlbumCollection"/>.
    /// </summary>
    public class AlbumCollectionViewModel : ObservableObject, IAlbumCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IAlbumCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IAlbumCollection"/> to wrap around.</param>
        public AlbumCollectionViewModel(IAlbumCollection collection)
        {
            _collection = collection;

            using (Threading.PrimaryContext)
            {
                Images = new SynchronizedObservableCollection<IImage>();
                Albums = new SynchronizedObservableCollection<IAlbumCollectionItem>();
            }

            SourceCores = _collection.GetSourceCores<ICoreAlbumCollection>().Select(MainViewModel.GetLoadedCore).ToList();

            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            AlbumItemsCountChanged += OnAlbumItemsCountChanged;
            AlbumItemsChanged += AlbumCollectionViewModel_AlbumItemsChanged;
            ImagesCountChanged += OnImagesCountChanged;
            ImagesChanged += AlbumCollectionViewModel_ImagesChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
            AlbumItemsChanged -= AlbumCollectionViewModel_AlbumItemsChanged;
            ImagesCountChanged -= OnImagesCountChanged;
            ImagesChanged -= AlbumCollectionViewModel_ImagesChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void OnNameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void OnDescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void OnAlbumItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalAlbumItemsCount));

        private void OnImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void OnLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

        private void AlbumCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Images.Insert(item.Index, item.Data);
            }

            foreach (var item in removedItems)
            {
                Images.RemoveAt(item.Index);
            }
        }

        private void AlbumCollectionViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IAlbum album:
                        Albums.Insert(item.Index, new AlbumViewModel(album));
                        break;
                    case IAlbumCollection collection:
                        Albums.Insert(item.Index, new AlbumCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IAlbumCollectionItem>)Albums, nameof(Albums));
                Albums.RemoveAt(item.Index);
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

        /// <inheritdoc/>
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _collection.AlbumItemsCountChanged += value;
            remove => _collection.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _collection.ImagesChanged += value;
            remove => _collection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _collection.ImagesCountChanged += value;
            remove => _collection.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => _collection.AlbumItemsChanged += value;
            remove => _collection.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            foreach (var item in await _collection.GetAlbumItemsAsync(limit, Albums.Count))
            {
                switch (item)
                {
                    case IAlbum album:
                        Albums.Add(new AlbumViewModel(album));
                        break;
                    case IAlbumCollection collection:
                        Albums.Add(new AlbumCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await _collection.GetImagesAsync(limit, Images.Count))
            {
                Images.Add(item);
            }
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IAlbumCollectionItem> Albums { get; set; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public string Id => _collection.Id;

        /// <inheritdoc />
        public Uri? Url => _collection.Url;

        /// <inheritdoc />
        public string Name => _collection.Name;

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
        public int TotalAlbumItemsCount => _collection.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => _collection.IsPlayAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _collection.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _collection.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collection.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collection.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailable(int index) => _collection.IsAddAlbumItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => _collection.IsRemoveAlbumItemAvailable(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _collection.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _collection.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _collection.IsRemoveImageAvailable(index);

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => _collection.GetSources<ICoreAlbumCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => _collection.GetSources<ICoreAlbumCollectionItem>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _collection.GetSources<ICoreImageCollection>();

        /// <summary>
        /// The sources for this item.
        /// </summary>
        public IReadOnlyList<ICoreAlbumCollection> Sources => _collection.GetSources<ICoreAlbumCollection>();

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
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _collection.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _collection.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => _collection.Equals(other);
    }
}