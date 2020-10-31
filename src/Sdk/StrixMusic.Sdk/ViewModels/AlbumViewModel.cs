using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreAlbum"/>.
    /// </summary>
    public class AlbumViewModel : MergeableObjectViewModel<ICoreAlbum>, ICoreAlbum, ITrackCollectionViewModel
    {
        private readonly ICoreAlbum _coreAlbum;
        private readonly IPlaybackHandlerService _playbackHandler;
        private ArtistViewModel _coreArtist; // TODO: Expose this field from readonly property

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewModel"/> class.
        /// </summary>
        /// <param name="coreAlbum"><inheritdoc cref="ICoreAlbum"/></param>
        public AlbumViewModel(ICoreAlbum coreAlbum)
        {
            _coreAlbum = coreAlbum;

            SourceCore = MainViewModel.GetLoadedCore(_coreAlbum.SourceCore);

            _playbackHandler = Ioc.Default.GetService<IPlaybackHandlerService>();

            Images = new SynchronizedObservableCollection<ICoreImage>(_coreAlbum.Images);
            Tracks = new SynchronizedObservableCollection<TrackViewModel>();

            if (_coreAlbum.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_coreAlbum.RelatedItems);

            _coreArtist = new ArtistViewModel(_coreAlbum.Artist);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _coreAlbum.PlaybackStateChanged += CoreAlbumPlaybackStateChanged;
            _coreAlbum.DescriptionChanged += CoreAlbumDescriptionChanged;
            _coreAlbum.DatePublishedChanged += CoreAlbumDatePublishedChanged;
            _coreAlbum.NameChanged += CoreAlbumNameChanged;
            _coreAlbum.UrlChanged += CoreAlbumUrlChanged;
        }

        private void DetachEvents()
        {
            _coreAlbum.PlaybackStateChanged -= CoreAlbumPlaybackStateChanged;
            _coreAlbum.DescriptionChanged -= CoreAlbumDescriptionChanged;
            _coreAlbum.DatePublishedChanged -= CoreAlbumDatePublishedChanged;
            _coreAlbum.NameChanged -= CoreAlbumNameChanged;
            _coreAlbum.UrlChanged -= CoreAlbumUrlChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _coreAlbum.PlaybackStateChanged += value;

            remove => _coreAlbum.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _coreAlbum.NameChanged += value;

            remove => _coreAlbum.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _coreAlbum.DescriptionChanged += value;

            remove => _coreAlbum.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _coreAlbum.UrlChanged += value;

            remove => _coreAlbum.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _coreAlbum.DurationChanged += value;

            remove => _coreAlbum.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?> DatePublishedChanged
        {
            add => _coreAlbum.DatePublishedChanged += value;

            remove => _coreAlbum.DatePublishedChanged -= value;
        }

        private void CoreAlbumUrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void CoreAlbumNameChanged(object sender, string e)
        {
            Name = e;
        }

        private void CoreAlbumDescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void CoreAlbumPlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void CoreAlbumDatePublishedChanged(object sender, DateTime? e)
        {
            DatePublished = e;
        }

        /// <inheritdoc />
        public string Id => _coreAlbum.Id;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public TimeSpan Duration => _coreAlbum.Duration;

        /// <inheritdoc />
        public int TotalTracksCount => _coreAlbum.TotalTracksCount;

        /// <inheritdoc />
        public IPlayableCollectionGroupBase? RelatedItems { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _coreAlbum.Genres;

        /// <summary>
        /// The tracks for this album.
        /// </summary>
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public string Name
        {
            get => _coreAlbum.Name;
            private set => SetProperty(() => _coreAlbum.Name, value);
        }

        /// <inheritdoc cref="IAlbumBase.Artist" />
        public ArtistViewModel Artist
        {
            get => _coreArtist;
            set => SetProperty(ref _coreArtist, new ArtistViewModel(value));
        }

        /// <inheritdoc />
        ICoreArtist ICoreAlbum.CoreArtist => Artist;

        /// <inheritdoc />
        public Uri? Url
        {
            get => _coreAlbum.Url;
            private set => SetProperty(() => _coreAlbum.Url, value);
        }

        /// <inheritdoc />
        public DateTime? DatePublished
        {
            get => _coreAlbum.DatePublished;
            set => SetProperty(() => _coreAlbum.DatePublished, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _coreAlbum.Description;
            private set => SetProperty(() => _coreAlbum.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _coreAlbum.PlaybackState;
            private set => SetProperty(() => _coreAlbum.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => _coreAlbum.IsPlayAsyncSupported;
            set => SetProperty(() => _coreAlbum.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => _coreAlbum.IsPauseAsyncSupported;
            set => SetProperty(() => _coreAlbum.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => _coreAlbum.IsChangeNameAsyncSupported;
            set => SetProperty(() => _coreAlbum.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _coreAlbum.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _coreAlbum.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDatePublishedAsyncSupported
        {
            get => _coreAlbum.IsChangeDatePublishedAsyncSupported;
            set => SetProperty(() => _coreAlbum.IsChangeDatePublishedAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => _coreAlbum.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _coreAlbum.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public Task PauseAsync() => _coreAlbum.PauseAsync();

        /// <inheritdoc />
        public async Task PlayAsync()
        {
            if (MainViewModel.Singleton?.ActiveDevice?.Type == DeviceType.Remote)
            {
                await _coreAlbum.PlayAsync();
                return;
            }

            if (_playbackHandler.PlaybackState == PlaybackState.Paused)
            {
                await _playbackHandler.ResumeAsync();
            }
            else
            {
                await _playbackHandler.PauseAsync();

                // The actual playback
                _playbackHandler.ClearNext();
                _playbackHandler.ClearPrevious();

                // Make sure we have all tracks
                while (Tracks.Count < TotalTracksCount)
                {
                    await PopulateMoreTracksAsync(100);
                }

                // Insert the items into queue
                foreach (var item in Tracks)
                {
                    var index = Tracks.IndexOf(item);

                    var mediaSource = await SourceCore.GetMediaSource(item);

                    if (mediaSource is null)
                        continue;

                    _playbackHandler.InsertNext(index, mediaSource);
                }

                if (MainViewModel.Singleton?.LocalDevice?.Model is StrixDevice device)
                {
                    device.SetPlaybackData(_coreAlbum, Tracks[0].Model);
                }

                // Play the first thing in the album.
                await _playbackHandler.PlayFromNext(0);
            }
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _coreAlbum.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _coreAlbum.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _coreAlbum.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _coreAlbum.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _coreAlbum.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _coreAlbum.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _coreAlbum.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => _coreAlbum.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _coreAlbum.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task ChangeDatePublishedAsync(DateTime datePublished) => _coreAlbum.ChangeDatePublishedAsync(datePublished);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset) => _coreAlbum.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            await foreach (var item in _coreAlbum.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            return _coreAlbum.AddTrackAsync(track, index);
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            return _coreAlbum.RemoveTrackAsync(index);
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <summary>
        /// Attempts to play the album.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the album, if playing.
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
    }
}
