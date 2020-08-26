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
    /// <inheritdoc/>
    public class BindableLibrary : ObservableObject
    {
        private readonly ILibrary _library;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableLibrary"/> class.
        /// </summary>
        /// <param name="library"></param>
        public BindableLibrary(ILibrary library)
        {
            _library = library;

            Albums = new ObservableCollection<IAlbum>(_library.Albums);
            Artists = new ObservableCollection<IArtist>(_library.Artists);
            Playlists = new ObservableCollection<IPlaylist>(_library.Playlists);
            Tracks = new ObservableCollection<ITrack>(_library.Tracks);
            Children = new ObservableCollection<IPlayableCollectionGroup>(_library.Children);

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _library.AlbumsChanged += Library_AlbumsChanged;
            _library.ArtistsChanged += Library_ArtistsChanged;
            _library.ChildrenChanged += Library_ChildrenChanged;
            _library.PlaybackStateChanged += Library_PlaybackStateChanged;
            _library.PlaylistsChanged += Library_PlaylistsChanged;
            _library.TracksChanged += Library_TracksChanged;
        }

        private void DetachEvents()
        {
            _library.AlbumsChanged -= Library_AlbumsChanged;
            _library.ArtistsChanged -= Library_ArtistsChanged;
            _library.ChildrenChanged -= Library_ChildrenChanged;
            _library.PlaybackStateChanged -= Library_PlaybackStateChanged;
            _library.PlaylistsChanged -= Library_PlaylistsChanged;
            _library.TracksChanged -= Library_TracksChanged;
        }

        private void Library_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
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

        private void Library_PlaylistsChanged(object sender, CollectionChangedEventArgs<IPlaylist> e)
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

        private void Library_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Library_ChildrenChanged(object sender, CollectionChangedEventArgs<IPlayableCollectionGroup> e)
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

        private void Library_ArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
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

        private void Library_AlbumsChanged(object sender, CollectionChangedEventArgs<IAlbum> e)
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
                _library.ChildrenChanged += value;
            }

            remove
            {
                _library.ChildrenChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlaylistCollection.PlaylistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged
        {
            add
            {
                _library.PlaylistsChanged += value;
            }

            remove
            {
                _library.PlaylistsChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrackCollection.TracksChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add
            {
                _library.TracksChanged += value;
            }

            remove
            {
                _library.TracksChanged -= value;
            }
        }

        /// <inheritdoc cref="IAlbumCollection.AlbumsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged
        {
            add
            {
                _library.AlbumsChanged += value;
            }

            remove
            {
                _library.AlbumsChanged -= value;
            }
        }

        /// <inheritdoc cref="IArtistCollection.ArtistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged
        {
            add
            {
                _library.ArtistsChanged += value;
            }

            remove
            {
                _library.ArtistsChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _library.PlaybackStateChanged += value;
            }

            remove
            {
                _library.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc cref="IArtistCollection.Artists"/>
        public ObservableCollection<IArtist> Artists { get; }

        /// <inheritdoc cref="IArtist.TotalRelatedArtistsCount"/>
        public int TotalArtistsCount => _library.TotalArtistsCount;

        /// <inheritdoc cref="IAlbumCollection.Albums"/>
        public ObservableCollection<IAlbum> Albums { get; }

        /// <inheritdoc cref="IAlbumCollection.TotalAlbumsCount"/>
        public int TotalAlbumsCount => _library.TotalAlbumsCount;

        /// <inheritdoc cref="ITrackCollection.Tracks"/>
        public ObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTracksCount => _library.TotalTracksCount;

        /// <inheritdoc cref="IPlaylistCollection.Playlists"/>
        public ObservableCollection<IPlaylist> Playlists { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _library.Id;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public ICore SourceCore => _library.SourceCore;

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name => _library.Name;

        /// <inheritdoc cref="IPlayable.Images"/>
        public IReadOnlyList<IImage> Images => _library.Images;

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url => _library.Url;

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description => _library.Description;

        /// <inheritdoc cref="IPlayableCollectionBase.Owner"/>
        public IUserProfile? Owner => _library.Owner;

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _library.PlaybackState;
            set => SetProperty(() => _library.PlaybackState, value);
        }

        /// <inheritdoc cref="IPlaylistCollection.TotalPlaylistCount"/>
        public int TotalPlaylistCount => _library.TotalPlaylistCount;

        /// <inheritdoc cref="IPlayableCollectionGroup.Children"/>
        public ObservableCollection<IPlayableCollectionGroup> Children { get; }

        /// <inheritdoc cref="IPlayableCollectionGroup.TotalChildrenCount"/>
        public int TotalChildrenCount => _library.TotalChildrenCount;

        /// <inheritdoc cref="IAlbumCollection.PopulateAlbumsAsync(int, int)"/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0) => _library.PopulateAlbumsAsync(limit, offset);

        /// <inheritdoc cref="IArtistCollection.PopulateArtistsAsync(int, int)"/>
        public Task PopulateArtistsAsync(int limit, int offset = 0) => _library.PopulateArtistsAsync(limit, offset);

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync(int, int)"/>
        public Task PopulateTracksAsync(int limit, int offset = 0) => _library.PopulateTracksAsync(limit, offset);

        /// <inheritdoc cref="IPlaylistCollection.PopulatePlaylistsAsync(int, int)"/>
        public Task PopulatePlaylistsAsync(int limit, int offset) => _library.PopulatePlaylistsAsync(limit, offset);

        /// <inheritdoc cref="IPlayableCollectionGroup.PopulateChildrenAsync(int, int)"/>
        public Task PopulateChildrenAsync(int limit, int offset = 0)
        {
            return _library.PopulateChildrenAsync(limit, offset);
        }

        /// <summary>
        /// Attempts to pause the item.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync()
        {
            return _library.PlayAsync();
        }

        /// <summary>
        /// Attempts to pause the item.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        public Task PauseAsync()
        {
            return _library.PauseAsync();
        }
    }
}
