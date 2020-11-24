using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="IAlbumCollection"/>.
    /// </summary>
    public class AlbumCollectionViewModel : ObservableObject, IAlbumCollectionViewModel
    {
        private readonly IAlbumCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IAlbumCollection"/> to wrap around.</param>
        public AlbumCollectionViewModel(IAlbumCollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

            Albums = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IAlbumCollectionItem>());
            SourceCores = collection.GetSourceCores<ICoreAlbumCollection>().Select(MainViewModel.GetLoadedCore).ToList();

            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
            AlbumItemsCountChanged += OnAlbumItemsCountChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
            AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => Url = e;

        private void OnNameChanged(object sender, string e) => Name = e;

        private void OnDescriptionChanged(object sender, string? e) => Description = e;

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void OnAlbumItemsCountChanged(object sender, int e) => TotalAlbumItemsCount = e;

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _collection.PlaybackStateChanged += value;
            remove => _collection.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _collection.NameChanged += value;
            remove => _collection.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _collection.DescriptionChanged += value;
            remove => _collection.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
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

        /// <inheritdoc/>
        public event EventHandler<int> AlbumItemsCountChanged
        {
            add => _collection.AlbumItemsCountChanged += value;
            remove => _collection.AlbumItemsCountChanged -= value;
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
        public SynchronizedObservableCollection<IAlbumCollectionItem> Albums { get; set; }

        /// <inheritdoc />
        public string Id => _collection.Id;

        /// <inheritdoc />
        public Uri? Url
        {
            get => _collection.Url;
            set => SetProperty(() => _collection.Url, value);
        }

        /// <inheritdoc />
        public string Name
        {
            get => _collection.Name;
            set => SetProperty(() => _collection.Name, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _collection.Description;
            set => SetProperty(() => _collection.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _collection.PlaybackState;
            set => SetProperty(() => _collection.PlaybackState, value);
        }

        /// <inheritdoc />
        public TimeSpan Duration
        {
            get => _collection.Duration;
            set => SetProperty(() => _collection.Duration, value);
        }

        /// <inheritdoc />
        public int TotalAlbumItemsCount
        {
            get => _collection.TotalAlbumItemsCount;
            set => SetProperty(() => _collection.TotalAlbumItemsCount, value);
        }

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
        public Task<bool> IsAddAlbumItemSupported(int index) => _collection.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _collection.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _collection.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => _collection.GetSources<ICoreAlbumCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => _collection.GetSources<ICoreAlbumCollectionItem>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => _collection.GetSources<ICoreImageCollection>();

        /// <summary>
        /// The sources for this merged item.
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
        public Task RemoveAlbumItemAsync(int index) => _collection.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }
    }
}