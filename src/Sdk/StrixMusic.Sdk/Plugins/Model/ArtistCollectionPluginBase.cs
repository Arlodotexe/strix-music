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
    /// An implementation of <see cref="IArtistCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class ArtistCollectionPluginBase : IModelPlugin, IArtistCollection, IDelegatable<IArtistCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal ArtistCollectionPluginBase(ModelPluginMetadata registration, IArtistCollection inner)
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
        public IArtistCollection Inner { get; set; }

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
        public virtual int TotalArtistItemsCount => Inner.TotalArtistItemsCount;

        /// <inheritdoc/>
        public virtual bool IsPlayArtistCollectionAsyncAvailable => Inner.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseArtistCollectionAsyncAvailable => Inner.IsPauseArtistCollectionAsyncAvailable;

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
        public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreArtistCollection>)Inner).SourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => ((IMerged<ICoreArtistCollection>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)InnerImageCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)InnerUrlCollection).Sources;

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => Inner.ArtistItemsChanged += value;
            remove => Inner.ArtistItemsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? ArtistItemsCountChanged
        {
            add => Inner.ArtistItemsCountChanged += value;
            remove => Inner.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => Inner.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => Inner.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPauseArtistCollectionAsyncAvailableChanged -= value;
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
        public virtual Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index) => Inner.AddArtistItemAsync(artistItem, index);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description) => InnerPlayable.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration) => InnerPlayable.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public virtual Task ChangeNameAsync(string name) => InnerPlayable.ChangeNameAsync(name);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreArtistCollectionItem other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreArtistCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => Inner.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddArtistItemAvailableAsync(int index) => Inner.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveArtistItemAvailableAsync(int index) => Inner.IsRemoveArtistItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task PauseArtistCollectionAsync() => Inner.PauseArtistCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => Inner.PlayArtistCollectionAsync(artistItem);

        /// <inheritdoc/>
        public virtual Task PlayArtistCollectionAsync() => Inner.PlayArtistCollectionAsync();

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveArtistItemAsync(int index) => Inner.RemoveArtistItemAsync(index);

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
