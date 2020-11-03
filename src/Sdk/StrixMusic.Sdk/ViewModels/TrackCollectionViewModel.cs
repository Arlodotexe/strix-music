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
using StrixMusic.Sdk.Extensions.SdkMember;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ITrackCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class TrackCollectionViewModel : ObservableObject, ITrackCollectionViewModel
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

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);

            SourceCores = collection.GetSourceCores().Select(MainViewModel.GetLoadedCore).ToList();

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => Url = e;

        private void OnNameChanged(object sender, string e) => Name = e;

        private void OnDescriptionChanged(object sender, string? e) => Description = e;

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

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
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _collection.TotalTracksCount;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged into this collection.
        /// </summary>
        public IReadOnlyList<ICoreTrackCollection> Sources => _collection.GetSources();

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => _collection.Sources;

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collection.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collection.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _collection.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _collection.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _collection.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await _collection.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

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
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }
    }
}