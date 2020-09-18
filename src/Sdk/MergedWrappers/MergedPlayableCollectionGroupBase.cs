using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Extensions.AsyncExtensions;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="IPlayableCollectionGroup"/>s.
    /// </summary>
    public class MergedPlayableCollectionGroup : MergedPlayableCollectionGroupBase, IEquatable<IPlayableCollectionGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroup"/> class.
        /// </summary>
        /// <param name="source"></param>
        public MergedPlayableCollectionGroup(IReadOnlyList<IPlayableCollectionGroup> source)
            : base(source)
        {
        }

        /// <inheritdoc/>
        public bool Equals(IPlayableCollectionGroup? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is IPlayableCollectionGroup other && Equals(other));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }


    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroup"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase : IPlayableCollectionGroup
    {
        private readonly List<IPlaylist> _playlists = new List<IPlaylist>();
        private readonly List<ITrack> _tracks = new List<ITrack>();
        private readonly List<IAlbum> _albums = new List<IAlbum>();
        private readonly List<IArtist> _artists = new List<IArtist>();
        private readonly List<IImage> _images = new List<IImage>();
        private readonly List<IPlayableCollectionGroup> _children = new List<IPlayableCollectionGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="source">The search results to merge.</param>
        protected MergedPlayableCollectionGroupBase(IReadOnlyList<IPlayableCollectionGroup> source)
        {
            Sources = source;

            // TODO: Use top Preferred core.
            PreferredSource = Sources[0];

            AttachPropertyChangedEvents(PreferredSource);

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

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected IPlayableCollectionGroup PreferredSource { get; }

        private void AttachCollectionChangedEvents(IPlayableCollectionGroup source)
        {
            source.ChildrenChanged += ChildrenChanged;
            source.PlaylistsChanged += PlaylistsChanged;
            source.TracksChanged += TracksChanged;
            source.AlbumsChanged += AlbumsChanged;
            source.ArtistsChanged += ArtistsChanged;
        }

        private void DetachCollectionChangedEvents(IPlayableCollectionGroup source)
        {
            source.ChildrenChanged -= ChildrenChanged;
            source.PlaylistsChanged -= PlaylistsChanged;
            source.TracksChanged -= TracksChanged;
            source.AlbumsChanged -= AlbumsChanged;
            source.ArtistsChanged -= ArtistsChanged;
        }

        private void AttachPropertyChangedEvents(IPlayable source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.ImagesChanged += ImagesChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
        }

        private void DetachPropertyChangedEvents(IPlayable source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.ImagesChanged -= ImagesChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
        }

        /// <summary>
        /// The source data that went into this merged instance.
        /// </summary>
        public IReadOnlyList<IPlayableCollectionGroup> Sources { get; }

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
        public ICore SourceCore => PreferredSource.SourceCore;

        /// <inheritdoc/>
        public string Id => PreferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url => PreferredSource.Url;

        /// <inheritdoc/>
        public string Name => PreferredSource.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _images;

        /// <inheritdoc/>
        public string? Description => PreferredSource.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PreferredSource.PlaybackState;

        /// <inheritdoc/>
        public TimeSpan Duration { get; } = new TimeSpan(0);

        /// <inheritdoc/>
        public virtual bool IsPlayAsyncSupported => PreferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsPauseAsyncSupported => PreferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncSupported => PreferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeImagesAsyncSupported => PreferredSource.IsChangeImagesAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncSupported => PreferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncSupported => PreferredSource.IsChangeDurationAsyncSupported;

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
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return PreferredSource.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return PreferredSource.PlayAsync();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            // The items in this Merged source are its own thing once we merge it, so any offset / limit passed here are completely disregarding the original source

            // Create a new collection that contains all data from the merged sources, even for data we don't have. Store the original offset of each and get it as needed.

            // Two ways of sorting the data:
            // Alternating until all sources run out
            // In order by rank
            var limitRemainder = limit % Sources.Count;
            var limitPerSource = (limit - limitRemainder) / Sources.Count;

            foreach (var source in Sources)
            {
                var remainingItems = source.TotalAlbumsCount - source.Albums.Count;

                if (remainingItems > 0)
                    await source.PopulateAlbumsAsync(limitPerSource);
            }

            return Albums;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].PopulateArtistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            // TODO
            var results = await Sources[0].PopulateChildrenAsync(limit, offset);
            _children.InsertRange(offset, results);

            return results;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].PopulatePlaylistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].PopulateTracksAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return Sources.InParallel(source => source.IsChangeNameAsyncSupported ? source.ChangeNameAsync(name) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            return Sources.InParallel(source => source.IsChangeImagesAsyncSupported ? source.ChangeImagesAsync(images) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return Sources.InParallel(source => source.IsChangeDescriptionAsyncSupported ? source.ChangeDescriptionAsync(description) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return Sources.InParallel(source => source.IsChangeDurationAsyncSupported ? source.ChangeDurationAsync(duration) : Task.CompletedTask);
        }
    }
}
