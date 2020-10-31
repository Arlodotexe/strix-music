using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper for <see cref="ICorePlaylist"/>.
    /// </summary>
    public class PlaylistViewModel : MergeableObjectViewModel<ICorePlaylist>, ICorePlaylist, ITrackCollectionViewModel
    {
        private readonly ICorePlaylist _playlist;

        private ICoreUserProfile? _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewModel"/> class.
        /// </summary>
        /// <param name="playlist">The <see cref="ICorePlaylist"/> to wrap.</param>
        public PlaylistViewModel(ICorePlaylist playlist)
        {
            _playlist = playlist;

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);

            if (_playlist.Owner != null)
                _owner = new UserProfileViewModel(_playlist.Owner);

            if (_playlist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_playlist.RelatedItems);

            Tracks = new SynchronizedObservableCollection<TrackViewModel>();
            Images = new SynchronizedObservableCollection<ICoreImage>();

            SourceCore = MainViewModel.GetLoadedCore(_playlist.SourceCore);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _playlist.DescriptionChanged += CorePlaylistDescriptionChanged;
            _playlist.NameChanged += CorePlaylistNameChanged;
            _playlist.PlaybackStateChanged += CorePlaylistPlaybackStateChanged;
            _playlist.UrlChanged += CorePlaylistUrlChanged;
        }

        private void DetachEvents()
        {
            _playlist.DescriptionChanged -= CorePlaylistDescriptionChanged;
            _playlist.NameChanged -= CorePlaylistNameChanged;
            _playlist.PlaybackStateChanged -= CorePlaylistPlaybackStateChanged;
            _playlist.UrlChanged -= CorePlaylistUrlChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _playlist.PlaybackStateChanged += value;

            remove => _playlist.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _playlist.NameChanged += value;

            remove => _playlist.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _playlist.DescriptionChanged += value;

            remove => _playlist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _playlist.UrlChanged += value;

            remove => _playlist.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _playlist.DurationChanged += value;
            remove => _playlist.DurationChanged -= value;
        }

        private void CorePlaylistUrlChanged(object sender, Uri? e) => Url = e;

        private void CorePlaylistPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void CorePlaylistNameChanged(object sender, string e) => Name = e;

        private void CorePlaylistDescriptionChanged(object sender, string? e) => Description = e;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _playlist.Id;

        /// <inheritdoc />
        public TimeSpan Duration => _playlist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroupBase? RelatedItems { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _playlist.TotalTracksCount;

        /// <summary>
        /// The tracks in this playlist.
        /// </summary>
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _playlist.Genres;

        /// <inheritdoc />
        public ICoreUserProfile? Owner
        {
            get => _owner;
            set => SetProperty(ref _owner, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _playlist.Url;
            set => SetProperty(() => _playlist.Url, value);
        }

        /// <inheritdoc />
        public string Name
        {
            get => _playlist.Name;
            set => SetProperty(() => _playlist.Name, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _playlist.Description;
            set => SetProperty(() => _playlist.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _playlist.PlaybackState;
            set => SetProperty(() => _playlist.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => _playlist.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _playlist.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _playlist.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _playlist.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _playlist.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _playlist.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _playlist.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _playlist.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _playlist.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => _playlist.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _playlist.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task PauseAsync() => _playlist.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => _playlist.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _playlist.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _playlist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _playlist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0) => _playlist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task PopulateMoreTracksAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index) => _playlist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _playlist.RemoveTrackAsync(index);

        /// <summary>
        /// Attempts to play the playlist.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the playlist.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }
    }
}
