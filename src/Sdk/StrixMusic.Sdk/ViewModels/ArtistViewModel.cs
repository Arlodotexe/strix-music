using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
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
                Images = new ObservableCollection<IImage>();
                Tracks = new ObservableCollection<TrackViewModel>();
                Albums = new ObservableCollection<IAlbumCollectionItem>();
            }

            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);
            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayAlbumCollectionAsyncCommand = new AsyncRelayCommand(PlayAlbumCollectionAsync);
            PauseAlbumCollectionAsyncCommand = new AsyncRelayCommand(PauseAlbumCollectionAsync);
            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(PlayTrack);

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
            LastPlayedChanged += OnLastPlayedChanged;

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

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
            LastPlayedChanged -= OnLastPlayedChanged;

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

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
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _artist.LastPlayedChanged += value;
            remove => _artist.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _artist.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _artist.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => _artist.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => _artist.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _artist.IsChangeNameAsyncAvailableChanged += value;
            remove => _artist.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _artist.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _artist.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _artist.IsChangeDurationAsyncAvailableChanged += value;
            remove => _artist.IsChangeDurationAsyncAvailableChanged -= value;
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

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable));

        private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPauseAlbumCollectionAsyncAvailable));

        private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPlayAlbumCollectionAsyncAvailable));

        private void ArtistViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void ArtistViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Tracks.ChangeCollection(addedItems, removedItems, item => new TrackViewModel(item.Data));
            });
        }

        private void ArtistViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Albums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IAlbum album => new AlbumViewModel(album),
                    IAlbumCollection collection => new AlbumCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            });
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

        /// <inheritdoc cref="IPlayableBase.Duration" />
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
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// The tracks released by this artist.
        /// </summary>
        public ObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

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
        public bool IsPlayTrackCollectionAsyncAvailable => _artist.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _artist.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => _artist.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => _artist.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _artist.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _artist.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _artist.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _artist.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _artist.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync() => _artist.PlayAlbumCollectionAsync();

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync() => _artist.PauseAlbumCollectionAsync();

        /// <summary>
        /// Plays a single track from this collection.
        /// </summary>
        /// <param name="track"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PlayTrack(ITrack track)
        {
            throw new NotImplementedException();
        }

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
            var items = await _artist.GetAlbumItemsAsync(limit, Albums.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    if (item is IAlbum album)
                    {
                        _ = Threading.OnPrimaryThread(() => Albums.Add(new AlbumViewModel(album)));
                    }
                }
            });
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            var items = await _artist.GetImagesAsync(limit, Images.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    Images.Add(item);
                }
            });
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            var items = await GetTracksAsync(limit, Tracks.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    Tracks.Add(new TrackViewModel(item));
                }
            });
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
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

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
