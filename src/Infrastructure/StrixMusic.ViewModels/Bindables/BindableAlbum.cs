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
    /// <inheritdoc/>
    public class BindableAlbum : ObservableObject
    {
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableAlbum"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public BindableAlbum(IAlbum album)
        {
            _album = album;

            SourceCore = new BindableCoreData(_album.SourceCore);

            Images = new ObservableCollection<IImage>(_album.Images);
            Tracks = new ObservableCollection<BindableTrack>(_album.Tracks.Select(x => new BindableTrack(x)));
            RelatedItems = new ObservableCollection<BindableCollectionGroup>(_album.RelatedItems.Select(x => new BindableCollectionGroup(x)));
            Artist = new BindableArtist(_album.Artist);

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
            _album.RelatedItemsChanged += Album_RelatedItemsChanged;
            _album.TracksChanged += Album_TracksChanged;
        }

        private void DetachEvents()
        {
            _album.PlaybackStateChanged -= Album_PlaybackStateChanged;
            _album.DescriptionChanged -= Album_DescriptionChanged;
            _album.NameChanged -= Album_NameChanged;
            _album.UrlChanged -= Album_UrlChanged;
            _album.ImagesChanged += Album_ImagesChanged;
            _album.RelatedItemsChanged -= Album_RelatedItemsChanged;
            _album.TracksChanged -= Album_TracksChanged;
        }

        private void Album_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                Tracks.Add(new BindableTrack(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Tracks.Remove(new BindableTrack(item));
            }
        }

        private void Album_RelatedItemsChanged(object sender, CollectionChangedEventArgs<IPlayableCollectionGroup> e)
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
        public ObservableCollection<BindableTrack> Tracks { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTrackCounts => _album.TotalTracksCount;

        /// <inheritdoc cref="IAlbum.Artist"/>
        public BindableArtist Artist { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _album.Id;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public BindableCoreData SourceCore { get; }

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

        /// <inheritdoc cref="ITrackCollection"/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged
        {
            add
            {
                _album.RelatedItemsChanged += value;
            }

            remove
            {
                _album.RelatedItemsChanged -= value;
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
        public ObservableCollection<BindableCollectionGroup> RelatedItems { get; }

        /// <inheritdoc cref="IRelatedCollectionGroups.TotalRelatedItemsCount"/>
        public int TotalRelatedItemsCount => _album.TotalRelatedItemsCount;

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync() => _album.PlayAsync();

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync(int, int)"/>
        public Task PopulateTracksAsync(int limit, int offset = 0) => _album.PopulateTracksAsync(limit, offset);

        /// <inheritdoc cref="IRelatedCollectionGroups.PopulateRelatedItemsAsync(int, int)"/>
        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            return _album.PopulateRelatedItemsAsync(limit, offset);
        }
    }
}
