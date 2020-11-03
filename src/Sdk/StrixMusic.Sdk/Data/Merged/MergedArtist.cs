using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreArtist"/> into a single <see cref="IArtist"/>
    /// </summary>
    public class MergedArtist : IArtist, IMerged<ICoreArtist>
    {
        private readonly ICoreArtist _preferredSource;
        private readonly List<ICoreArtist> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="MergedArtist"/>.
        /// </summary>
        /// <param name="sources">The sources used</param>
        public MergedArtist(IReadOnlyList<ICoreArtist> sources)
        {
            _sources = sources?.ToList() ?? throw new ArgumentNullException();

            Images = new SynchronizedObservableCollection<IImage>();

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];
        }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => Sources.Select(x => x.SourceCore).ToList();

        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The item to merge into this Artist</param>
        public void AddSource(ICoreArtist itemToMerge)
        {
            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _sources.Add(itemToMerge);
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> ISdkMember<ICoreArtist>.Sources => Sources;

        /// <summary>
        /// The merged sources for this <see cref="IArtist"/>
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => _sources;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

        /// <inheritdoc />
        public Uri? Url => _preferredSource.Url;

        /// <inheritdoc />
        public string Name => _preferredSource.Name;

        /// <inheritdoc />
        public string? Description => _preferredSource.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _preferredSource.PlaybackState;

        /// <inheritdoc />
        public TimeSpan Duration => _preferredSource.Duration;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _preferredSource.Genres;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _preferredSource.TotalTracksCount;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _preferredSource.TotalAlbumItemsCount;

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => _preferredSource.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _preferredSource.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _preferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _preferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _preferredSource.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _preferredSource.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _preferredSource.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _preferredSource.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _preferredSource.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _preferredSource.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _preferredSource.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => _preferredSource.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public async Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            var albums = new List<ICoreAlbum>();
            var albumCollections = new List<ICoreAlbumCollection>();

            // Using one source for now.
            await foreach (var item in _preferredSource.GetAlbumItemsAsync(limit, offset))
            {
                if (item is ICoreAlbum album)
                    albums.Add(album);

                if (item is ICoreAlbumCollection collection)
                    albumCollections.Add(collection);
            }

            // Turn each item into an Sdk Member
            var mergedAlbums = albums.Select(x => new MergedAlbum(new List<ICoreAlbum> { x }));
            var mergedCollections = albumCollections.Select(x => new MergedAlbumCollection(new List<ICoreAlbumCollection> {x}));

            // Handle possible multiple enumeration.
            var albumCollectionItems = mergedAlbums as MergedAlbum[] ?? mergedAlbums.ToArray();

            return mergedCollections.Union<IAlbumCollectionItem>(albumCollectionItems).ToList();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return _preferredSource.PlayAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return _preferredSource.PauseAsync();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            return _preferredSource.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            return _preferredSource.ChangeDescriptionAsync(description);
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return _preferredSource.ChangeDurationAsync(duration);
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            return _preferredSource.RemoveTrackAsync(index);
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
        {
            return _preferredSource.RemoveAlbumItemAsync(index);
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _preferredSource.NameChanged += value;
            remove => _preferredSource.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _preferredSource.DescriptionChanged += value;
            remove => _preferredSource.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _preferredSource.PlaybackStateChanged += value;
            remove => _preferredSource.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _preferredSource.UrlChanged += value;
            remove => _preferredSource.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _preferredSource.DurationChanged += value;
            remove => _preferredSource.DurationChanged -= value;
        }

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreArtist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreArtist other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _preferredSource.GetHashCode();
        }
    }
}
