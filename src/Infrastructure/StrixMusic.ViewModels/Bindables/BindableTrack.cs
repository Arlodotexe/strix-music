using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class BindableTrack : BindableMergeableObject<ITrack>
    {
        private readonly ITrack _track;

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ITrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        public BindableTrack(ITrack track)
        {
            _track = track;

            if (_track.Album != null)
                Album = new BindableAlbum(_track.Album);

            Genres = new ObservableCollection<string>(_track.Genres);
            Artists = new ObservableCollection<BindableArtist>(_track.Artists.Select(x => new BindableArtist(x)));
            Images = new ObservableCollection<IImage>(_track.Images);
            RelatedItems = new ObservableCollection<BindableCollectionGroup>(_track.RelatedItems.Select(x => new BindableCollectionGroup(x)));
            SourceCore = new BindableCore(_track.SourceCore);

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _track.AlbumChanged += Track_AlbumChanged;
            _track.ArtistsChanged += Track_ArtistsChanged;
            _track.DatePublishedChanged += Track_DatePublishedChanged;
            _track.DescriptionChanged += Track_DescriptionChanged;
            _track.GenresChanged += Track_GenresChanged;
            _track.IsExplicitChanged += Track_IsExplicitChanged;
            _track.LanguageChanged += Track_LanguageChanged;
            _track.LyricsChanged += Track_LyricsChanged;
            _track.NameChanged += Track_NameChanged;
            _track.PlaybackStateChanged += Track_PlaybackStateChanged;
            _track.PlayCountChanged += Track_PlayCountChanged;
            _track.TrackNumberChanged += Track_TrackNumberChanged;
            _track.UrlChanged += Track_UrlChanged;
            _track.ImagesChanged += Track_ImagesChanged;
            _track.RelatedItemsChanged += Track_RelatedItemsChanged;
        }

        private void DetachEvents()
        {
            _track.AlbumChanged -= Track_AlbumChanged;
            _track.ArtistsChanged -= Track_ArtistsChanged;
            _track.DatePublishedChanged -= Track_DatePublishedChanged;
            _track.DescriptionChanged -= Track_DescriptionChanged;
            _track.GenresChanged -= Track_GenresChanged;
            _track.IsExplicitChanged -= Track_IsExplicitChanged;
            _track.LanguageChanged -= Track_LanguageChanged;
            _track.LyricsChanged -= Track_LyricsChanged;
            _track.NameChanged -= Track_NameChanged;
            _track.PlaybackStateChanged -= Track_PlaybackStateChanged;
            _track.PlayCountChanged -= Track_PlayCountChanged;
            _track.TrackNumberChanged -= Track_TrackNumberChanged;
            _track.UrlChanged -= Track_UrlChanged;
            _track.ImagesChanged -= Track_ImagesChanged;
            _track.RelatedItemsChanged -= Track_RelatedItemsChanged;
        }

        private void Track_RelatedItemsChanged(object sender, CollectionChangedEventArgs<IPlayableCollectionGroup> e)
        {
            foreach (var item in e.AddedItems)
            {
                RelatedItems.Add(new BindableCollectionGroup(item));
            }

            foreach (var item in e.RemovedItems)
            {
                RelatedItems.Remove(new BindableCollectionGroup(item));
            }
        }

        private void Track_ImagesChanged(object sender, CollectionChangedEventArgs<IImage> e)
        {
            foreach (var item in e.AddedItems)
            {
                Images.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Images.Remove(item);
            }
        }

        private void Track_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Track_TrackNumberChanged(object sender, int? e)
        {
            TrackNumber = e;
        }

        private void Track_PlayCountChanged(object sender, int? e)
        {
            PlayCount = e;
        }

        private void Track_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Track_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Track_LyricsChanged(object sender, ILyrics? e)
        {
            Lyrics = e;
        }

        private void Track_LanguageChanged(object sender, string? e)
        {
            Language = e;
        }

        private void Track_IsExplicitChanged(object sender, bool e)
        {
            IsExplicit = e;
        }

        private void Track_GenresChanged(object sender, CollectionChangedEventArgs<string> e)
        {
            foreach (var item in e.AddedItems)
            {
                Genres.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Genres.Remove(item);
            }
        }

        private void Track_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void Track_DatePublishedChanged(object sender, DateTime? e)
        {
            DatePublished = e;
        }

        private void Track_ArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Artists.Add(new BindableArtist(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Artists.Remove(new BindableArtist(item));
            }
        }

        private void Track_AlbumChanged(object sender, IAlbum? e)
        {
            if (e != null)
                Album = new BindableAlbum(e);
            else
                Album = null;
        }

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url
        {
            get => _track.Url;
            set => SetProperty(() => _track.Url, value);
        }

        /// <inheritdoc cref="ITrack.Type"/>
        public string Type => _track.Type;

        /// <inheritdoc cref="ITrack.Artists"/>
        public ObservableCollection<BindableArtist> Artists { get; }

        private BindableAlbum? _album;

        /// <inheritdoc cref="ITrack.Album"/>
        public BindableAlbum? Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }

        /// <inheritdoc cref="ITrack.DatePublished"/>
        public DateTime? DatePublished
        {
            get => _track.DatePublished;
            set => SetProperty(() => _track.DatePublished, value);
        }

        /// <inheritdoc cref="ITrack.Genres"/>
        public ObservableCollection<string> Genres { get; }

        /// <inheritdoc cref="ITrack.TrackNumber"/>
        public int? TrackNumber
        {
            get => _track.TrackNumber;
            set => SetProperty(() => _track.TrackNumber, value);
        }

        /// <inheritdoc cref="ITrack.PlayCount"/>
        public int? PlayCount
        {
            get => _track.PlayCount;
            set => SetProperty(() => _track.PlayCount, value);
        }

        /// <inheritdoc cref="ITrack.Language"/>
        public string? Language
        {
            get => _track.Language;
            set => SetProperty(() => _track.Language, value);
        }

        /// <inheritdoc cref="ITrack.Lyrics"/>
        public ILyrics? Lyrics
        {
            get => _track.Lyrics;
            set => SetProperty(() => _track.Lyrics, value);
        }

        /// <inheritdoc cref="ITrack.IsExplicit"/>
        public bool IsExplicit
        {
            get => _track.IsExplicit;
            set => SetProperty(() => _track.IsExplicit, value);
        }

        /// <inheritdoc cref="IPlayable.Duration"/>
        public TimeSpan Duration => _track.Duration;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public BindableCore SourceCore { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _track.Id;

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name
        {
            get => _track.Name;
            set => SetProperty(() => _track.Name, value);
        }

        /// <inheritdoc cref="IPlayable.Images"/>
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description
        {
            get => _track.Description;
            set => SetProperty(() => _track.Description, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _track.PlaybackState;
            set => SetProperty(() => _track.PlaybackState, value);
        }

        /// <inheritdoc cref="IRelatedCollectionGroups.RelatedItems"/>
        public ObservableCollection<BindableCollectionGroup> RelatedItems { get; }

        /// <inheritdoc cref="IRelatedCollectionGroups.TotalRelatedItemsCount"/>
        public int TotalRelatedItemsCount => _track.TotalRelatedItemsCount;

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _track.PlaybackStateChanged += value;
            }

            remove
            {
                _track.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.ArtistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged
        {
            add
            {
                _track.ArtistsChanged += value;
            }

            remove
            {
                _track.ArtistsChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.GenresChanged"/>
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged
        {
            add
            {
                _track.GenresChanged += value;
            }

            remove
            {
                _track.GenresChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.AlbumChanged"/>
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add
            {
                _track.AlbumChanged += value;
            }

            remove
            {
                _track.AlbumChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.DatePublishedChanged"/>
        public event EventHandler<DateTime?> DatePublishedChanged
        {
            add
            {
                _track.DatePublishedChanged += value;
            }

            remove
            {
                _track.DatePublishedChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.TrackNumberChanged"/>
        public event EventHandler<int?> TrackNumberChanged
        {
            add
            {
                _track.TrackNumberChanged += value;
            }

            remove
            {
                _track.TrackNumberChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.PlayCountChanged"/>
        public event EventHandler<int?> PlayCountChanged
        {
            add
            {
                _track.PlayCountChanged += value;
            }

            remove
            {
                _track.PlayCountChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.LanguageChanged"/>
        public event EventHandler<string?> LanguageChanged
        {
            add
            {
                _track.LanguageChanged += value;
            }

            remove
            {
                _track.LanguageChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.LyricsChanged"/>
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add
            {
                _track.LyricsChanged += value;
            }

            remove
            {
                _track.LyricsChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrack.IsExplicitChanged"/>
        public event EventHandler<bool> IsExplicitChanged
        {
            add
            {
                _track.IsExplicitChanged += value;
            }

            remove
            {
                _track.IsExplicitChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.NameChanged"/>
        public event EventHandler<string> NameChanged
        {
            add
            {
                _track.NameChanged += value;
            }

            remove
            {
                _track.NameChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.DescriptionChanged"/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                _track.DescriptionChanged += value;
            }

            remove
            {
                _track.DescriptionChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.UrlChanged"/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                _track.UrlChanged += value;
            }

            remove
            {
                _track.UrlChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged
        {
            add
            {
                _track.ImagesChanged += value;
            }

            remove
            {
                _track.ImagesChanged -= value;
            }
        }

        /// <inheritdoc cref="IRelatedCollectionGroups.RelatedItemsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged
        {
            add
            {
                _track.RelatedItemsChanged += value;
            }

            remove
            {
                _track.RelatedItemsChanged -= value;
            }
        }

        /// <summary>
        /// Attempts to pause the track, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        public Task PauseAsync()
        {
            return _track.PauseAsync();
        }

        /// <summary>
        /// Attempts to play the track.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync()
        {
            return _track.PlayAsync();
        }

        /// <inheritdoc cref="IRelatedCollectionGroups.PopulateRelatedItemsAsync(int, int)"/>
        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            return _track.PopulateRelatedItemsAsync(limit, offset);
        }
    }
}
