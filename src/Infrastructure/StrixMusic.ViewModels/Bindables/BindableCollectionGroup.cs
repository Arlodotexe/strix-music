using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class BindableCollectionGroup : ObservableObject, IPlayableCollectionGroup
    {
        /// <summary>
        /// Backing field for the items. Is modified with merged items from all inputted collections.
        /// </summary>
        private readonly IList<IPlayableCollectionBase> _playableCollections = new List<IPlayableCollectionBase>();

        private readonly IPlayableCollectionGroup _collectionBase;
        private int _totalItemsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        /// <param name="decideItemCombine">When called, return an item from <see cref="IPlayableCollectionBase"/> to use as a key when grouping items.</param>
        public BindableCollectionGroup(IPlayableCollectionGroup collectionGroup)
        {
            _collectionBase = collectionGroup;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionBase">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public BindableCollectionGroup(IPlayableCollectionBase collectionBase)
        {
            _collectionBase = (IPlayableCollectionGroup)collectionBase;
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> ChildrenChanged
        {
            add
            {
                _collectionBase.ChildrenChanged += value;
            }

            remove
            {
                _collectionBase.ChildrenChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged
        {
            add
            {
                _collectionBase.PlaylistsChanged += value;
            }

            remove
            {
                _collectionBase.PlaylistsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add
            {
                _collectionBase.TracksChanged += value;
            }

            remove
            {
                _collectionBase.TracksChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged
        {
            add
            {
                _collectionBase.AlbumsChanged += value;
            }

            remove
            {
                _collectionBase.AlbumsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged
        {
            add
            {
                _collectionBase.ArtistsChanged += value;
            }

            remove
            {
                _collectionBase.ArtistsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _collectionBase.PlaybackStateChanged += value;
            }

            remove
            {
                _collectionBase.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionBase> Items => _collectionBase.Children;

        /// <inheritdoc/>
        public string Id => _collectionBase.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _collectionBase.SourceCore;

        /// <inheritdoc/>
        public string Name => _collectionBase.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _collectionBase.Images;

        /// <inheritdoc/>
        public Uri? Url => _collectionBase.Url;

        /// <inheritdoc/>
        public string? Description => _collectionBase.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _collectionBase.Owner;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _collectionBase.PlaybackState;

        /// <inheritdoc/>
        public int TotalChildrenCount => _totalItemsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> Children => _collectionBase.Children;

        /// <inheritdoc/>
        public IReadOnlyList<IPlaylist> Playlists => _collectionBase.Playlists;

        /// <inheritdoc/>
        public int TotalPlaylistCount => _collectionBase.TotalPlaylistCount;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _collectionBase.Tracks;

        /// <inheritdoc/>
        public int TotalTracksCount => _collectionBase.TotalTracksCount;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _collectionBase.Albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _collectionBase.TotalAlbumsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _collectionBase.Artists;

        /// <inheritdoc/>
        public int TotalArtistsCount => _collectionBase.TotalArtistsCount;

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _collectionBase.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _collectionBase.PlayAsync();
        }

        /// <inheritdoc/>
        public Task PopulateChildrenAsync(int limit, int offset)
        {
            for (int i = offset; i < limit; i++)
            {
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return _collectionBase.PopulatePlaylistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            return _collectionBase.PopulateTracksAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            return _collectionBase.PopulateAlbumsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateArtistsAsync(int limit, int offset = 0)
        {
            return _collectionBase.PopulateArtistsAsync(limit, offset);
        }
    }
}
