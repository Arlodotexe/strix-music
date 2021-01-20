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
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ICoreArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class ArtistCollectionViewModel : ObservableObject, IArtistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IArtistCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IArtistCollection"/> to wrap around.</param>
        public ArtistCollectionViewModel(IArtistCollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

            SourceCores = collection.GetSourceCores<ICoreArtistCollection>().Select(MainViewModel.GetLoadedCore).ToList();

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            using (Threading.PrimaryContext)
            {
                Images = new SynchronizedObservableCollection<IImage>();
                Artists = new SynchronizedObservableCollection<IArtistCollectionItem>();
            }

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
            ArtistItemsCountChanged += OnArtistItemsCountChanged;
            ArtistItemsChanged += ArtistCollectionViewModel_ArtistItemsChanged;
            ImagesCountChanged += ArtistCollectionViewModel_ImagesCountChanged;
            ImagesChanged += ArtistCollectionViewModel_ImagesChanged;
            LastPlayedChanged += OnLastPlayedChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
            ArtistItemsCountChanged -= OnArtistItemsCountChanged;
            ArtistItemsChanged -= ArtistCollectionViewModel_ArtistItemsChanged;
            ImagesCountChanged -= ArtistCollectionViewModel_ImagesCountChanged;
            ImagesChanged -= ArtistCollectionViewModel_ImagesChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void OnNameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void OnDescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void ArtistCollectionViewModel_ImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void OnArtistItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalArtistItemsCount));

        private void OnLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

        private void ArtistCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
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

        private void ArtistCollectionViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IArtist artist:
                        Artists.Insert(item.Index, new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.Insert(item.Index, new ArtistCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not a supported artist item for {nameof(ArtistCollectionViewModel)}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IArtistCollectionItem>)Artists, nameof(Artists));
                Artists.RemoveAt(item.Index);
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
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _collection.ArtistItemsCountChanged += value;
            remove => _collection.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _collection.ArtistItemsChanged += value;
            remove => _collection.ArtistItemsChanged -= value;
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
        public int TotalArtistItemsCount => _collection.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

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
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtistCollection> Sources => this.GetSources<ICoreArtistCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemSupported(int index) => _collection.IsAddArtistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemSupported(int index) => _collection.IsRemoveArtistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _collection.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await _collection.GetArtistItemsAsync(limit, Artists.Count))
            {
                switch (item)
                {
                    case IArtist artist:
                        Artists.Add(new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.Add(new ArtistCollectionViewModel(collection));
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
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _collection.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _collection.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAsyncCommand { get; }
    }
}