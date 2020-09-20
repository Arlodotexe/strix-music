using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// A bindable wrapper for <see cref="IPlaylist"/>.
    /// </summary>
    public class ObservablePlaylist : ObservableMergeableObject<IPlaylist>, IPlaylist
    {
        private readonly IPlaylist _playlist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservablePlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The <see cref="IPlaylist"/> to wrap.</param>
        public ObservablePlaylist(IPlaylist playlist)
        {
            _playlist = playlist;

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            if (_playlist.Owner != null)
                _owner = new ObservableUserProfile(_playlist.Owner);

            if (_playlist.RelatedItems != null)
                RelatedItems = new ObservableCollectionGroup(_playlist.RelatedItems);

            Tracks = new ObservableCollection<ITrack>(_playlist.Tracks.Select(x => new ObservableTrack(x)));
            Images = new ObservableCollection<IImage>(_playlist.Images);

            SourceCore = MainViewModel.GetLoadedCore(_playlist.SourceCore);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _playlist.DescriptionChanged += Playlist_DescriptionChanged;
            _playlist.NameChanged += Playlist_NameChanged;
            _playlist.PlaybackStateChanged += Playlist_PlaybackStateChanged;
            _playlist.UrlChanged += Playlist_UrlChanged;
        }

        private void DetachEvents()
        {
            _playlist.DescriptionChanged -= Playlist_DescriptionChanged;
            _playlist.NameChanged -= Playlist_NameChanged;
            _playlist.PlaybackStateChanged -= Playlist_PlaybackStateChanged;
            _playlist.UrlChanged -= Playlist_UrlChanged;
        }

        private void Playlist_UrlChanged(object sender, Uri? e) => Url = e;

        private void Playlist_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void Playlist_NameChanged(object sender, string e) => Name = e;

        private void Playlist_DescriptionChanged(object sender, string? e) => Description = e;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _playlist.Id;

        /// <inheritdoc />
        public ObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _playlist.TotalTracksCount;

        private IUserProfile? _owner;

        /// <inheritdoc />
        public IUserProfile? Owner
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
        public TimeSpan Duration => _playlist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public ObservableCollection<string>? Genres => _playlist.Genres;

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
        public ObservableCollection<bool> IsRemoveTrackSupportedMap => _playlist.IsRemoveTrackSupportedMap;

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveGenreSupportedMap => _playlist.IsRemoveGenreSupportedMap;

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveImageSupportedMap => _playlist.IsRemoveImageSupportedMap;

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
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0) => _playlist.GetTracksAsync(limit, offset);

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
        public Task PopulateMoreTracksAsync(int limit) => _playlist.PopulateMoreTracksAsync(limit);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _playlist.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _playlist.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _playlist.IsAddImageSupported(index);
    }
}
