﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="IAlbum"/>.
    /// </summary>
    public class AlbumViewModel : ObservableObject, IAlbum, IArtistCollectionViewModel, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IAlbum _album;
        private readonly IPlaybackHandlerService _playbackHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewModel"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public AlbumViewModel(IAlbum album)
        {
            _album = album;

            SourceCores = _album.GetSourceCores<ICoreAlbum>().Select(MainViewModel.GetLoadedCore).ToList();

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();

            using (Threading.PrimaryContext)
            {
                Images = new ObservableCollection<IImage>();
                Tracks = new ObservableCollection<TrackViewModel>();
                Artists = new ObservableCollection<IArtistCollectionItem>();
            }

            if (_album.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_album.RelatedItems);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += AlbumPlaybackStateChanged;
            DescriptionChanged += AlbumDescriptionChanged;
            DatePublishedChanged += AlbumDatePublishedChanged;
            NameChanged += AlbumNameChanged;
            UrlChanged += AlbumUrlChanged;

            IsPlayAsyncAvailableChanged += OnIsPlayAsyncAvailableChanged;
            IsPauseAsyncAvailableChanged += OnIsPauseAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            TrackItemsCountChanged += AlbumOnTrackItemsCountChanged;
            TrackItemsChanged += AlbumViewModel_TrackItemsChanged;
            ImagesCountChanged += AlbumViewModel_ImagesCountChanged;
            ImagesChanged += AlbumViewModel_ImagesChanged;
            LastPlayedChanged += OnLastPlayedChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= AlbumPlaybackStateChanged;
            DescriptionChanged -= AlbumDescriptionChanged;
            DatePublishedChanged -= AlbumDatePublishedChanged;
            NameChanged -= AlbumNameChanged;
            UrlChanged -= AlbumUrlChanged;

            IsPlayAsyncAvailableChanged -= OnIsPlayAsyncAvailableChanged;
            IsPauseAsyncAvailableChanged -= OnIsPauseAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            TrackItemsCountChanged += AlbumOnTrackItemsCountChanged;
            TrackItemsChanged -= AlbumViewModel_TrackItemsChanged;
            ImagesCountChanged -= AlbumViewModel_ImagesCountChanged;
            ImagesChanged -= AlbumViewModel_ImagesChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _album.PlaybackStateChanged += value;
            remove => _album.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _album.NameChanged += value;
            remove => _album.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _album.DescriptionChanged += value;
            remove => _album.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged
        {
            add => _album.UrlChanged += value;
            remove => _album.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _album.DurationChanged += value;
            remove => _album.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _album.LastPlayedChanged += value;
            remove => _album.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAsyncAvailableChanged
        {
            add => _album.IsPlayAsyncAvailableChanged += value;
            remove => _album.IsPlayAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAsyncAvailableChanged
        {
            add => _album.IsPauseAsyncAvailableChanged += value;
            remove => _album.IsPauseAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _album.IsChangeNameAsyncAvailableChanged += value;
            remove => _album.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _album.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _album.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _album.IsChangeDurationAsyncAvailableChanged += value;
            remove => _album.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? DatePublishedChanged
        {
            add => _album.DatePublishedChanged += value;

            remove => _album.DatePublishedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged
        {
            add => _album.TrackItemsCountChanged += value;

            remove => _album.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged
        {
            add => _album.TrackItemsChanged += value;

            remove => _album.TrackItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _album.ImagesChanged += value;

            remove => _album.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _album.ImagesCountChanged += value;

            remove => _album.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _album.ArtistItemsCountChanged += value;
            remove => _album.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _album.ArtistItemsChanged += value;
            remove => _album.ArtistItemsChanged -= value;
        }

        private void AlbumUrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void AlbumNameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void AlbumDescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void AlbumPlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void AlbumDatePublishedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(DatePublished));

        private void AlbumOnTrackItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalTracksCount));

        private void AlbumViewModel_ImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void OnLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable));

        private void OnIsPauseAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPauseAsyncAvailable));

        private void OnIsPlayAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPlayAsyncAvailable));

        private void AlbumViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in addedItems)
                {
                    Tracks.InsertOrAdd(item.Index, new TrackViewModel(item.Data));
                }

                foreach (var item in removedItems)
                {
                    Guard.IsInRangeFor(item.Index, (IReadOnlyList<ITrack>)Tracks, nameof(Tracks));
                    Tracks.RemoveAt(item.Index);
                }
            });
        }

        private void AlbumViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in addedItems)
                {
                    Images.InsertOrAdd(item.Index, item.Data);
                }

                foreach (var item in removedItems)
                {
                    Guard.IsInRangeFor(item.Index, (IReadOnlyList<IImage>)Images, nameof(Images));
                    Images.RemoveAt(item.Index);
                }
            });
        }

        /// <inheritdoc />
        public string Id => _album.Id;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this album.
        /// </summary>
        public IReadOnlyList<ICoreAlbum> Sources => _album.GetSources<ICoreAlbum>();

        /// <summary>
        /// The merged sources for this album.
        /// </summary>
        IReadOnlyList<ICoreAlbum> IMerged<ICoreAlbum>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public TimeSpan Duration => _album.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _album.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _album.AddedAt;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _album.Genres;

        /// <summary>
        /// The tracks for this album.
        /// </summary>
        public ObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public string Name => _album.Name;

        /// <inheritdoc />
        public int TotalTracksCount => _album.TotalTracksCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _album.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _album.TotalImageCount;

        /// <inheritdoc />
        public Uri? Url => _album.Url;

        /// <inheritdoc />
        public DateTime? DatePublished => _album.DatePublished;

        /// <inheritdoc />
        public string? Description => _album.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _album.PlaybackState;

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => _album.IsPlayAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _album.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _album.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _album.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDatePublishedAsyncAvailable => _album.IsChangeDatePublishedAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _album.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task PauseAsync() => _album.PauseAsync();

        /// <inheritdoc />
        public async Task PlayAsync()
        {
            if (MainViewModel.Singleton?.ActiveDevice?.Type == DeviceType.Remote)
            {
                await _album.PlayAsync();
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

                    // TODO: Use core of active device.
                    var mediaSource = await SourceCores[0].GetMediaSource(item.Model.GetSources<ICoreTrack>()[0]);

                    if (mediaSource is null)
                        continue;

                    _playbackHandler.InsertNext(index, mediaSource);
                }

                if (MainViewModel.Singleton?.LocalDevice?.Model is StrixDevice device)
                {
                    device.SetPlaybackData(_album, Tracks[0].Model);
                }

                // Play the first thing in the album.
                await _playbackHandler.PlayFromNext(0);
            }
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _album.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _album.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _album.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _album.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _album.IsAddTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index) => _album.IsAddArtistItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index) => _album.IsAddGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _album.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index) => _album.IsRemoveGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index) => _album.IsRemoveTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index) => _album.IsRemoveArtistItemAvailable(index);

        /// <inheritdoc />
        public Task ChangeDatePublishedAsync(DateTime datePublished) => _album.ChangeDatePublishedAsync(datePublished);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _album.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _album.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _album.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _album.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _album.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _album.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _album.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await _album.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _album.GetImagesAsync(limit, offset);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            return _album.GetArtistItemsAsync(limit, offset);
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            var items = await _album.GetImagesAsync(limit, Images.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    Images.Add(item);
                }
            });
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            var items = await _album.GetArtistItemsAsync(limit, Artists.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    if (item is IArtist artist)
                    {
                        Artists.Add(new ArtistViewModel(artist));
                    }
                }
            });
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

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

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbum other) => _album.Equals(other);
    }
}
