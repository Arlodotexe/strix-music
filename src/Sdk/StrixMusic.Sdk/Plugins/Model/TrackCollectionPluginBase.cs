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
    /// An implementation of <see cref="ITrackCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class TrackCollectionPluginBase : ITrackCollection, IDelegatable<ITrackCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected TrackCollectionPluginBase(ModelPluginMetadata registration, ITrackCollection inner)
        {
            Registration = registration;
            Inner = inner;
            InnerUrlCollection = inner;
            InnerImageCollection = inner;
            InnerPlayable = inner;
            InnerDownloadable = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

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
        public virtual int TotalTrackCount => Inner.TotalTrackCount;

        /// <inheritdoc/>
        public virtual bool IsPlayTrackCollectionAsyncAvailable => Inner.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseTrackCollectionAsyncAvailable => Inner.IsPauseTrackCollectionAsyncAvailable;

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

        /// <inheritdoc/>
        public IReadOnlyList<ICoreTrackCollection> Sources => ((IMerged<ICoreTrackCollection>)Inner).Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreTrackCollection>)Inner).SourceCores;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICore> IMerged<ICoreImageCollection>.SourceCores => InnerImageCollection.SourceCores;

        /// <inheritdoc/>
        IReadOnlyList<ICore> IMerged<ICoreUrlCollection>.SourceCores => InnerUrlCollection.SourceCores;

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => Inner.TracksChanged += value;
            remove => Inner.TracksChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? TracksCountChanged
        {
            add => Inner.TracksCountChanged += value;
            remove => Inner.TracksCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => Inner.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => Inner.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => Inner.IsPauseTrackCollectionAsyncAvailableChanged -= value;
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
        public virtual Task AddTrackAsync(ITrack track, int index) => Inner.AddTrackAsync(track, index);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description) => InnerPlayable.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration) => InnerPlayable.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public virtual Task ChangeNameAsync(string name) => InnerPlayable.ChangeNameAsync(name);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreTrackCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => Inner.GetTracksAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddTrackAvailableAsync(int index) => Inner.IsAddTrackAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveTrackAvailableAsync(int index) => Inner.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task PauseTrackCollectionAsync() => Inner.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PlayTrackCollectionAsync(ITrack track) => Inner.PlayTrackCollectionAsync(track);

        /// <inheritdoc/>
        public virtual Task PlayTrackCollectionAsync() => Inner.PlayTrackCollectionAsync();

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveTrackAsync(int index) => Inner.RemoveTrackAsync(index);

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
