using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper for <see cref="IPlaylist"/>.
    /// </summary>
    public class PlaylistViewModel : MergeableObjectViewModel<IPlaylist>, IPlaylist, ITrackCollectionViewModel
    {
        private readonly IPlaylist _playlist;

        private IUserProfile? _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewModel"/> class.
        /// </summary>
        /// <param name="playlist">The <see cref="IPlaylist"/> to wrap.</param>
        public PlaylistViewModel(IPlaylist playlist)
        {
            _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));

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
            Images = new SynchronizedObservableCollection<IImage>();

            SourceCores = playlist.GetSourceCores<ICorePlaylist>().Select(MainViewModel.GetLoadedCore).ToList();

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

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources that form this member.
        /// </summary>
        public IReadOnlyList<ICorePlaylist> Sources => _playlist.GetSources<ICorePlaylist>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> ISdkMember<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylist> ISdkMember<ICorePlaylist>.Sources => Sources;

        /// <inheritdoc />
        public string Id => _playlist.Id;

        /// <inheritdoc />
        public TimeSpan Duration => _playlist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _playlist.TotalTracksCount;

        /// <summary>
        /// The tracks in this playlist.
        /// </summary>
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _playlist.Genres;

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
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0) => _playlist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await _playlist.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }

            OnPropertyChanged(nameof(TotalTracksCount));
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _playlist.AddTrackAsync(track, index);

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
