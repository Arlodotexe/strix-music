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
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreArtist"/>.
    /// </summary>
    public class ArtistViewModel : ObservableObject, IArtist, IAlbumCollectionViewModel, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly MergedArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="artist">The <see cref="MergedArtist"/> to wrap.</param>
        public ArtistViewModel(MergedArtist artist)
        {
            _artist = artist;

            SourceCores = _artist.GetSourceCores<ICoreArtist>().Select(MainViewModel.GetLoadedCore).ToList();

            if (_artist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_artist.RelatedItems);

            Tracks = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<TrackViewModel>());
            Albums = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IAlbumCollectionItem>());
            Images = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IImage>());

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += ArtistPlaybackStateChanged;
            DescriptionChanged += ArtistDescriptionChanged;
            NameChanged += ArtistNameChanged;
            UrlChanged += ArtistUrlChanged;
            AlbumItemsCountChanged += Artist_AlbumItemsCountChanged;
            TrackItemsCountChanged += ArtistOnTrackItemsCountChanged;
            ImagesCountChanged += ArtistViewModel_ImagesCountChanged;
            ImagesChanged += ArtistViewModel_ImagesChanged;
            AlbumItemsChanged += ArtistViewModel_AlbumItemsChanged;
            TrackItemsChanged += ArtistViewModel_TrackItemsChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= ArtistPlaybackStateChanged;
            DescriptionChanged -= ArtistDescriptionChanged;
            NameChanged -= ArtistNameChanged;
            UrlChanged -= ArtistUrlChanged;
            AlbumItemsCountChanged -= Artist_AlbumItemsCountChanged;
            TrackItemsCountChanged -= ArtistOnTrackItemsCountChanged;
            ImagesCountChanged -= ArtistViewModel_ImagesCountChanged;
            ImagesChanged -= ArtistViewModel_ImagesChanged;
            AlbumItemsChanged -= ArtistViewModel_AlbumItemsChanged;
            TrackItemsChanged -= ArtistViewModel_TrackItemsChanged;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _artist.DurationChanged += value;

            remove => _artist.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _artist.PlaybackStateChanged += value;

            remove => _artist.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _artist.NameChanged += value;

            remove => _artist.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _artist.DescriptionChanged += value;

            remove => _artist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _artist.UrlChanged += value;

            remove => _artist.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> TrackItemsCountChanged
        {
            add => _artist.TrackItemsCountChanged += value;
            remove => _artist.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> AlbumItemsCountChanged
        {
            add => _artist.AlbumItemsCountChanged += value;
            remove => _artist.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> ImagesCountChanged
        {
            add => _artist.ImagesCountChanged += value;
            remove => _artist.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage> ImagesChanged
        {
            add => _artist.ImagesChanged += value;
            remove => _artist.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem> AlbumItemsChanged
        {
            add => _artist.AlbumItemsChanged += value;
            remove => _artist.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack> TrackItemsChanged
        {
            add => _artist.TrackItemsChanged += value;
            remove => _artist.TrackItemsChanged -= value;
        }

        private void ArtistUrlChanged(object sender, Uri? e) => Url = e;

        private void ArtistNameChanged(object sender, string e) => Name = e;

        private void ArtistDescriptionChanged(object sender, string? e) => Description = e;

        private void ArtistPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void ArtistOnTrackItemsCountChanged(object sender, int e) => TotalTracksCount = e;

        private void Artist_AlbumItemsCountChanged(object sender, int e) => TotalAlbumItemsCount = e;

        private void ArtistViewModel_ImagesCountChanged(object sender, int e) => TotalImageCount = e;

        private void ArtistViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
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

        private void ArtistViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
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

        private void ArtistViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IAlbum album:
                        Albums.Insert(item.Index, new AlbumViewModel(album));
                        break;
                    case IAlbumCollection collection:
                        Albums.Insert(item.Index, new AlbumCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IAlbumCollectionItem>)Albums, nameof(Albums));
                Albums.RemoveAt(item.Index);
            }
        }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => this.GetSources<ICoreArtist>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> ISdkMember<ICoreArtist>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public string Id => _artist.Id;

        /// <inheritdoc cref="IPlayable.Duration" />
        public TimeSpan Duration => _artist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// The artistViewModel's albums.
        /// </summary>
        public SynchronizedObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// The tracks released by this artistViewModel.
        /// </summary>
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _artist.Genres;

        /// <inheritdoc />
        public string Name
        {
            get => _artist.Name;
            private set => SetProperty(_artist.Name, value, _artist, (m, v) => m.Name = v);
        }

        /// <inheritdoc />
        public int TotalAlbumItemsCount
        {
            get => _artist.TotalAlbumItemsCount;
            private set => SetProperty(_artist.TotalAlbumItemsCount, value, _artist, (m, v) => m.TotalAlbumItemsCount = v);
        }

        /// <inheritdoc />
        public int TotalTracksCount
        {
            get => _artist.TotalTracksCount;
            private set => SetProperty(_artist.TotalTracksCount, value, _artist, (m, v) => m.TotalTracksCount = v);
        }

        /// <inheritdoc />
        public int TotalImageCount
        {
            get => _artist.TotalTracksCount;
            private set => SetProperty(_artist.TotalTracksCount, value, _artist, (m, v) => m.TotalTracksCount = v);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _artist.Url;
            private set => SetProperty(_artist.Url, value, _artist, (m, v) => m.Url = v);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _artist.Description;
            private set => SetProperty(_artist.Description, value, _artist, (m, v) => m.Description = v);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _artist.PlaybackState;
            private set => SetProperty(_artist.PlaybackState, value, _artist, (m, v) => m.PlaybackState = v);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => _artist.IsPlayAsyncSupported;
            set => SetProperty(_artist.IsPlayAsyncSupported, value, _artist, (m, v) => m.IsPlayAsyncSupported = v);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => _artist.IsPauseAsyncSupported;
            set => SetProperty(_artist.IsPauseAsyncSupported, value, _artist, (m, v) => m.IsPauseAsyncSupported = v);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => _artist.IsChangeNameAsyncSupported;
            set => SetProperty(_artist.IsChangeNameAsyncSupported, value, _artist, (m, v) => m.IsChangeNameAsyncSupported = v);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _artist.IsChangeDescriptionAsyncSupported;
            set => SetProperty(_artist.IsChangeDescriptionAsyncSupported, value, _artist, (m, v) => m.IsChangeDescriptionAsyncSupported = v);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => _artist.IsChangeDurationAsyncSupported;
            set => SetProperty(_artist.IsChangeDurationAsyncSupported, value, _artist, (m, v) => m.IsChangeDurationAsyncSupported = v);
        }

        /// <inheritdoc />
        public Task PlayAsync() => _artist.PlayAsync();

        /// <inheritdoc />
        public Task PauseAsync() => _artist.PauseAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _artist.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _artist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _artist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _artist.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _artist.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _artist.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _artist.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _artist.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _artist.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _artist.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => _artist.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _artist.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _artist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            foreach (var item in await _artist.GetAlbumItemsAsync(limit, Albums.Count))
            {
                if (item is IAlbum album)
                {
                    Albums.Add(new AlbumViewModel(album));
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await _artist.GetImagesAsync(limit, Images.Count))
            {
                Images.Add(item);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _artist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _artist.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _artist.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _artist.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _artist.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _artist.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _artist.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <summary>
        /// Attempts to play the artistViewModel.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the artistViewModel, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the artistViewModel, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the artistViewModel, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the artistViewModel, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }
    }
}
