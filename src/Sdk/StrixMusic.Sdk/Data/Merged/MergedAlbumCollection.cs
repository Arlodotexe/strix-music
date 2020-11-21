using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreAlbumCollection"/> into a single <see cref="IAlbumCollection"/>
    /// </summary>
    public class MergedAlbumCollection : IAlbumCollection, IMerged<ICoreAlbumCollection>
    {
        private readonly List<ICoreAlbumCollection> _sources;
        private readonly ICoreAlbumCollection _preferredSource;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumMap;

        /// <summary>
        /// Creates a new instance of <see cref="MergedAlbumCollection"/>.
        /// </summary>
        /// <param name="sources">The initial sources to merge together.</param>
        public MergedAlbumCollection(IEnumerable<ICoreAlbumCollection> sources)
        {
            _sources = sources.ToList();
            _preferredSource = _sources[0];

            TotalAlbumItemsCount = _preferredSource.TotalAlbumItemsCount;
            TotalImageCount = _preferredSource.TotalImageCount;

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _albumMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);
            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreAlbumCollection source)
        {
            AttachPlayableEvents(source);
            _albumMap.ItemsChanged += AlbumMap_ItemsChanged;
            _imageMap.ItemsChanged += ImageMap_ItemsChanged;
            _albumMap.ItemsCountChanged += AlbumMap_ItemsCountChanged;
            _imageMap.ItemsCountChanged += ImageMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreAlbumCollection source)
        {
            DetachPlayableEvents(source);
        }

        private void AttachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
        }

        private void DetachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string> NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage> ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int> ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem> AlbumItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int> AlbumItemsCountChanged;

        private void ImageMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;

        }

        private void ImageMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            throw new NotImplementedException();
        }

        private void AlbumMap_ItemsCountChanged(object sender, int e)
        {
            throw new NotImplementedException();
        }

        private void AlbumMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sources.Select(x => x.SourceCore).ToList();

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreAlbumCollection> Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        public int TotalAlbumItemsCount { get; }

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddSource(ICoreAlbumCollection itemToMerge)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return other.Name.Equals(this.Name, StringComparison.InvariantCulture);
        }

        /// <inheritdoc />
        public int TotalImageCount { get; private set; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public Uri? Url { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string? Description { get; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public TimeSpan Duration { get; }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported { get; }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return TODO_IMPLEMENT_ME;
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            return TODO_IMPLEMENT_ME;
        }
    }
}