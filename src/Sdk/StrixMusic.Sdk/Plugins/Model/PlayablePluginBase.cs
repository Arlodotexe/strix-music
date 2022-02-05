using OwlCore.ComponentModel;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IPlayable"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class PlayablePluginBase : IPlayable, IDelegatable<IPlayable>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlayablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected PlayablePluginBase(ModelPluginMetadata registration, IPlayable inner)
        {
            Registration = registration;

            Inner = inner;
            InnerDownloadable = inner;
            InnerImageCollection = inner;
            InnerUrlCollection = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IDownloadable InnerDownloadable { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IImageCollection InnerImageCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IUrlCollection InnerUrlCollection { get; set; }

        /// <inheritdoc/>
        public IPlayable Inner { get; }

        /// <inheritdoc/>
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc/>
        public virtual string Id => Inner.Id;

        /// <inheritdoc/>
        public virtual string Name => Inner.Name;

        /// <inheritdoc/>
        public virtual string? Description => Inner.Description;

        /// <inheritdoc/>
        public virtual DateTime? LastPlayed => Inner.LastPlayed;

        /// <inheritdoc/>
        public virtual PlaybackState PlaybackState => Inner.PlaybackState;

        /// <inheritdoc/>
        public virtual TimeSpan Duration => Inner.Duration;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncAvailable => Inner.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncAvailable => Inner.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncAvailable => Inner.IsChangeDurationAsyncAvailable;

        /// <inheritdoc/>
        public virtual int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc/>
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICore> IMerged<ICoreImageCollection>.SourceCores => InnerImageCollection.SourceCores;

        /// <inheritdoc/>
        IReadOnlyList<ICore> IMerged<ICoreUrlCollection>.SourceCores => InnerUrlCollection.SourceCores;

        /// <inheritdoc/>
        public virtual event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => Inner.PlaybackStateChanged += value;
            remove => Inner.PlaybackStateChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<string>? NameChanged
        {
            add => Inner.NameChanged += value;
            remove => Inner.NameChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<string?>? DescriptionChanged
        {
            add => Inner.DescriptionChanged += value;
            remove => Inner.DescriptionChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<TimeSpan>? DurationChanged
        {
            add => Inner.DurationChanged += value;
            remove => Inner.DurationChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => Inner.LastPlayedChanged += value;
            remove => Inner.LastPlayedChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => Inner.IsChangeNameAsyncAvailableChanged += value;
            remove => Inner.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => Inner.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => Inner.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => Inner.IsChangeDurationAsyncAvailableChanged += value;
            remove => Inner.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
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
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description) => Inner.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration) => Inner.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public virtual Task ChangeNameAsync(string name) => Inner.ChangeNameAsync(name);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index) => InnerUrlCollection.RemoveUrlAsync(index);

        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation) => InnerDownloadable.StartDownloadOperationAsync(operation);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index) => InnerImageCollection.AddImageAsync(image, index);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerDownloadable,
                InnerImageCollection,
                InnerUrlCollection,
            };

            return new ValueTask(uniqueInstances.AsParallel()
                                                .Select(x => x.DisposeAsync().AsTask())
                                                .Aggregate((x, y) => Task.WhenAll(x, y)));
        }
    }
}
