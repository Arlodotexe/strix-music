using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Mergers
{
    /// <summary>
    /// A concrete class that merged multiple <see cref="ISearchResults"/>
    /// </summary>
    public class MergedSearchResults : ISearchResults
    {
        private ISearchResults[] _searchResults;

        private List<IPlaylist> _playlists = new List<IPlaylist>();
        private List<ITrack> _tracks = new List<ITrack>();
        private List<IAlbum> _albums = new List<IAlbum>();
        private List<IArtist> _artists = new List<IArtist>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchResults"/> class.
        /// </summary>
        /// <param name="searchResults">The search results to merge.</param>
        public MergedSearchResults(ISearchResults[] searchResults)
        {
            _searchResults = searchResults;

            foreach (var item in searchResults)
            {
                TotalChildrenCount += item.TotalChildrenCount;
                TotalPlaylistCount += item.TotalPlaylistCount;
                TotalTracksCount += item.TotalTracksCount;
                TotalAlbumsCount += item.TotalAlbumsCount;
                TotalArtistsCount += item.TotalArtistsCount;

                _playlists.AddRange(item.Playlists);
                _tracks.AddRange(item.Tracks);
                _albums.AddRange(item.Albums);
                _artists.AddRange(item.Artists);
            }
        }

        private List<IPlayableCollectionGroup> _children = new List<IPlayableCollectionGroup>();

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> Children => _children;

        /// <inheritdoc/>
        public int TotalChildrenCount { get; } = 0;

        /// <inheritdoc/>
        public IReadOnlyList<IPlaylist> Playlists => _playlists;

        /// <inheritdoc/>
        public int TotalPlaylistCount { get; } = 0;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _tracks;

        /// <inheritdoc/>
        public int TotalTracksCount { get; } = 0;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount { get; } = 0;

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _artists;

        /// <inheritdoc/>
        public int TotalArtistsCount { get; } = 0;

        /// <inheritdoc/>
        public ICore SourceCore => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Id => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri? Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string? Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> ChildrenChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string> NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?> UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateArtistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateChildrenAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
