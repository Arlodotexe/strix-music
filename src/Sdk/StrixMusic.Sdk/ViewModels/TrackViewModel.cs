// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ITrack"/>.
    /// </summary>
    public sealed class TrackViewModel : ObservableObject, ISdkViewModel, ITrack, IArtistCollectionViewModel, IImageCollectionViewModel, IGenreCollectionViewModel
    {
        private readonly SemaphoreSlim _populateArtistsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateGenresMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;
        private readonly ITrack _model;

        /// <summary>
        /// Creates a new instance of <see cref="TrackViewModel"/>.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        internal TrackViewModel(MainViewModel root, ITrack track)
        {
            _syncContext = SynchronizationContext.Current;

            Root = root;
            _model = root.Plugins.ModelPlugins.Track.Execute(track);

            SourceCores = _model.GetSourceCores<ICoreTrack>().Select(root.GetLoadedCore).ToList();

            if (_model.Album != null)
                Album = new AlbumViewModel(root, _model.Album);

            if (_model.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(root, _model.RelatedItems);

            Artists = new ObservableCollection<IArtistCollectionItem>();
            UnsortedArtists = new ObservableCollection<IArtistCollectionItem>();
            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();
            Genres = new ObservableCollection<IGenre>();

            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);
            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);

            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>(x => _model.PlayArtistCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IArtistCollectionItem>()));

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreGenresCommand = new AsyncRelayCommand<int>(PopulateMoreGenresAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitArtistCollectionAsyncCommand = new AsyncRelayCommand(InitArtistCollectionAsync);
            InitGenreCollectionAsyncCommand = new AsyncRelayCommand(InitGenreCollectionAsync);
            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);

            ChangeArtistCollectionSortingTypeCommand = new RelayCommand<ArtistSortingType>(x => SortArtistCollection(x, CurrentArtistSortingDirection));
            ChangeArtistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortArtistCollection(CurrentArtistSortingType, x));

            AttachEvents();
        }

        private void AttachEvents()
        {
            AlbumChanged += Track_AlbumChanged;
            DescriptionChanged += Track_DescriptionChanged;
            IsExplicitChanged += Track_IsExplicitChanged;
            LanguageChanged += Track_LanguageChanged;
            LyricsChanged += Track_LyricsChanged;
            NameChanged += Track_NameChanged;
            PlaybackStateChanged += Track_PlaybackStateChanged;
            TrackNumberChanged += Track_TrackNumberChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged += OnDownloadInfoChanged);

            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            ArtistItemsCountChanged += OnArtistItemsCountChanged;
            ArtistItemsChanged += TrackViewModel_ArtistItemsChanged;
            ImagesCountChanged += OnImagesCountChanged;
            ImagesChanged += TrackViewModel_ImagesChanged;
            GenresChanged += TrackViewModel_GenresChanged;
            GenresCountChanged += OnGenresCountChanged;
            UrlsChanged += TrackViewModel_UrlsChanged;
            UrlsCountChanged += OnUrlsCountChanged;

            _model.PlaybackStateChanged += OnPlaybackStateChanged;
        }

        private void DetachEvents()
        {
            AlbumChanged -= Track_AlbumChanged;
            DescriptionChanged -= Track_DescriptionChanged;
            IsExplicitChanged -= Track_IsExplicitChanged;
            LanguageChanged -= Track_LanguageChanged;
            LyricsChanged -= Track_LyricsChanged;
            NameChanged -= Track_NameChanged;
            PlaybackStateChanged -= Track_PlaybackStateChanged;
            TrackNumberChanged -= Track_TrackNumberChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            ArtistItemsCountChanged -= OnArtistItemsCountChanged;
            ArtistItemsChanged -= TrackViewModel_ArtistItemsChanged;
            ImagesCountChanged -= OnImagesCountChanged;
            ImagesChanged -= TrackViewModel_ImagesChanged;
            GenresChanged -= TrackViewModel_GenresChanged;
            GenresCountChanged -= OnGenresCountChanged;
            UrlsChanged -= TrackViewModel_UrlsChanged;
            UrlsCountChanged -= OnUrlsCountChanged;

            _model.PlaybackStateChanged -= OnPlaybackStateChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => _model.DownloadInfoChanged += value;
            remove => _model.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IAlbum?>? AlbumChanged
        {
            add => _model.AlbumChanged += value;
            remove => _model.AlbumChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int?>? TrackNumberChanged
        {
            add => _model.TrackNumberChanged += value;
            remove => _model.TrackNumberChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo?>? LanguageChanged
        {
            add => _model.LanguageChanged += value;
            remove => _model.LanguageChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ILyrics?>? LyricsChanged
        {
            add => _model.LyricsChanged += value;
            remove => _model.LyricsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsExplicitChanged
        {
            add => _model.IsExplicitChanged += value;
            remove => _model.IsExplicitChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _model.NameChanged += value;
            remove => _model.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _model.DescriptionChanged += value;
            remove => _model.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _model.DurationChanged += value;
            remove => _model.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _model.LastPlayedChanged += value;
            remove => _model.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => _model.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => _model.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _model.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _model.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _model.IsChangeNameAsyncAvailableChanged += value;
            remove => _model.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _model.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _model.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _model.IsChangeDurationAsyncAvailableChanged += value;
            remove => _model.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _model.ArtistItemsCountChanged += value;
            remove => _model.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _model.ArtistItemsChanged += value;
            remove => _model.ArtistItemsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _model.ImagesCountChanged += value;
            remove => _model.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _model.ImagesChanged += value;
            remove => _model.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? GenresCountChanged
        {
            add => _model.GenresCountChanged += value;
            remove => _model.GenresCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => _model.GenresChanged += value;
            remove => _model.GenresChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _model.UrlsCountChanged += value;
            remove => _model.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _model.UrlsChanged += value;
            remove => _model.UrlsChanged -= value;
        }

        private void Track_TrackNumberChanged(object sender, int? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TrackNumber)), null);

        private void Track_PlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void Track_NameChanged(object sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void Track_LyricsChanged(object sender, ILyrics? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Lyrics)), null);

        private void Track_LanguageChanged(object sender, CultureInfo? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Language)), null);

        private void Track_IsExplicitChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsExplicit)), null);

        private void Track_DescriptionChanged(object sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void OnLastPlayedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)), null);

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)), null);

        private void Track_AlbumChanged(object sender, IAlbum? e)
        {
            Album = e is null ? null : new AlbumViewModel(Root, e);
            _syncContext.Post(_ => OnPropertyChanged(nameof(Album)), null);
        }

        private void OnArtistItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalArtistItemsCount)), null);

        private void OnImagesCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void OnGenresCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalGenreCount)), null);

        private void OnUrlsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void TrackViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _syncContext.Post(_ =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        }, null);

        private void TrackViewModel_GenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems) => _syncContext.Post(_ =>
        {
            Genres.ChangeCollection(addedItems, removedItems);
        }, null);

        private void TrackViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems) => _syncContext.Post(_ =>
        {
            Urls.ChangeCollection(addedItems, removedItems);
        }, null);

        private void TrackViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems) => _syncContext.Post(_ =>
        {
            if (CurrentArtistSortingType == ArtistSortingType.Unsorted)
            {
                Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(Root, artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            }
            else
            {
                // Preventing index issues during artists emission from the core, also making sure that unordered artists updated. 
                UnsortedArtists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(Root, artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });

                // Avoiding direct assignment to prevent effect on UI.
                foreach (var item in UnsortedArtists)
                {
                    if (!Artists.Contains(item))
                        Artists.Add(item);
                }

                foreach (var item in Artists)
                {
                    if (!UnsortedArtists.Contains(item))
                        Artists.Remove(item);
                }

                SortArtistCollection(CurrentArtistSortingType, CurrentArtistSortingDirection);
            }
        }, null);

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ =>
        {
            PlaybackStateChanged?.Invoke(this, PlaybackState);
            OnPropertyChanged(nameof(PlaybackState));
        }, null);

        ///<inheritdoc />
        public void SortArtistCollection(ArtistSortingType artistSorting, SortDirection sortDirection)
        {
            CurrentArtistSortingType = artistSorting;
            CurrentArtistSortingDirection = sortDirection;

            CollectionSorting.SortArtists(Artists, artistSorting, sortDirection, UnsortedArtists);
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <summary>
        /// The merged sources for this model.
        /// </summary>
        public IReadOnlyList<ICoreTrack> Sources => _model.GetSources<ICoreTrack>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrack> IMerged<ICoreTrack>.Sources => Sources;

        /// <summary>
        /// The artists for this track.
        /// </summary>
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IGenre> Genres { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public ArtistSortingType CurrentArtistSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentArtistSortingDirection { get; private set; }

        /// <inheritdoc />
        public TrackType Type => _model.Type;

        /// <inheritdoc />
        public TimeSpan Duration => _model.Duration;

        /// <inheritdoc />
        public DateTime? AddedAt => _model.AddedAt;

        /// <inheritdoc />
        public DateTime? LastPlayed => _model.LastPlayed;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public string Id => _model.Id;

        /// <inheritdoc />
        public string Name => _model.Name;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _model.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _model.TotalImageCount;

        /// <inheritdoc />
        public int TotalGenreCount => _model.TotalGenreCount;

        /// <inheritdoc />
        public int TotalUrlCount => _model.TotalUrlCount;

        /// <inheritdoc cref="ITrack.Album" />
        public AlbumViewModel? Album { get; private set; }

        /// <inheritdoc />
        IAlbum? ITrack.Album => _model.Album;

        /// <inheritdoc />
        public int? TrackNumber => _model.TrackNumber;

        /// <inheritdoc/>
        public int? DiscNumber => _model.DiscNumber;

        /// <inheritdoc />
        public CultureInfo? Language => _model.Language;

        /// <inheritdoc />
        public ILyrics? Lyrics => _model.Lyrics;

        /// <inheritdoc />
        public bool IsExplicit => _model.IsExplicit;

        /// <inheritdoc />
        public string? Description => _model.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => Root.ActiveDevice?.NowPlaying?.Track?.Id == Id ? Root.ActiveDevice?.PlaybackState ?? PlaybackState.None : PlaybackState.None;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _model.DownloadInfo;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => _model.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => _model.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _model.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _model.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _model.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeAlbumAsyncAvailable => _model.IsChangeAlbumAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeTrackNumberAsyncAvailable => _model.IsChangeTrackNumberAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeLanguageAsyncAvailable => _model.IsChangeLanguageAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeLyricsAsyncAvailable => _model.IsChangeLyricsAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeIsExplicitAsyncAvailable => _model.IsChangeIsExplicitAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _model.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _model.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index) => _model.IsAddGenreAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _model.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _model.IsRemoveArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _model.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index) => _model.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _model.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task ChangeAlbumAsync(IAlbum? album) => _model.ChangeAlbumAsync(album);

        /// <inheritdoc />
        public Task ChangeTrackNumberAsync(int? trackNumber) => _model.ChangeTrackNumberAsync(trackNumber);

        /// <inheritdoc />
        public Task ChangeLanguageAsync(CultureInfo language) => _model.ChangeLanguageAsync(language);

        /// <inheritdoc />
        public Task ChangeLyricsAsync(ILyrics? lyrics) => _model.ChangeLyricsAsync(lyrics);

        /// <inheritdoc />
        public Task ChangeIsExplicitAsync(bool isExplicit) => _model.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync() => _model.PauseArtistCollectionAsync();

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation) => _model.StartDownloadOperationAsync(operation);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _model.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _model.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _model.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _model.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task AddGenreAsync(IGenre genre, int index) => _model.AddGenreAsync(genre, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl genre, int index) => _model.AddUrlAsync(genre, index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _model.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _model.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index) => _model.RemoveGenreAsync(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _model.RemoveUrlAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _model.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _model.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => _model.GetGenresAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _model.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => _model.PlayArtistCollectionAsync(artistItem);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync() => _model.PlayArtistCollectionAsync();

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateArtistsMutex))
            {
                var items = await GetArtistItemsAsync(limit, Artists.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                    {
                        if (item is IArtist artist)
                            Artists.Add(new ArtistViewModel(Root, artist));

                        if (item is IArtistCollection collection)
                            Artists.Add(new ArtistCollectionViewModel(Root, collection));
                    }
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                var items = await GetImagesAsync(limit, Images.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Images.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreGenresAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateGenresMutex))
            {
                var items = await GetGenresAsync(limit, Genres.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Genres.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateUrlsMutex))
            {
                var items = await GetUrlsAsync(limit, Urls.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Urls.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<ArtistSortingType> ChangeArtistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeArtistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreGenresCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <summary>
        /// Command to change the name. It triggers <see cref="ChangeNameAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string> ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Command to change the description. It triggers <see cref="ChangeDescriptionAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Command to change the duration. It triggers <see cref="ChangeDurationAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }

        /// <inheritdoc/>
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc/>
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        /// <inheritdoc/>
        public Task InitArtistCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ArtistCollection(this, cancellationToken);

        /// <inheritdoc/>
        public Task InitGenreCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.GenreCollection(this, cancellationToken);

        /// <inheritdoc/>
        public IAsyncRelayCommand InitArtistCollectionAsyncCommand { get; }

        /// <inheritdoc/>
        public IAsyncRelayCommand InitGenreCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => _model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => _model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrack other) => _model.Equals(other);

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return _model.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _model.DisposeAsync();
        }

        /// <inheritdoc />
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (!IsInitialized)
                return;

            await Task.WhenAll(InitImageCollectionAsync(cancellationToken), InitArtistCollectionAsync(cancellationToken), InitGenreCollectionAsync(cancellationToken));
            IsInitialized = true;
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }
    }
}
