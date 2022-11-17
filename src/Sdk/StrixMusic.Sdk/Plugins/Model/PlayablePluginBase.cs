// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IPlayable"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class PlayablePluginBase : IModelPlugin, IPlayable, IDelegatable<IPlayable>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlayablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        internal protected PlayablePluginBase(ModelPluginMetadata registration, IPlayable inner)
        {
            Metadata = registration;

            Inner = inner;
            InnerDownloadable = inner;
            InnerImageCollection = inner;
            InnerUrlCollection = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

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
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

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
        public virtual Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => Inner.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => Inner.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc/>
        public virtual Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => Inner.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => InnerDownloadable.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerImageCollection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => InnerImageCollection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerUrlCollection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => InnerUrlCollection.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);
    }
}
