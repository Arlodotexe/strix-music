using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
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
        private readonly IArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="artist">The <see cref="IArtist"/> to wrap.</param>
        public ArtistViewModel(IArtist artist)
        {
            _artist = artist;

            SourceCores = _artist.GetSourceCores<ICoreArtist>().Select(MainViewModel.GetLoadedCore).ToList();

            if (_artist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_artist.RelatedItems);

            using (Threading.PrimaryContext)
            {
                Images = new SynchronizedObservableCollection<IImage>();
                Tracks = new SynchronizedObservableCollection<TrackViewModel>();
                Albums = new SynchronizedObservableCollection<IAlbumCollectionItem>();
            }

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
            LastPlayedChanged += OnLastPlayedChanged;
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
            LastPlayedChanged -= OnLastPlayedChanged;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _artist.DurationChanged += value;

            remove => _artist.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _artist.LastPlayedChanged += value;

            remove => _artist.LastPlayedChanged -= value;
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
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _artist.DescriptionChanged += value;

            remove => _artist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged
        {
            add => _artist.UrlChanged += value;

            remove => _artist.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged
        {
            add => _artist.TrackItemsCountChanged += value;
            remove => _artist.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _artist.AlbumItemsCountChanged += value;
            remove => _artist.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _artist.ImagesCountChanged += value;
            remove => _artist.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _artist.ImagesChanged += value;
            remove => _artist.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => _artist.AlbumItemsChanged += value;
            remove => _artist.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged
        {
            add => _artist.TrackItemsChanged += value;
            remove => _artist.TrackItemsChanged -= value;
        }

        private void ArtistUrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void ArtistNameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void ArtistDescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void ArtistPlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void ArtistOnTrackItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalTracksCount));

        private void Artist_AlbumItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalAlbumItemsCount));

        private void ArtistViewModel_ImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void OnLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

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

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => this.GetSources<ICoreArtist>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> IMerged<ICoreArtist>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public string Id => _artist.Id;

        /// <inheritdoc cref="IPlayable.Duration" />
        public TimeSpan Duration => _artist.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _artist.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _artist.AddedAt;

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
        public string Name => _artist.Name;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _artist.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalTracksCount => _artist.TotalTracksCount;

        /// <inheritdoc />
        public int TotalImageCount => _artist.TotalTracksCount;

        /// <inheritdoc />
        public Uri? Url => _artist.Url;

        /// <inheritdoc />
        public string? Description => _artist.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _artist.PlaybackState;

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => _artist.IsPlayAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _artist.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _artist.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _artist.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _artist.IsChangeDurationAsyncAvailable;

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
        public Task<bool> IsAddImageAvailable(int index) => _artist.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailable(int index) => _artist.IsAddAlbumItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _artist.IsAddTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index) => _artist.IsAddGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _artist.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index) => _artist.IsRemoveTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => _artist.IsRemoveAlbumItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index) => _artist.IsRemoveGenreAvailable(index);

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

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtist other) => _artist.Equals(other);
    }
}
