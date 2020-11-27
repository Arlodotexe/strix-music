using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ITrackCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class TrackCollectionViewModel : ObservableObject, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly ITrackCollection _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITrackCollectionViewModel"/> class.
        /// </summary>
        /// <param name="collection">The base <see cref="ITrackCollection"/> containing properties about this class.</param>
        public TrackCollectionViewModel(ITrackCollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException();

            Tracks = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<TrackViewModel>());
            Images = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IImage>());

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            SourceCores = collection.GetSourceCores<ICoreTrackCollection>().Select(MainViewModel.GetLoadedCore).ToList();

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;

            TrackItemsCountChanged += OnTrackItemsCountChanged;
            TrackItemsChanged += TrackCollectionViewModel_TrackItemsChanged;
            ImagesChanged += TrackCollectionViewModel_ImagesChanged;
            ImagesCountChanged += TrackCollectionViewModel_ImagesCountChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;

            TrackItemsCountChanged -= OnTrackItemsCountChanged;
            TrackItemsChanged -= TrackCollectionViewModel_TrackItemsChanged;
            ImagesChanged -= TrackCollectionViewModel_ImagesChanged;
            ImagesCountChanged -= TrackCollectionViewModel_ImagesCountChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => Url = e;

        private void OnNameChanged(object sender, string e) => Name = e;

        private void OnDescriptionChanged(object sender, string? e) => Description = e;

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void OnTrackItemsCountChanged(object sender, int e) => TotalTracksCount = e;

        private void TrackCollectionViewModel_ImagesCountChanged(object sender, int e) => TotalImageCount = e;

        private void TrackCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
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

        private void TrackCollectionViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Tracks.Insert(item.Index, new TrackViewModel(item.Data));
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<ITrack>)Tracks, nameof(Tracks));
                Tracks.RemoveAt(item.Index);
            }
        }

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

        /// <inheritdoc />
        public event EventHandler<int> TrackItemsCountChanged
        {
            add => _collection.TrackItemsCountChanged += value;
            remove => _collection.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> ImagesCountChanged
        {
            add => _collection.ImagesCountChanged += value;
            remove => _collection.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage> ImagesChanged
        {
            add => _collection.ImagesChanged += value;
            remove => _collection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack> TrackItemsChanged
        {
            add => _collection.TrackItemsChanged += value;
            remove => _collection.TrackItemsChanged -= value;
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
        public Uri? Url
        {
            get => _collection.Url;
            set => SetProperty(_collection.Url, value, _collection, (m, v) => m.Url = v);
        }

        /// <inheritdoc />
        public string Name
        {
            get => _collection.Name;
            set => SetProperty(_collection.Name, value, _collection, (m, v) => m.Name = v);
        }

        /// <inheritdoc />
        public int TotalTracksCount
        {
            get => _collection.TotalTracksCount;
            set => SetProperty(_collection.TotalTracksCount, value, _collection, (m, v) => m.TotalTracksCount = v);
        }

        /// <inheritdoc />
        public int TotalImageCount
        {
            get => _collection.TotalImageCount;
            set => SetProperty(_collection.TotalImageCount, value, _collection, (m, v) => m.TotalImageCount = v);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _collection.Description;
            set => SetProperty(_collection.Description, value, _collection, (m, v) => m.Description = v);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _collection.PlaybackState;
            set => SetProperty(_collection.PlaybackState, value, _collection, (m, v) => m.PlaybackState = v);
        }

        /// <inheritdoc />
        public TimeSpan Duration
        {
            get => _collection.Duration;
            set => SetProperty(_collection.Duration, value, _collection, (m, v) => m.Duration = v);
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged into this collection.
        /// </summary>
        public IReadOnlyList<ICoreTrackCollection> Sources => _collection.GetSources<ICoreTrackCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _collection.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _collection.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collection.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collection.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _collection.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

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
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await _collection.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
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
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }
    }
}