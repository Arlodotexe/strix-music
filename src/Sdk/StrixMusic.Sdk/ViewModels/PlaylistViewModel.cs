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
    /// A bindable wrapper for <see cref="IPlaylist"/>.
    /// </summary>
    public class PlaylistViewModel : ObservableObject, IPlaylist, ITrackCollectionViewModel, IImageCollectionViewModel
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

            SourceCores = playlist.GetSourceCores<ICorePlaylist>().Select(MainViewModel.GetLoadedCore).ToList();

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            if (_playlist.Owner != null)
                _owner = new UserProfileViewModel(_playlist.Owner);

            if (_playlist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_playlist.RelatedItems);

            Tracks = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<TrackViewModel>());
            Images = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IImage>());

            AttachEvents();
        }

        private void AttachEvents()
        {
            DescriptionChanged += CorePlaylistDescriptionChanged;
            NameChanged += CorePlaylistNameChanged;
            PlaybackStateChanged += CorePlaylistPlaybackStateChanged;
            UrlChanged += CorePlaylistUrlChanged;

            TrackItemsCountChanged += PlaylistOnTrackItemsCountChanged;
            TrackItemsChanged += PlaylistViewModel_TrackItemsChanged;
            ImagesCountChanged += PlaylistViewModel_ImagesCountChanged;
            ImagesChanged += PlaylistViewModel_ImagesChanged;
        }

        private void DetachEvents()
        {
            DescriptionChanged -= CorePlaylistDescriptionChanged;
            NameChanged -= CorePlaylistNameChanged;
            PlaybackStateChanged -= CorePlaylistPlaybackStateChanged;
            UrlChanged -= CorePlaylistUrlChanged;

            TrackItemsCountChanged -= PlaylistOnTrackItemsCountChanged;
            TrackItemsChanged -= PlaylistViewModel_TrackItemsChanged;
            ImagesCountChanged -= PlaylistViewModel_ImagesCountChanged;
            ImagesChanged -= PlaylistViewModel_ImagesChanged;
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

        /// <inheritdoc />
        public event EventHandler<int> TrackItemsCountChanged
        {
            add => _playlist.TrackItemsCountChanged += value;
            remove => _playlist.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> ImagesCountChanged
        {
            add => _playlist.ImagesCountChanged += value;
            remove => _playlist.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage> ImagesChanged
        {
            add => _playlist.ImagesChanged += value;
            remove => _playlist.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack> TrackItemsChanged
        {
            add => _playlist.TrackItemsChanged += value;
            remove => _playlist.TrackItemsChanged -= value;
        }

        private void CorePlaylistUrlChanged(object sender, Uri? e) => Url = e;

        private void CorePlaylistPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void CorePlaylistNameChanged(object sender, string e) => Name = e;

        private void CorePlaylistDescriptionChanged(object sender, string? e) => Description = e;

        private void PlaylistOnTrackItemsCountChanged(object sender, int e) => TotalTracksCount = e;

        private void PlaylistViewModel_ImagesCountChanged(object sender, int e) => TotalImageCount = e;

        private void PlaylistViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
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

        private void PlaylistViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
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
            set => SetProperty(_playlist.Url, value, _playlist, (m, v) => m.Url = v);
        }

        /// <inheritdoc />
        public string Name
        {
            get => _playlist.Name;
            set => SetProperty(_playlist.Name, value, _playlist, (m, v) => m.Name = v);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _playlist.Description;
            set => SetProperty(_playlist.Description, value, _playlist, (m, v) => m.Description = v);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _playlist.PlaybackState;
            set => SetProperty(_playlist.PlaybackState, value, _playlist, (m, v) => m.PlaybackState = v);
        }

        /// <inheritdoc />
        public int TotalTracksCount
        {
            get => _playlist.TotalTracksCount;
            set => SetProperty(_playlist.TotalTracksCount, value, _playlist, (m, v) => m.TotalTracksCount = v);
        }

        /// <inheritdoc />
        public int TotalImageCount
        {
            get => _playlist.TotalImageCount;
            set => SetProperty(_playlist.TotalImageCount, value, _playlist, (m, v) => m.TotalImageCount = v);
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
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _playlist.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _playlist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _playlist.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _playlist.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _playlist.RemoveImageAsync(index);

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
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await _playlist.GetImagesAsync(limit, Images.Count))
            {
                Images.Add(item);
            }
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
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }
    }
}
