using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <inheritdoc/>
    public class BindableLibrary : ObservableObject, ILibrary
    {
        private readonly ILibrary _library;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableLibrary"/> class.
        /// </summary>
        /// <param name="library"></param>
        public BindableLibrary(ILibrary library)
        {
            _library = library;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _library.Artists;

        /// <inheritdoc/>
        public int TotalArtistsCount => _library.TotalArtistsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _library.Albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _library.TotalAlbumsCount;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _library.Tracks;

        /// <inheritdoc/>
        public int TotalTracksCount => _library.TotalTracksCount;

        /// <inheritdoc/>
        public IReadOnlyList<IPlaylist> Playlists => _library.Playlists;

        /// <inheritdoc/>
        public string Id => _library.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _library.SourceCore;

        /// <inheritdoc/>
        public string Name => _library.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _library.Images;

        /// <inheritdoc/>
        public Uri? Url => _library.Url;

        /// <inheritdoc/>
        public string? Description => _library.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _library.Owner;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _library.PlaybackState;

        /// <inheritdoc/>
        public int TotalPlaylistCount => _library.TotalPlaylistCount;

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> Children => _library.Children;

        /// <inheritdoc/>
        public int TotalChildrenCount => _library.TotalChildrenCount;

        /// <inheritdoc/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0) => _library.PopulateAlbumsAsync(limit, offset);

        /// <inheritdoc/>
        public Task PopulateArtistsAsync(int limit, int offset = 0) => _library.PopulateArtistsAsync(limit, offset);

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0) => _library.PopulateTracksAsync(limit, offset);

        /// <inheritdoc/>
        public Task PopulatePlaylistsAsync(int limit, int offset) => _library.PopulatePlaylistsAsync(limit, offset);

        /// <inheritdoc/>
        public Task PopulateChildrenAsync(int limit, int offset = 0)
        {
            return _library.PopulateChildrenAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _library.PlayAsync();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _library.PauseAsync();
        }
    }
}
