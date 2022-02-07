using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IAlbumCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class AlbumCollectionPluginBase : IModelPlugin, IAlbumCollection, IDelegatable<IAlbumCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected AlbumCollectionPluginBase(ModelPluginMetadata registration, IAlbumCollection inner)
        {
            Registration = registration;
            Inner = inner;
            InnerUrlCollection = inner;
            InnerImageCollection = inner;
            InnerPlayable = inner;
            InnerDownloadable = inner;
        }

        /// <summary>
        /// Metadata about the plugin which was provided during registration.
        /// </summary>
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IAlbumCollection Inner { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IPlayable InnerPlayable { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IDownloadable InnerDownloadable { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IUrlCollection InnerUrlCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IImageCollection InnerImageCollection { get; set; }

        /// <inheritdoc/>
        public virtual int TotalAlbumItemsCount => Inner.TotalAlbumItemsCount;

        /// <inheritdoc/>
        public virtual bool IsPlayAlbumCollectionAsyncAvailable => Inner.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseAlbumCollectionAsyncAvailable => Inner.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual DateTime? AddedAt => Inner.AddedAt;

        /// <inheritdoc/>
        public virtual string Id => InnerPlayable.Id;

        /// <inheritdoc/>
        public virtual string Name => InnerPlayable.Name;

        /// <inheritdoc/>
        public virtual string? Description => InnerPlayable.Description;

        /// <inheritdoc/>
        public virtual DateTime? LastPlayed => InnerPlayable.LastPlayed;

        /// <inheritdoc/>
        public virtual PlaybackState PlaybackState => InnerPlayable.PlaybackState;

        /// <inheritdoc/>
        public virtual TimeSpan Duration => InnerPlayable.Duration;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncAvailable => InnerPlayable.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncAvailable => InnerPlayable.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncAvailable => InnerPlayable.IsChangeDurationAsyncAvailable;

        /// <inheritdoc/>
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc/>
        public virtual int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc/>
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreAlbumCollection>)Inner).SourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbumCollectionItem>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => ((IMerged<ICoreAlbumCollection>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)InnerImageCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)InnerUrlCollection).Sources;

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => Inner.AlbumItemsChanged += value;
            remove => Inner.AlbumItemsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? AlbumItemsCountChanged
        {
            add => Inner.AlbumItemsCountChanged += value;
            remove => Inner.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => Inner.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => Inner.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => InnerPlayable.PlaybackStateChanged += value;
            remove => InnerPlayable.PlaybackStateChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<string>? NameChanged
        {
            add => InnerPlayable.NameChanged += value;
            remove => InnerPlayable.NameChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<string?>? DescriptionChanged
        {
            add => InnerPlayable.DescriptionChanged += value;
            remove => InnerPlayable.DescriptionChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<TimeSpan>? DurationChanged
        {
            add => InnerPlayable.DurationChanged += value;
            remove => InnerPlayable.DurationChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => InnerPlayable.LastPlayedChanged += value;
            remove => InnerPlayable.LastPlayedChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeNameAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDurationAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => InnerImageCollection.ImagesChanged += value;
            remove => InnerImageCollection.ImagesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index) => InnerImageCollection.AddImageAsync(image, index);

        /// <inheritdoc/>
        public virtual Task AddAlbumItemAsync(IAlbumCollectionItem AlbumItem, int index) => Inner.AddAlbumItemAsync(AlbumItem, index);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description) => InnerPlayable.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration) => InnerPlayable.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public virtual Task ChangeNameAsync(string name) => InnerPlayable.ChangeNameAsync(name);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreAlbumCollectionItem other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreAlbumCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => Inner.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddAlbumItemAvailableAsync(int index) => Inner.IsAddAlbumItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => Inner.IsRemoveAlbumItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task PauseAlbumCollectionAsync() => Inner.PauseAlbumCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem) => Inner.PlayAlbumCollectionAsync(albumItem);

        /// <inheritdoc/>
        public virtual Task PlayAlbumCollectionAsync() => Inner.PlayAlbumCollectionAsync();

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveAlbumItemAsync(int index) => Inner.RemoveAlbumItemAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index) => InnerUrlCollection.RemoveUrlAsync(index);

        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation) => InnerDownloadable.StartDownloadOperationAsync(operation);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerDownloadable,
                InnerPlayable,
                InnerImageCollection,
                InnerUrlCollection,
            };

            return new ValueTask(uniqueInstances.AsParallel()
                                                .Select(x => x.DisposeAsync().AsTask())
                                                .Aggregate((x, y) => Task.WhenAll(x, y)));
        }
    }
}
