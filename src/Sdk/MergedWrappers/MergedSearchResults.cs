using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
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
        private readonly List<IPlayableCollectionGroup> _children = new List<IPlayableCollectionGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchResults"/> class.
        /// </summary>
        /// <param name="searchResults">The search results to merge.</param>
        public MergedSearchResults(ISearchResults[] searchResults)
        {
            Sources = searchResults;

            // TODO: Use top Preferred core.
            _preferredSource = Sources[0];

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
                _tracks = _tracks.Union(item.Tracks).ToList();
                _albums.AddRange(item.Albums);
                _artists.AddRange(item.Artists);
                _images.AddRange(item.Images);

                AttachCollectionChangedEvents(item);
            }
        }

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add
            {
                _preferredSource.DurationChanged += value;
            }

            remove
            {
                _preferredSource.DurationChanged -= value;
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

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> Children => _children;

        /// <inheritdoc/>
        public int TotalChildrenCount { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IPlaylist> Playlists => _playlists;

        /// <inheritdoc/>
        public int TotalPlaylistCount { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _tracks;

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _artists;

        /// <inheritdoc/>
        public int TotalArtistsCount { get; }

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
        public bool IsPlayAsyncSupported => _preferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => _preferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => _preferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => _preferredSource.IsChangeImagesAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => _preferredSource.IsChangeDurationAsyncSupported;

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
        public Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            // The items in this Merged source are its own thing once we merge it, so any offset / limit passed here are completely disregarding the original source

            // Create a new collection that contains all data from the merged sources, even for data we don't have. Store the original offset of each and get it as needed.

            // Two ways of sorting the data:
            // Alternating until all sources run out
            // In order by rank
            var limitRemainder = limit % Sources.Count;
            var limitPerSource = (limit - limitRemainder) / Sources.Count;

            Parallel.ForEach(Sources, async source =>
            {
                var remainingItems = source.TotalAlbumsCount - source.Albums.Count;

                if (remainingItems > 0)
                    await source.PopulateAlbumsAsync(limitPerSource);
            });

            return Task.FromResult(Albums);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return _preferredSource.ChangeNameAsync(name);
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            return _preferredSource.ChangeImagesAsync(images);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return _preferredSource.ChangeDescriptionAsync(description);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return _preferredSource.ChangeDurationAsync(duration);
        }
    }
}
