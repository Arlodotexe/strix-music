using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.MergedWrappers
{
    /// <summary>
    /// A concrete class that merged multiple <see cref="ISearchResults"/>
    /// </summary>
    public class MergedSearchResults : ISearchResults
    {
        private readonly ISearchResults _preferredSource;

        private readonly List<IPlaylist> _playlists = new List<IPlaylist>();
        private readonly List<ITrack> _tracks = new List<ITrack>();
        private readonly List<IAlbum> _albums = new List<IAlbum>();
        private readonly List<IArtist> _artists = new List<IArtist>();
        private readonly List<IImage> _images = new List<IImage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchResults"/> class.
        /// </summary>
        /// <param name="searchResults">The search results to merge.</param>
        public MergedSearchResults(ISearchResults[] searchResults)
        {
            Sources = searchResults;

            // TODO: Use top preffered core.
            _preferredSource = Sources.First();

            AttachPropertyChangedEvents(_preferredSource);

            foreach (var item in Sources)
            {
                TotalChildrenCount += item.TotalChildrenCount;
                TotalPlaylistCount += item.TotalPlaylistCount;
                TotalTracksCount += item.TotalTracksCount;
                TotalAlbumsCount += item.TotalAlbumsCount;
                TotalArtistsCount += item.TotalArtistsCount;
                Duration += item.Duration;

                // todo: merge data as needed
                // todo 2: Don't do this. Cores shouldn't supply data unless it's requested, otherwise we'd have data scattered around.
                _playlists.AddRange(item.Playlists);
                _tracks.Union(item.Tracks);
                _albums.AddRange(item.Albums);
                _artists.AddRange(item.Artists);
                _images.AddRange(item.Images);

                AttachCollectionChangedEvents(item);
            }
        }

        private void AttachCollectionChangedEvents(ISearchResults source)
        {
            source.ChildrenChanged += ChildrenChanged;
            source.PlaylistsChanged += PlaylistsChanged;
            source.TracksChanged += TracksChanged;
            source.AlbumsChanged += AlbumsChanged;
            source.ArtistsChanged += ArtistsChanged;
        }

        private void DetachCollectionChangedEvents(ISearchResults source)
        {
            source.ChildrenChanged -= ChildrenChanged;
            source.PlaylistsChanged -= PlaylistsChanged;
            source.TracksChanged -= TracksChanged;
            source.AlbumsChanged -= AlbumsChanged;
            source.ArtistsChanged -= ArtistsChanged;
        }

        private void AttachPropertyChangedEvents(ISearchResults source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.ImagesChanged += ImagesChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
        }

        private void DetachPropertyChangedEvents(ISearchResults source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.ImagesChanged -= ImagesChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
        }

        /// <summary>
        /// The source data that went into this merged instance.
        /// </summary>
        public IReadOnlyList<ISearchResults> Sources { get; }

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
        public ICore SourceCore => _preferredSource.SourceCore;

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url => _preferredSource.Url;

        /// <inheritdoc/>
        public string Name => _preferredSource.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _images;

        /// <inheritdoc/>
        public string? Description => _preferredSource.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _preferredSource.PlaybackState;

        /// <inheritdoc/>
        public TimeSpan Duration { get; } = new TimeSpan(0);

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _preferredSource.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _preferredSource.PlayAsync();
        }

        /// <inheritdoc/>
        public async Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            // The items in this Merged source are its own thing once we merge it, so any offset / limit passed here are completely disregarding the original source

            // For offset
            // Create a new collection that contains all data from the merged sources, even for data we don't have. Store the original offset of each and get it as needed.

            // For limit:
            // Check how many items are left in each core.
            // For the limit that was requested, get the number of items we can get per core (limitPerSource).
            // The remainder gets pulled from the highest ranking preferred core, moving to the next highest ranking core if there are no remaining items.
            var limitRemainder = limit % Sources.Count;
            var limitPerSource = (limit - limitRemainder) / Sources.Count;

            Parallel.ForEach(Sources, async source =>
            {
                var remainingItems = source.TotalAlbumsCount - source.Albums.Count();

                if (remainingItems > 0)
                    await source.PopulateAlbumsAsync(limitPerSource);
            });
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
