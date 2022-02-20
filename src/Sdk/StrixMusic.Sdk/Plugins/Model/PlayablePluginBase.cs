﻿using System;
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
        protected internal PlayablePluginBase(ModelPluginMetadata registration, IPlayable inner)
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
        virtual public DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc/>
        virtual public string Id => Inner.Id;

        /// <inheritdoc/>
        virtual public string Name => Inner.Name;

        /// <inheritdoc/>
        virtual public string? Description => Inner.Description;

        /// <inheritdoc/>
        virtual public DateTime? LastPlayed => Inner.LastPlayed;

        /// <inheritdoc/>
        virtual public PlaybackState PlaybackState => Inner.PlaybackState;

        /// <inheritdoc/>
        virtual public TimeSpan Duration => Inner.Duration;

        /// <inheritdoc/>
        virtual public bool IsChangeNameAsyncAvailable => Inner.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        virtual public bool IsChangeDescriptionAsyncAvailable => Inner.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        virtual public bool IsChangeDurationAsyncAvailable => Inner.IsChangeDurationAsyncAvailable;

        /// <inheritdoc/>
        virtual public int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc/>
        virtual public int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICore> IMerged<ICoreImageCollection>.SourceCores => InnerImageCollection.SourceCores;

        /// <inheritdoc/>
        IReadOnlyList<ICore> IMerged<ICoreUrlCollection>.SourceCores => InnerUrlCollection.SourceCores;

        /// <inheritdoc/>
        virtual public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => Inner.PlaybackStateChanged += value;
            remove => Inner.PlaybackStateChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<string>? NameChanged
        {
            add => Inner.NameChanged += value;
            remove => Inner.NameChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<string?>? DescriptionChanged
        {
            add => Inner.DescriptionChanged += value;
            remove => Inner.DescriptionChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<TimeSpan>? DurationChanged
        {
            add => Inner.DurationChanged += value;
            remove => Inner.DurationChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => Inner.LastPlayedChanged += value;
            remove => Inner.LastPlayedChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => Inner.IsChangeNameAsyncAvailableChanged += value;
            remove => Inner.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => Inner.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => Inner.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => Inner.IsChangeDurationAsyncAvailableChanged += value;
            remove => Inner.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => InnerImageCollection.ImagesChanged += value;
            remove => InnerImageCollection.ImagesChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        virtual public Task ChangeDescriptionAsync(string? description) => Inner.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        virtual public Task ChangeDurationAsync(TimeSpan duration) => Inner.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        virtual public Task ChangeNameAsync(string name) => Inner.ChangeNameAsync(name);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

        /// <inheritdoc/>
        virtual public Task RemoveUrlAsync(int index) => InnerUrlCollection.RemoveUrlAsync(index);

        /// <inheritdoc/>
        virtual public Task StartDownloadOperationAsync(DownloadOperation operation) => InnerDownloadable.StartDownloadOperationAsync(operation);

        /// <inheritdoc/>
        virtual public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        virtual public Task AddImageAsync(IImage image, int index) => InnerImageCollection.AddImageAsync(image, index);

        /// <inheritdoc/>
        virtual public bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        virtual public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        virtual public Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        virtual public bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        virtual public ValueTask DisposeAsync()
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