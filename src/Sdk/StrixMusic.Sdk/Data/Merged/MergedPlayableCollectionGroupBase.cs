using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Extensions.AsyncExtensions;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase<TCoreBase> : IPlayableCollectionGroup
        where TCoreBase : ICorePlayableCollectionGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase{T}"/> class.
        /// </summary>
        /// <param name="sources">The search results to merge.</param>
        protected MergedPlayableCollectionGroupBase(IReadOnlyList<TCoreBase> sources)
        {
            Sources = sources;

            // TODO: Use top Preferred core.
            if (sources is null)
                throw new ArgumentNullException(nameof(sources));

            PreferredSource = sources[0];

            AttachPropertyChangedEvents(PreferredSource);

            foreach (var item in sources)
            {
                TotalChildrenCount += item.TotalChildrenCount;
                TotalPlaylistItemsCount += item.TotalPlaylistItemsCount;
                TotalTracksCount += item.TotalTracksCount;
                TotalAlbumItemsCount += item.TotalAlbumItemsCount;
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                Duration += item.Duration;

                // todo: merge data as needed
                // todo 2: Don't do this in the ctor. Cores shouldn't supply data unless it's requested, otherwise we'd have data scattered around.
                // removed
            }
        }

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected ICorePlayableCollectionGroup PreferredSource { get; }

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
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc cref="ISdkMember{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => Sources.Select(x => x.SourceCore).ToList();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> ISdkMember<ICorePlayableCollectionGroupChildren>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> ISdkMember<ICorePlaylistCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> ISdkMember<ICorePlaylistCollectionItem>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> ISdkMember<ICorePlayableCollectionGroup>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc cref="ISdkMember{T}.Sources"/>
        public virtual IReadOnlyList<TCoreBase> Sources { get; }

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
        public int TotalPlaylistItemsCount { get; }

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public int TotalAlbumItemsCount { get; }

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; }

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
        public Task<bool> IsAddAlbumItemSupported(int index)
        {
            return PreferredSource.IsAddAlbumItemSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index)
        {
            return PreferredSource.IsAddArtistSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index)
        {
            return PreferredSource.IsAddPlaylistItemSupported(index);
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
        public Task<bool> IsRemoveAlbumItemSupported(int index)
        {
            return PreferredSource.IsRemoveAlbumItemSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistItemSupported(int index)
        {
            return PreferredSource.IsRemovePlaylistItemSupported(index);
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
        public async Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            // The items in this Merged source are its own thing once we merge it, so any offset / limit passed here are completely disregarding the original source

            // Create a new collection that contains all data from the merged sources, even for data we don't have. Store the original offset of each and get it as needed.

            // Two ways of sorting the data:
            // Alternating until all sources run out
            // In order by rank
            var limitRemainder = limit % Sources.Count;
            var limitPerSource = (limit - limitRemainder) / Sources.Count;

            var result = new List<IAlbumCollectionItem>();

            foreach (var source in Sources)
            {
                // TODO: private field, keep track of albums you've already populated for this core.
                // var remainingItems = source.TotalAlbumItemsCount - source.Albums.Count;
                var remainingItems = source.TotalAlbumItemsCount;

                if (remainingItems > 0)
                {
                    // TODO: Offset is not 0, map it correctly
                    await foreach (var item in source.GetAlbumItemsAsync(limitPerSource, 0))
                    {
                        // TODO: wrap with another merged and yield that

                        if (item is IAlbumBase)
                        {
                            // merge to IAlbum and add to end result
                        }

                        if (item is IAlbumCollectionBase)
                        {
                            // merge to IAlbumCollection and add to return result
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            // TODO
            return Task.FromResult<IReadOnlyList<IArtistCollectionItem>>(new List<IArtistCollectionItem>());
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset = 0)
        {
            // TODO
            return Task.FromResult<IReadOnlyList<IPlayableCollectionGroup>>(new List<IPlayableCollectionGroup>());
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset)
        {
            // TODO
            return Task.FromResult<IReadOnlyList<IPlaylistCollectionItem>>(new List<IPlaylistCollectionItem>());
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0)
        {
            // TODO
            return Task.FromResult<IReadOnlyList<ITrack>>(new List<ITrack>());
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
            var trackToAdd = track.GetSources<ICoreTrack>().First(x => x.SourceCore == PreferredSource.SourceCore);

            return PreferredSource.AddTrackAsync(trackToAdd, index);
        }

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index)
        {
            var artistToAdd = artist.GetSources().First(x => x.SourceCore == PreferredSource.SourceCore);
            return PreferredSource.AddArtistItemAsync(artistToAdd, index);
        }

        /// <inheritdoc/>
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            var albumItemToAdd = album.GetSources().First(x => x.SourceCore == PreferredSource.SourceCore);

            return PreferredSource.AddAlbumItemAsync(albumItemToAdd, index);
        }

        /// <inheritdoc/>
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index)
        {
            var playlistToAdd = playlist.GetSources().First(x => x.SourceCore == PreferredSource.SourceCore);

            return PreferredSource.AddPlaylistItemAsync(playlistToAdd, index);
        }

        /// <inheritdoc/>
        public Task AddChildAsync(IPlayableCollectionGroup child, int index)
        {
            var childToAdd = child.GetSources<ICorePlayableCollectionGroup>()
                .First(x => x.SourceCore == PreferredSource.SourceCore);

            return PreferredSource.AddChildAsync(childToAdd, index);
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
        public Task RemoveAlbumItemAsync(int index)
        {
            return PreferredSource.RemoveAlbumItemAsync(index);
        }

        /// <inheritdoc/>
        public Task RemovePlaylistItemAsync(int index)
        {
            return PreferredSource.RemovePlaylistItemAsync(index);
        }

        /// <inheritdoc/>
        public Task RemoveChildAsync(int index)
        {
            return PreferredSource.RemoveChildAsync(index);
        }
    }
}
