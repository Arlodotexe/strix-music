using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Extensions.AsyncExtensions;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
{
    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroup"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase : IPlayableCollectionGroup
    {
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
                // todo 2: Don't do this in the ctor. Cores shouldn't supply data unless it's requested, otherwise we'd have data scattered around.
                // removed
            }
        }

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected IPlayableCollectionGroup PreferredSource { get; }

        private void AttachPropertyChangedEvents(IPlayable source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
        }

        private void DetachPropertyChangedEvents(IPlayable source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <summary>
        /// The source data that went into this merged instance.
        /// </summary>
        public IReadOnlyList<IPlayableCollectionGroup> Sources { get; }

        /// <inheritdoc/>
        public ICore SourceCore => PreferredSource.SourceCore;

        /// <inheritdoc/>
        public string Id => PreferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url => PreferredSource.Url;

        /// <inheritdoc/>
        public string Name => PreferredSource.Name;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IImage> Images { get; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc/>
        public string? Description => PreferredSource.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PreferredSource.PlaybackState;

        /// <inheritdoc/>
        public TimeSpan Duration { get; } = new TimeSpan(0);

        /// <inheritdoc/>
        public int TotalChildrenCount { get; }

        /// <inheritdoc/>
        public int TotalPlaylistCount { get; }

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public int TotalAlbumsCount { get; }

        /// <inheritdoc/>
        public int TotalArtistsCount { get; }

        /// <inheritdoc/>
        public virtual bool IsPlayAsyncSupported => PreferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsPauseAsyncSupported => PreferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncSupported => PreferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncSupported => PreferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncSupported => PreferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index)
        {
            return PreferredSource.IsAddTrackSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddAlbumSupported(int index)
        {
            return PreferredSource.IsAddAlbumSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index)
        {
            return PreferredSource.IsAddArtistSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistSupported(int index)
        {
            return PreferredSource.IsAddPlaylistSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddChildSupported(int index)
        {
            return PreferredSource.IsAddChildSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index)
        {
            return PreferredSource.IsAddImageSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return PreferredSource.IsRemoveImageSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackSupported(int index)
        {
            return PreferredSource.IsRemoveTrackSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistSupported(int index)
        {
            return PreferredSource.IsRemoveArtistSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumSupported(int index)
        {
            return PreferredSource.IsRemoveAlbumSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistSupported(int index)
        {
            return PreferredSource.IsRemovePlaylistSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveChildSupported(int index)
        {
            return PreferredSource.IsRemoveChildSupported(index);
        }

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
        public async IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0)
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
                // TODO: private field, keep track of albums you've already populated for this core.
                // var remainingItems = source.TotalAlbumsCount - source.Albums.Count;
                var remainingItems = source.TotalAlbumsCount;

                if (remainingItems > 0)
                {
                    // TODO: Offset is not 0, map it correctly
                    await foreach (var item in source.GetAlbumsAsync(limitPerSource, 0))
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].GetArtistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].GetChildrenAsync(limit, offset);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].GetPlaylistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0)
        {
            // TODO
            return Sources[0].GetTracksAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return Sources.InParallel(source => source.IsChangeNameAsyncSupported ? source.ChangeNameAsync(name) : Task.CompletedTask);
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

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index)
        {
            return PreferredSource.AddTrackAsync(track, index);
        }

        /// <inheritdoc/>
        public Task AddArtistAsync(IArtist artist, int index)
        {
            return PreferredSource.AddArtistAsync(artist, index);
        }

        /// <inheritdoc/>
        public Task AddAlbumAsync(IAlbum album, int index)
        {
            return PreferredSource.AddAlbumAsync(album, index);
        }

        /// <inheritdoc/>
        public Task AddPlaylistAsync(IPlayableCollectionGroup playlist, int index)
        {
            return PreferredSource.AddPlaylistAsync(playlist, index);
        }

        /// <inheritdoc/>
        public Task AddChildAsync(IPlayableCollectionGroup child, int index)
        {
            return PreferredSource.AddChildAsync(child, index);
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index)
        {
            return PreferredSource.RemoveTrackAsync(index);
        }

        /// <inheritdoc/>
        public Task RemoveArtistAsync(int index)
        {
            return PreferredSource.RemoveArtistAsync(index);
        }

        /// <inheritdoc/>
        public Task RemoveAlbumAsync(int index)
        {
            return PreferredSource.RemoveAlbumAsync(index);
        }

        /// <inheritdoc/>
        public Task RemovePlaylistAsync(int index)
        {
            return PreferredSource.RemovePlaylistAsync(index);
        }

        /// <inheritdoc/>
        public Task RemoveChildAsync(int index)
        {
            return PreferredSource.RemoveChildAsync(index);
        }
    }
}
