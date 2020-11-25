using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class TrackViewModel : ObservableObject, ITrack, IArtistCollectionViewModel, IImageCollectionViewModel
    {
        private IAlbum? _album;

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ITrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        public TrackViewModel(ITrack track)
        {
            Model = track;

            SourceCores = Model.GetSourceCores<ICoreTrack>().Select(MainViewModel.GetLoadedCore).ToList();

            if (Model.Album != null)
                Album = new AlbumViewModel(Model.Album);

            if (Model.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(Model.RelatedItems);

            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IArtistCollectionItem>());
            Images = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IImage>());

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

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
            UrlChanged += Track_UrlChanged;

            ArtistItemsCountChanged += ModelOnArtistItemsCountChanged;
            ArtistItemsChanged += TrackViewModel_ArtistItemsChanged;
            ImagesCountChanged += TrackViewModel_ImagesCountChanged;
            ImagesChanged += TrackViewModel_ImagesChanged;
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
            UrlChanged -= Track_UrlChanged;

            ArtistItemsCountChanged += ModelOnArtistItemsCountChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => Model.PlaybackStateChanged += value;

            remove => Model.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add => Model.AlbumChanged += value;

            remove => Model.AlbumChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int?> TrackNumberChanged
        {
            add => Model.TrackNumberChanged += value;

            remove => Model.TrackNumberChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo?> LanguageChanged
        {
            add => Model.LanguageChanged += value;

            remove => Model.LanguageChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add => Model.LyricsChanged += value;

            remove => Model.LyricsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool> IsExplicitChanged
        {
            add => Model.IsExplicitChanged += value;

            remove => Model.IsExplicitChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => Model.NameChanged += value;

            remove => Model.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => Model.DescriptionChanged += value;

            remove => Model.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => Model.UrlChanged += value;

            remove => Model.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => Model.DurationChanged += value;

            remove => Model.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> ArtistItemsCountChanged
        {
            add => Model.ArtistItemsCountChanged += value;
            remove => Model.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> ImagesCountChanged
        {
            add => Model.ImagesCountChanged += value;
            remove => Model.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage> ImagesChanged
        {
            add => Model.ImagesChanged += value;
            remove => Model.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem> ArtistItemsChanged
        {
            add => Model.ArtistItemsChanged += value;
            remove => Model.ArtistItemsChanged -= value;
        }

        private void Track_UrlChanged(object sender, Uri? e) => Url = e;

        private void Track_TrackNumberChanged(object sender, int? e) => TrackNumber = e;

        private void Track_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void Track_NameChanged(object sender, string e) => Name = e;

        private void Track_LyricsChanged(object sender, ILyrics? e) => Lyrics = e;

        private void Track_LanguageChanged(object sender, CultureInfo? e) => Language = e;

        private void Track_IsExplicitChanged(object sender, bool e) => IsExplicit = e;

        private void Track_DescriptionChanged(object sender, string? e) => Description = e;

        private void Track_AlbumChanged(object sender, IAlbum? e) => Album = e != null ? new AlbumViewModel(e) : null;

        private void ModelOnArtistItemsCountChanged(object sender, int e) => TotalArtistItemsCount = e;

        private void TrackViewModel_ImagesCountChanged(object sender, int e) => TotalImageCount = e;

        private void TrackViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
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

        private void TrackViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IArtist artist:
                        Artists.Insert(item.Index, new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.Insert(item.Index, new ArtistCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IArtistCollectionItem>)Artists, nameof(Artists));
                Artists.RemoveAt(item.Index);
            }
        }

        /// <summary>
        /// The wrapped model for this <see cref="TrackViewModel"/>.
        /// </summary>
        internal ITrack Model { get; }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this model
        /// </summary>
        public IReadOnlyList<ICoreTrack> Sources => Model.GetSources<ICoreTrack>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrack> ISdkMember<ICoreTrack>.Sources => Sources;

        /// <summary>
        /// The artists for this track.
        /// </summary>
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => Model.Genres;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public TrackType Type => Model.Type;

        /// <inheritdoc />
        public TimeSpan Duration => Model.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public string Id => Model.Id;

        /// <inheritdoc />
        public string Name
        {
            get => Model.Name;
            set => SetProperty(() => Model.Name, value);
        }

        /// <inheritdoc />
        public int TotalArtistItemsCount
        {
            get => Model.TotalArtistItemsCount;
            set => SetProperty(() => Model.TotalArtistItemsCount, value);
        }

        /// <inheritdoc />
        public int TotalImageCount
        {
            get => Model.TotalImageCount;
            set => SetProperty(() => Model.TotalImageCount, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => Model.Url;
            set => SetProperty(() => Model.Url, value);
        }

        /// <inheritdoc />
        public IAlbum? Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }

        /// <inheritdoc />
        public int? TrackNumber
        {
            get => Model.TrackNumber;
            set => SetProperty(() => Model.TrackNumber, value);
        }

        /// <inheritdoc/>
        public int? DiscNumber => Model.DiscNumber;

        /// <inheritdoc />
        public CultureInfo? Language
        {
            get => Model.Language;
            set => SetProperty(() => Model.Language, value);
        }

        /// <inheritdoc />
        public ILyrics? Lyrics
        {
            get => Model.Lyrics;
            set => SetProperty(() => Model.Lyrics, value);
        }

        /// <inheritdoc />
        public bool IsExplicit
        {
            get => Model.IsExplicit;
            set => SetProperty(() => Model.IsExplicit, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => Model.Description;
            set => SetProperty(() => Model.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => Model.PlaybackState;
            set => SetProperty(() => Model.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => Model.IsPlayAsyncSupported;
            set => SetProperty(() => Model.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => Model.IsPauseAsyncSupported;
            set => SetProperty(() => Model.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => Model.IsChangeNameAsyncSupported;
            set => SetProperty(() => Model.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => Model.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => Model.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => Model.IsChangeDurationAsyncSupported;
            set => SetProperty(() => Model.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeAlbumAsyncSupported
        {
            get => Model.IsChangeAlbumAsyncSupported;
            set => SetProperty(() => Model.IsChangeAlbumAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeTrackNumberAsyncSupported
        {
            get => Model.IsChangeTrackNumberAsyncSupported;
            set => SetProperty(() => Model.IsChangeTrackNumberAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeLanguageAsyncSupported
        {
            get => Model.IsChangeLanguageAsyncSupported;
            set => SetProperty(() => Model.IsChangeLanguageAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeLyricsAsyncSupported
        {
            get => Model.IsChangeLyricsAsyncSupported;
            set => SetProperty(() => Model.IsChangeLyricsAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeIsExplicitAsyncSupported
        {
            get => Model.IsChangeIsExplicitAsyncSupported;
            set => SetProperty(() => Model.IsChangeIsExplicitAsyncSupported, value);
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => Model.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => Model.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => Model.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => Model.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => Model.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => Model.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public Task ChangeAlbumAsync(IAlbum? album) => Model.ChangeAlbumAsync(album);

        /// <inheritdoc />
        public Task ChangeTrackNumberAsync(int? trackNumber) => Model.ChangeTrackNumberAsync(trackNumber);

        /// <inheritdoc />
        public Task ChangeLanguageAsync(CultureInfo language) => Model.ChangeLanguageAsync(language);

        /// <inheritdoc />
        public Task ChangeLyricsAsync(ILyrics? lyrics) => Model.ChangeLyricsAsync(lyrics);

        /// <inheritdoc />
        public Task ChangeIsExplicitAsync(bool isExplicit) => Model.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc />
        public Task PauseAsync() => Model.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => Model.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => Model.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => Model.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => Model.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => Model.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => Model.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => Model.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => Model.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => Model.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => Model.RemoveArtistAsync(index);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await GetArtistItemsAsync(limit, Artists.Count))
            {
                if (item is IArtist artist)
                {
                    Artists.Add(new ArtistViewModel(artist));
                }

                if (item is IArtistCollection collection)
                {
                    Artists.Add(new ArtistCollectionViewModel(collection));
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await GetImagesAsync(limit, Images.Count))
            {
                Images.Add(item);
            }
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <summary>
        /// Attempts to play the track.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the track, if playing.
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
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }
    }
}
