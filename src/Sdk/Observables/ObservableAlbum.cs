using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IAlbum"/>.
    /// </summary>
    public class ObservableAlbum : ObservableMergeableObject<IAlbum>
    {
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAlbum"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public ObservableAlbum(IAlbum album)
        {
            _album = album;

            SourceCore = MainViewModel.GetLoadedCore(_album.SourceCore);

            Images = new ObservableCollection<IImage>(_album.Images);
            Tracks = new ObservableCollection<ObservableTrack>(_album.Tracks.Select(x => new ObservableTrack(x)));

            if (_album.RelatedItems != null)
                RelatedItems = new ObservableCollectionGroup(_album.RelatedItems);

            _artist = new ObservableArtist(_album.Artist);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeImagesAsyncCommand = new AsyncRelayCommand<IReadOnlyList<IImage>>(ChangeImagesAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _album.PlaybackStateChanged += Album_PlaybackStateChanged;
            _album.DescriptionChanged += Album_DescriptionChanged;
            _album.DatePublishedChanged += Album_DatePublishedChanged;
            _album.NameChanged += Album_NameChanged;
            _album.UrlChanged += Album_UrlChanged;
            _album.ImagesChanged += Album_ImagesChanged;
            _album.TracksChanged += Album_TracksChanged;
        }

        private void DetachEvents()
        {
            _album.PlaybackStateChanged -= Album_PlaybackStateChanged;
            _album.DescriptionChanged -= Album_DescriptionChanged;
            _album.DatePublishedChanged -= Album_DatePublishedChanged;
            _album.NameChanged -= Album_NameChanged;
            _album.UrlChanged -= Album_UrlChanged;
            _album.ImagesChanged += Album_ImagesChanged;
            _album.TracksChanged -= Album_TracksChanged;
        }

        private void Album_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                Tracks.Insert(item.Index, new ObservableTrack(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Tracks.RemoveAt(item.Index);
            }
        }

        private void Album_ImagesChanged(object sender, CollectionChangedEventArgs<IImage> e)
        {
            foreach (var item in e.AddedItems)
            {
                Images.Insert(item.Index, item.Data);
            }

            foreach (var item in e.RemovedItems)
            {
                Images.RemoveAt(item.Index);
            }
        }

        private void Album_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Album_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Album_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void Album_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Album_DatePublishedChanged(object sender, DateTime? e)
        {
            DatePublished = e;
        }

        /// <inheritdoc cref="ITrackCollection.Tracks"/>
        public ObservableCollection<ObservableTrack> Tracks { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTrackCounts => _album.TotalTracksCount;

        private ObservableArtist _artist;

        /// <inheritdoc cref="IAlbum.Artist"/>
        public ObservableArtist Artist
        {
            get => _artist;
            set => SetProperty(ref _artist, value);
        }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _album.Id;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public ObservableCore SourceCore { get; }

        /// <inheritdoc cref="IPlayable.Duration"/>
        public TimeSpan Duration => _album.Duration;

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name
        {
            get => _album.Name;
            private set => SetProperty(() => _album.Name, value);
        }

        /// <inheritdoc cref="IPlayable.Images"/>
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url
        {
            get => _album.Url;
            private set => SetProperty(() => _album.Url, value);
        }

        /// <inheritdoc cref="IAlbum.DatePublished"/>
        public DateTime? DatePublished
        {
            get => _album.DatePublished;
            set => SetProperty(() => _album.DatePublished, value);
        }

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description
        {
            get => _album.Description;
            private set => SetProperty(() => _album.Description, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _album.PlaybackState;
            private set => SetProperty(() => _album.PlaybackState, value);
        }

        /// <inheritdoc cref="IPlayable.IsPlayAsyncSupported"/>
        public bool IsPlayAsyncSupported
        {
            get => _album.IsPlayAsyncSupported;
            set => SetProperty(() => _album.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsPauseAsyncSupported"/>
        public bool IsPauseAsyncSupported
        {
            get => _album.IsPauseAsyncSupported;
            set => SetProperty(() => _album.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeNameAsyncSupported"/>
        public bool IsChangeNameAsyncSupported
        {
            get => _album.IsChangeNameAsyncSupported;
            set => SetProperty(() => _album.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeImagesAsyncSupported"/>
        public bool IsChangeImagesAsyncSupported
        {
            get => _album.IsChangeImagesAsyncSupported;
            set => SetProperty(() => _album.IsChangeImagesAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeDescriptionAsyncSupported"/>
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _album.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _album.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc cref="IAlbum.IsChangeDatePublishedAsyncSupported"/>
        public bool IsChangeDatePublishedAsyncSupported
        {
            get => _album.IsChangeDatePublishedAsyncSupported;
            set => SetProperty(() => _album.IsChangeDatePublishedAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.IsChangeDurationAsyncSupported"/>
        public bool IsChangeDurationAsyncSupported
        {
            get => _album.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _album.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _album.PlaybackStateChanged += value;

            remove => _album.PlaybackStateChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.NameChanged"/>
        public event EventHandler<string>? NameChanged
        {
            add => _album.NameChanged += value;

            remove => _album.NameChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.DescriptionChanged"/>
        public event EventHandler<string?> DescriptionChanged
        {
            add => _album.DescriptionChanged += value;

            remove => _album.DescriptionChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.UrlChanged"/>
        public event EventHandler<Uri?> UrlChanged
        {
            add => _album.UrlChanged += value;

            remove => _album.UrlChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged
        {
            add => _album.ImagesChanged += value;

            remove => _album.ImagesChanged -= value;
        }

        /// <inheritdoc cref="ITrackCollection.TracksChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add => _album.TracksChanged += value;

            remove => _album.TracksChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.DurationChanged"/>
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _album.DurationChanged += value;

            remove => _album.DurationChanged -= value;
        }

        /// <inheritdoc cref="IAlbum.DatePublishedChanged"/>
        public event EventHandler<DateTime?> DatePublishedChanged
        {
            add => _album.DatePublishedChanged += value;

            remove => _album.DatePublishedChanged -= value;
        }

        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        public Task PauseAsync() => _album.PauseAsync();

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTracksCount => _album.TotalTracksCount;

        /// <inheritdoc cref="IAlbum.RelatedItems"/>
        public ObservableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync() => _album.PlayAsync();

        /// <inheritdoc cref="IPlayable.ChangeNameAsync"/>
        public Task ChangeNameAsync(string name) => _album.ChangeNameAsync(name);

        /// <inheritdoc cref="IPlayable.ChangeImagesAsync"/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images) => _album.ChangeImagesAsync(images);

        /// <inheritdoc cref="IPlayable.ChangeDescriptionAsync"/>
        public Task ChangeDescriptionAsync(string? description) => _album.ChangeDescriptionAsync(description);

        /// <inheritdoc cref="IPlayable.ChangeDurationAsync"/>
        public Task ChangeDurationAsync(TimeSpan duration) => _album.ChangeDurationAsync(duration);

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync(int, int)"/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0) => _album.PopulateTracksAsync(limit, offset);

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
        /// Attempts to change the images for the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeImagesAsyncCommand { get; }

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
