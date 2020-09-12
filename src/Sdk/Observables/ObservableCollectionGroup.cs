using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class ObservableCollectionGroup : ObservableObject
    {
        private readonly IPlayableCollectionGroup _collectionGroupBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public ObservableCollectionGroup(IPlayableCollectionGroup collectionGroup)
        {
            _collectionGroupBase = collectionGroup;

            SourceCore = new ObservableCore(_collectionGroupBase.SourceCore);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeImagesAsyncCommand = new AsyncRelayCommand<IReadOnlyList<IImage>>(ChangeImagesAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            Tracks = new ObservableCollection<ObservableTrack>(_collectionGroupBase.Tracks.Select(x => new ObservableTrack(x)));
            Playlists = new ObservableCollection<ObservablePlaylist>(_collectionGroupBase.Playlists.Select(x => new ObservablePlaylist(x)));
            Albums = new ObservableCollection<ObservableAlbum>(_collectionGroupBase.Albums.Select(x => new ObservableAlbum(x)));
            Artists = new ObservableCollection<ObservableArtist>(_collectionGroupBase.Artists.Select(x => new ObservableArtist(x)));
            Children = new ObservableCollection<ObservableCollectionGroup>(_collectionGroupBase.Children.Select(x => new ObservableCollectionGroup(x)));

            AttachEvents();
        }

        private void AttachEvents()
        {
            _collectionGroupBase.AlbumsChanged += CollectionGroupBase_AlbumsChanged;
            _collectionGroupBase.ArtistsChanged += CollectionGroupBase_ArtistsChanged;
            _collectionGroupBase.ChildrenChanged += CollectionGroupBase_ChildrenChanged;
            _collectionGroupBase.PlaybackStateChanged += CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.PlaylistsChanged += CollectionGroupBase_PlaylistsChanged;
            _collectionGroupBase.TracksChanged += CollectionGroupBase_TracksChanged;
            _collectionGroupBase.DescriptionChanged += CollectionGroupBase_DescriptionChanged;
            _collectionGroupBase.NameChanged += CollectionGroupBase_NameChanged;
            _collectionGroupBase.UrlChanged += CollectionGroupBase_UrlChanged;
        }

        private void DetachEvents()
        {
            _collectionGroupBase.AlbumsChanged -= CollectionGroupBase_AlbumsChanged;
            _collectionGroupBase.ArtistsChanged -= CollectionGroupBase_ArtistsChanged;
            _collectionGroupBase.ChildrenChanged -= CollectionGroupBase_ChildrenChanged;
            _collectionGroupBase.PlaybackStateChanged -= CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.PlaylistsChanged -= CollectionGroupBase_PlaylistsChanged;
            _collectionGroupBase.TracksChanged -= CollectionGroupBase_TracksChanged;
            _collectionGroupBase.DescriptionChanged -= CollectionGroupBase_DescriptionChanged;
            _collectionGroupBase.NameChanged -= CollectionGroupBase_NameChanged;
            _collectionGroupBase.UrlChanged -= CollectionGroupBase_UrlChanged;
        }

        private void CollectionGroupBase_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void CollectionGroupBase_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void CollectionGroupBase_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void CollectionGroupBase_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
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

        private void CollectionGroupBase_PlaylistsChanged(object sender, CollectionChangedEventArgs<IPlaylist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Playlists.Add(new ObservablePlaylist(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Playlists.Remove(new ObservablePlaylist(item));
            }
        }

        private void CollectionGroupBase_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void CollectionGroupBase_ChildrenChanged(object sender, CollectionChangedEventArgs<IPlayableCollectionGroup> e)
        {
            foreach (var item in e.AddedItems)
            {
                Children.Add(new ObservableCollectionGroup(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Children.Remove(new ObservableCollectionGroup(item));
            }
        }

        private void CollectionGroupBase_ArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Artists.Add(new ObservableArtist(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Artists.Remove(new ObservableArtist(item));
            }
        }

        private void CollectionGroupBase_AlbumsChanged(object sender, CollectionChangedEventArgs<IAlbum> e)
        {
            foreach (var item in e.AddedItems)
            {
                Albums.Add(new ObservableAlbum(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Albums.Remove(new ObservableAlbum(item));
            }
        }

        /// <inheritdoc cref="IPlayableCollectionGroup.ChildrenChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> ChildrenChanged
        {
            add
            {
                _collectionGroupBase.ChildrenChanged += value;
            }

            remove
            {
                _collectionGroupBase.ChildrenChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlaylistCollection.PlaylistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged
        {
            add
            {
                _collectionGroupBase.PlaylistsChanged += value;
            }

            remove
            {
                _collectionGroupBase.PlaylistsChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrackCollection.TracksChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add
            {
                _collectionGroupBase.TracksChanged += value;
            }

            remove
            {
                _collectionGroupBase.TracksChanged -= value;
            }
        }

        /// <inheritdoc cref="IAlbumCollection.AlbumsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged
        {
            add
            {
                _collectionGroupBase.AlbumsChanged += value;
            }

            remove
            {
                _collectionGroupBase.AlbumsChanged -= value;
            }
        }

        /// <inheritdoc cref="IArtistCollection.ArtistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged
        {
            add
            {
                _collectionGroupBase.ArtistsChanged += value;
            }

            remove
            {
                _collectionGroupBase.ArtistsChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _collectionGroupBase.PlaybackStateChanged += value;
            }

            remove
            {
                _collectionGroupBase.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.DurationChanged"/>
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add
            {
                _collectionGroupBase.DurationChanged += value;
            }

            remove
            {
                _collectionGroupBase.DurationChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayableCollectionGroup.Children"/>
        public ObservableCollection<ObservableCollectionGroup> Children { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _collectionGroupBase.Id;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public ObservableCore SourceCore { get; }

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name
        {
            get => _collectionGroupBase.Name;
            private set => SetProperty(() => _collectionGroupBase.Name, value);
        }

        /// <inheritdoc cref="IPlayable.Images"/>
        public IReadOnlyList<IImage> Images => _collectionGroupBase.Images;

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url
        {
            get => _collectionGroupBase.Url;
            private set => SetProperty(() => _collectionGroupBase.Url, value);
        }

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description
        {
            get => _collectionGroupBase.Description;
            private set => SetProperty(() => _collectionGroupBase.Description, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _collectionGroupBase.PlaybackState;
            private set => SetProperty(() => _collectionGroupBase.PlaybackState, value);
        }

        /// <inheritdoc cref="IPlayableCollectionGroup.TotalChildrenCount"/>
        public int TotalChildrenCount => _collectionGroupBase.TotalChildrenCount;

        /// <inheritdoc cref="IPlaylistCollection.Playlists"/>
        public ObservableCollection<ObservablePlaylist> Playlists { get; }

        /// <inheritdoc cref="IPlaylistCollection.TotalPlaylistCount"/>
        public int TotalPlaylistCount => _collectionGroupBase.TotalPlaylistCount;

        /// <inheritdoc cref="ITrackCollection.Tracks"/>
        public ObservableCollection<ObservableTrack> Tracks { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTracksCount => _collectionGroupBase.TotalTracksCount;

        /// <inheritdoc cref="IAlbumCollection.Albums"/>
        public ObservableCollection<ObservableAlbum> Albums { get; }

        /// <inheritdoc cref="IAlbumCollection.TotalAlbumsCount"/>
        public int TotalAlbumsCount => _collectionGroupBase.TotalAlbumsCount;

        /// <inheritdoc cref="IArtistCollection.Artists"/>
        public ObservableCollection<ObservableArtist> Artists { get; }

        /// <inheritdoc cref="IArtistCollection.TotalArtistsCount"/>
        public int TotalArtistsCount => _collectionGroupBase.TotalArtistsCount;

        /// <inheritdoc/>
        public Task PauseAsync() => _collectionGroupBase.PauseAsync();

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync() => _collectionGroupBase.PlayAsync();

        /// <inheritdoc cref="IPlayable.ChangeNameAsync"/>
        public Task ChangeNameAsync(string name) => _collectionGroupBase.ChangeNameAsync(name);

        /// <inheritdoc cref="IPlayable.ChangeImagesAsync"/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images) => _collectionGroupBase.ChangeImagesAsync(images);

        /// <inheritdoc cref="IPlayable.ChangeDescriptionAsync"/>
        public Task ChangeDescriptionAsync(string? description) => _collectionGroupBase.ChangeDescriptionAsync(description);

        /// <inheritdoc cref="IPlayable.ChangeDurationAsync"/>
        public Task ChangeDurationAsync(TimeSpan duration) => _collectionGroupBase.ChangeDurationAsync(duration);

        /// <inheritdoc cref="IPlayableCollectionGroup.PopulateChildrenAsync(int, int)"/>
        public Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset) => _collectionGroupBase.PopulateChildrenAsync(limit, offset);

        /// <inheritdoc cref="IPlaylistCollection.PopulatePlaylistsAsync(int, int)"/>
        public Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0) => _collectionGroupBase.PopulatePlaylistsAsync(limit, offset);

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync(int, int)"/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0) => _collectionGroupBase.PopulateTracksAsync(limit, offset);

        /// <inheritdoc cref="IAlbumCollection.PopulateAlbumsAsync(int, int)"/>
        public Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0) => _collectionGroupBase.PopulateAlbumsAsync(limit, offset);

        /// <inheritdoc cref="IArtistCollection.PopulateArtistsAsync(int, int)"/>
        public Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0) => _collectionGroupBase.PopulateArtistsAsync(limit, offset);

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
