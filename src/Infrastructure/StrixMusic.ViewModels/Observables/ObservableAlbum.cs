using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
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

            SourceCore = new ObservableCore(_album.SourceCore);

            Images = new ObservableCollection<IImage>(_album.Images);
            Tracks = new ObservableCollection<ObservableTrack>(_album.Tracks.Select(x => new ObservableTrack(x)));
            RelatedItems = new ObservableCollectionGroup(_album.RelatedItems);
            _artist = new ObservableArtist(_album.Artist);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _album.PlaybackStateChanged += Album_PlaybackStateChanged;
            _album.DescriptionChanged += Album_DescriptionChanged;
            _album.NameChanged += Album_NameChanged;
            _album.UrlChanged += Album_UrlChanged;
            _album.ImagesChanged += Album_ImagesChanged;
            _album.TracksChanged += Album_TracksChanged;
        }

        private void DetachEvents()
        {
            _album.PlaybackStateChanged -= Album_PlaybackStateChanged;
            _album.DescriptionChanged -= Album_DescriptionChanged;
            _album.NameChanged -= Album_NameChanged;
            _album.UrlChanged -= Album_UrlChanged;
            _album.ImagesChanged += Album_ImagesChanged;
            _album.TracksChanged -= Album_TracksChanged;
        }

        private void Album_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                Tracks.Add(new ObservableTrack(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Tracks.Remove(new ObservableTrack(item));
            }
        }

        private void Album_ImagesChanged(object sender, CollectionChangedEventArgs<IImage> e)
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

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _album.PlaybackStateChanged += value;
            }

            remove
            {
                _album.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.NameChanged"/>
        public event EventHandler<string>? NameChanged
        {
            add
            {
                _album.NameChanged += value;
            }

            remove
            {
                _album.NameChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.DescriptionChanged"/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                _album.DescriptionChanged += value;
            }

            remove
            {
                _album.DescriptionChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.UrlChanged"/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                _album.UrlChanged += value;
            }

            remove
            {
                _album.UrlChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged
        {
            add
            {
                _album.ImagesChanged += value;
            }

            remove
            {
                _album.ImagesChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrackCollection.TracksChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add
            {
                _album.TracksChanged += value;
            }

            remove
            {
                _album.TracksChanged -= value;
            }
        }

        /// <summary>
        /// Attempts to pause the album, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        public Task PauseAsync() => _album.PauseAsync();

        /// <summary>
        /// Attempts to play the album.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTracksCount => _album.TotalTracksCount;

        /// <inheritdoc cref="IRelatedCollectionGroups.RelatedItems"/>
        public ObservableCollectionGroup RelatedItems { get; }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync() => _album.PlayAsync();

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync(int, int)"/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0) => _album.PopulateTracksAsync(limit, offset);
    }
}
