// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    /// An implementation of <see cref="IPlaylist"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class PlaylistPluginBase : IModelPlugin, IPlaylist, IDelegatable<IPlaylist>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlaylistPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal PlaylistPluginBase(ModelPluginMetadata registration, IPlaylist inner)
        {
            Metadata = registration;
            Inner = inner;
            InnerTrackCollection = inner;
            InnerImageCollection = inner;
            InnerUrlCollection = inner;
            InnerPlayable = inner;
            InnerDownloadable = inner;
        }
        
        /// <summary>
        /// Metadata about the plugin which was provided during registration.
        /// </summary>
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IPlaylist Inner { get; set; }
        
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
        public ITrackCollection InnerTrackCollection { get; set; }
        
        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IImageCollection InnerImageCollection { get; set; }
        
        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IUrlCollection InnerUrlCollection { get; set; }
        
        /// <inheritdoc/>
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;
        
        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => InnerDownloadable.StartDownloadOperationAsync(operation, cancellationToken);
        
        /// <inheritdoc/>
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }
        
        /// <inheritdoc/>
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;
        
        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.RemoveUrlAsync(index, cancellationToken);
        
        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsAddUrlAvailableAsync(index, cancellationToken);
        
        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);
        
        /// <inheritdoc/>
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => ((IMerged<ICorePlaylist>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICorePlaylist> IMerged<ICorePlaylist>.Sources => ((IMerged<ICorePlaylist>)Inner).Sources;

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICore> SourceCores => ((IMerged<ICorePlaylist>)Inner).SourceCores;
        
        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources =>
            ((IMerged<ICoreTrackCollection>)InnerTrackCollection).Sources;

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerUrlCollection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => InnerUrlCollection.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc/>
        public virtual event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerImageCollection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => InnerImageCollection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => InnerImageCollection.ImagesChanged += value;
            remove => InnerImageCollection.ImagesChanged -= value;
        }

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
        public virtual Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => InnerPlayable.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDurationAsync(duration, cancellationToken);

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
        public virtual DateTime? AddedAt => Inner.AddedAt;

        /// <inheritdoc/>
        public virtual int TotalTrackCount => InnerTrackCollection.TotalTrackCount;

        /// <inheritdoc/>
        public virtual bool IsPlayTrackCollectionAsyncAvailable => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseTrackCollectionAsyncAvailable => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => InnerTrackCollection.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public virtual Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => InnerTrackCollection.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => InnerTrackCollection.RemoveTrackAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerTrackCollection.IsAddTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerTrackCollection.IsRemoveTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? TracksCountChanged
        {
            add => InnerTrackCollection.TracksCountChanged += value;
            remove => InnerTrackCollection.TracksCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreTrackCollection other) => InnerTrackCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => InnerTrackCollection.PlayTrackCollectionAsync(track, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerTrackCollection.GetTracksAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => InnerTrackCollection.AddTrackAsync(track, index, cancellationToken);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => InnerTrackCollection.TracksChanged += value;
            remove => InnerTrackCollection.TracksChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICorePlaylistCollectionItem other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICorePlaylist other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual IUserProfile? Owner => Inner.Owner;

        /// <inheritdoc/>
        public virtual IPlayableCollectionGroup? RelatedItems => Inner.RelatedItems;

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerTrackCollection,
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
