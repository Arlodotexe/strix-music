using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class BindableCollectionGroup : ObservableObject
    {
        private readonly IPlayableCollectionGroup _collectionGroupBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public BindableCollectionGroup(IPlayableCollectionGroup collectionGroup)
        {
            _collectionGroupBase = collectionGroup;

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            Tracks = new ObservableCollection<ITrack>(_collectionGroupBase.Tracks);
            Playlists = new ObservableCollection<IPlaylist>(_collectionGroupBase.Playlists);
            Albums = new ObservableCollection<IAlbum>(_collectionGroupBase.Albums);
            Artists = new ObservableCollection<IArtist>(_collectionGroupBase.Artists);
            Children = new ObservableCollection<IPlayableCollectionGroup>(_collectionGroupBase.Children);

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
        }

        private void DetachEvents()
        {
            _collectionGroupBase.AlbumsChanged -= CollectionGroupBase_AlbumsChanged;
            _collectionGroupBase.ArtistsChanged -= CollectionGroupBase_ArtistsChanged;
            _collectionGroupBase.ChildrenChanged -= CollectionGroupBase_ChildrenChanged;
            _collectionGroupBase.PlaybackStateChanged -= CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.PlaylistsChanged -= CollectionGroupBase_PlaylistsChanged;
            _collectionGroupBase.TracksChanged -= CollectionGroupBase_TracksChanged;
        }

        private void CollectionGroupBase_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                Tracks.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Tracks.Remove(item);
            }
        }

        private void CollectionGroupBase_PlaylistsChanged(object sender, CollectionChangedEventArgs<IPlaylist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Playlists.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Playlists.Remove(item);
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
                Children.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Children.Remove(item);
            }
        }

        private void CollectionGroupBase_ArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Artists.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Artists.Remove(item);
            }
        }

        private void CollectionGroupBase_AlbumsChanged(object sender, CollectionChangedEventArgs<IAlbum> e)
        {
            foreach (var item in e.AddedItems)
            {
                Albums.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Albums.Remove(item);
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

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
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

        /// <inheritdoc cref="IPlayableCollectionGroup.Children"/>
        public ObservableCollection<IPlayableCollectionGroup> Children { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _collectionGroupBase.Id;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public ICore SourceCore => _collectionGroupBase.SourceCore;

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name => _collectionGroupBase.Name;

        /// <inheritdoc cref="IPlayable.Images"/>
        public IReadOnlyList<IImage> Images => _collectionGroupBase.Images;

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url => _collectionGroupBase.Url;

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description => _collectionGroupBase.Description;

        /// <inheritdoc cref="IPlayableCollectionBase.Owner"/>
        public IUserProfile? Owner => _collectionGroupBase.Owner;

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _collectionGroupBase.PlaybackState;
            set => SetProperty(() => _collectionGroupBase.PlaybackState, value);
        }

        /// <inheritdoc cref="IPlayableCollectionGroup.TotalChildrenCount"/>
        public int TotalChildrenCount => _collectionGroupBase.TotalChildrenCount;

        /// <inheritdoc cref="IPlaylistCollection.Playlists"/>
        public ObservableCollection<IPlaylist> Playlists { get; }

        /// <inheritdoc cref="IPlaylistCollection.cou"/>
        public int TotalPlaylistCount => _collectionGroupBase.TotalPlaylistCount;

        /// <inheritdoc cref="ITrackCollection.Tracks"/>
        public ObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTracksCount => _collectionGroupBase.TotalTracksCount;

        /// <inheritdoc cref="IAlbumCollection.Albums"/>
        public ObservableCollection<IAlbum> Albums { get; }

        /// <inheritdoc cref="IAlbumCollection.TotalAlbumsCount"/>
        public int TotalAlbumsCount => _collectionGroupBase.TotalAlbumsCount;

        /// <inheritdoc cref="IArtistCollection.Artists"/>
        public ObservableCollection<IArtist> Artists { get; }

        /// <inheritdoc cref="IArtistCollection.TotalArtistsCount"/>
        public int TotalArtistsCount => _collectionGroupBase.TotalArtistsCount;

        /// <summary>
        /// Attempts to a pause the collection (if playing).
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _collectionGroupBase.PauseAsync();
        }

        /// <summary>
        /// Attempts to play the collection. Resumes if paused.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync()
        {
            return _collectionGroupBase.PlayAsync();
        }

        /// <inheritdoc cref="IPlayableCollectionGroup.PopulateChildrenAsync(int, int)"/>
        public Task PopulateChildrenAsync(int limit, int offset)
        {
            return _collectionGroupBase.PopulateChildrenAsync(limit, offset);
        }

        /// <inheritdoc cref="IPlaylistCollection.PopulatePlaylistsAsync(int, int)"/>
        public Task PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return _collectionGroupBase.PopulatePlaylistsAsync(limit, offset);
        }

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync(int, int)"/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            return _collectionGroupBase.PopulateTracksAsync(limit, offset);
        }

        /// <inheritdoc cref="IAlbumCollection.PopulateAlbumsAsync(int, int)"/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            return _collectionGroupBase.PopulateAlbumsAsync(limit, offset);
        }

        /// <inheritdoc cref="IArtistCollection.PopulateArtistsAsync(int, int)"/>
        public Task PopulateArtistsAsync(int limit, int offset = 0)
        {
            return _collectionGroupBase.PopulateArtistsAsync(limit, offset);
        }
    }
}
