﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="ITrackCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class TrackCollectionPluginBase : IModelPlugin, ITrackCollection, IDelegatable<ITrackCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TrackCollectionPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal TrackCollectionPluginBase(ModelPluginMetadata registration, ITrackCollection inner)
        {
            Metadata = registration;
            Inner = inner;
            InnerUrlCollection = inner;
            InnerImageCollection = inner;
            InnerPlayable = inner;
            InnerDownloadable = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public ITrackCollection Inner { get; set; }

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
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc/>
        virtual public int TotalTrackCount => Inner.TotalTrackCount;

        /// <inheritdoc/>
        virtual public bool IsPlayTrackCollectionAsyncAvailable => Inner.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        virtual public bool IsPauseTrackCollectionAsyncAvailable => Inner.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        virtual public DateTime? AddedAt => Inner.AddedAt;

        /// <inheritdoc/>
        virtual public string Id => InnerPlayable.Id;

        /// <inheritdoc/>
        virtual public string Name => InnerPlayable.Name;

        /// <inheritdoc/>
        virtual public string? Description => InnerPlayable.Description;

        /// <inheritdoc/>
        virtual public DateTime? LastPlayed => InnerPlayable.LastPlayed;

        /// <inheritdoc/>
        virtual public PlaybackState PlaybackState => InnerPlayable.PlaybackState;

        /// <inheritdoc/>
        virtual public TimeSpan Duration => InnerPlayable.Duration;

        /// <inheritdoc/>
        virtual public bool IsChangeNameAsyncAvailable => InnerPlayable.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        virtual public bool IsChangeDescriptionAsyncAvailable => InnerPlayable.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        virtual public bool IsChangeDurationAsyncAvailable => InnerPlayable.IsChangeDurationAsyncAvailable;

        /// <inheritdoc/>
        virtual public DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc/>
        virtual public int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc/>
        virtual public int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreTrackCollection> Sources => ((IMerged<ICoreTrackCollection>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => Inner.TracksChanged += value;
            remove => Inner.TracksChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? TracksCountChanged
        {
            add => Inner.TracksCountChanged += value;
            remove => Inner.TracksCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => Inner.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => Inner.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => InnerPlayable.PlaybackStateChanged += value;
            remove => InnerPlayable.PlaybackStateChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<string>? NameChanged
        {
            add => InnerPlayable.NameChanged += value;
            remove => InnerPlayable.NameChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<string?>? DescriptionChanged
        {
            add => InnerPlayable.DescriptionChanged += value;
            remove => InnerPlayable.DescriptionChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<TimeSpan>? DurationChanged
        {
            add => InnerPlayable.DurationChanged += value;
            remove => InnerPlayable.DurationChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => InnerPlayable.LastPlayedChanged += value;
            remove => InnerPlayable.LastPlayedChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeNameAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDurationAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDurationAsyncAvailableChanged -= value;
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
        virtual public event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => InnerImageCollection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => Inner.AddTrackAsync(track, index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => InnerUrlCollection.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc/>
        virtual public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc/>
        virtual public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => InnerPlayable.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc/>
        virtual public bool Equals(ICoreTrackCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        virtual public bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        virtual public bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        virtual public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerImageCollection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        virtual public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => Inner.GetTracksAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        virtual public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerUrlCollection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsAddTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsRemoveTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => Inner.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        virtual public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => Inner.PlayTrackCollectionAsync(track, cancellationToken);

        /// <inheritdoc/>
        virtual public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => Inner.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        virtual public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => Inner.RemoveTrackAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => InnerDownloadable.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc/>
        virtual public ValueTask DisposeAsync()
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
