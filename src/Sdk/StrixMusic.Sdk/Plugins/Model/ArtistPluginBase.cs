// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

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
    /// An implementation of <see cref="IArtist"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class ArtistPluginBase : IModelPlugin, IArtist, IDelegatable<IArtist>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArtistPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal ArtistPluginBase(ModelPluginMetadata registration, IArtist inner)
        {
            Registration = registration;
            Inner = inner;
            InnerAlbumCollection = inner;
            InnerTrackCollection = inner;
            InnerImageCollection = inner;
            InnerUrlCollection = inner;
            InnerGenreCollection = inner;
            InnerPlayable = inner;
            InnerDownloadable = inner;
        }

        /// <summary>
        /// Metadata about the plugin which was provided during registration.
        /// </summary>
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IArtist Inner { get; set; }

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
        public IAlbumCollection InnerAlbumCollection { get; set; }

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
        public IGenreCollection InnerGenreCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IUrlCollection InnerUrlCollection { get; set; }

        /// <inheritdoc/>
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation) => InnerDownloadable.StartDownloadOperationAsync(operation);

        /// <inheritdoc/>
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

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

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreArtist>)Inner).SourceCores;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreGenreCollection> Sources => InnerGenreCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtist>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbum>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreArtist> IMerged<ICoreArtist>.Sources => ((IMerged<ICoreArtist>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources =>
            ((IMerged<ICoreTrackCollection>)InnerTrackCollection).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources =>
            ((IMerged<ICoreAlbumCollection>)InnerAlbumCollection).Sources;

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index) => InnerImageCollection.AddImageAsync(image, index);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => InnerImageCollection.ImagesChanged += value;
            remove => InnerImageCollection.ImagesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index) => InnerUrlCollection.RemoveUrlAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual int TotalGenreCount => InnerGenreCollection.TotalGenreCount;

        /// <inheritdoc/>
        public virtual Task RemoveGenreAsync(int index) => InnerGenreCollection.RemoveGenreAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddGenreAvailableAsync(int index) => InnerGenreCollection.IsAddGenreAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveGenreAvailableAsync(int index) => InnerGenreCollection.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc/>
        public virtual event EventHandler<int>? GenresCountChanged
        {
            add => InnerGenreCollection.GenresCountChanged += value;
            remove => InnerGenreCollection.GenresCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreGenreCollection other) => InnerGenreCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => InnerGenreCollection.GetGenresAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddGenreAsync(IGenre genre, int index) => InnerGenreCollection.AddGenreAsync(genre, index);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => InnerGenreCollection.GenresChanged += value;
            remove => InnerGenreCollection.GenresChanged -= value;
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
        public virtual Task ChangeNameAsync(string name) => InnerPlayable.ChangeNameAsync(name);

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description) => InnerPlayable.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration) => InnerPlayable.ChangeDurationAsync(duration);

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
        public virtual Task PlayTrackCollectionAsync() => InnerTrackCollection.PlayTrackCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PauseTrackCollectionAsync() => InnerTrackCollection.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public virtual Task RemoveTrackAsync(int index) => InnerTrackCollection.RemoveTrackAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddTrackAvailableAsync(int index) => InnerTrackCollection.IsAddTrackAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveTrackAvailableAsync(int index) => InnerTrackCollection.IsRemoveTrackAvailableAsync(index);

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
        public virtual Task PlayTrackCollectionAsync(ITrack track) => InnerTrackCollection.PlayTrackCollectionAsync(track);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => InnerTrackCollection.GetTracksAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddTrackAsync(ITrack track, int index) => InnerTrackCollection.AddTrackAsync(track, index);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => InnerTrackCollection.TracksChanged += value;
            remove => InnerTrackCollection.TracksChanged -= value;
        }

        /// <inheritdoc/>
        public virtual int TotalAlbumItemsCount => InnerAlbumCollection.TotalAlbumItemsCount;

        /// <inheritdoc/>
        public virtual bool IsPlayAlbumCollectionAsyncAvailable => InnerAlbumCollection.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseAlbumCollectionAsyncAvailable => InnerAlbumCollection.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual Task PlayAlbumCollectionAsync() => InnerAlbumCollection.PlayAlbumCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PauseAlbumCollectionAsync() => InnerAlbumCollection.PauseAlbumCollectionAsync();

        /// <inheritdoc/>
        public virtual Task RemoveAlbumItemAsync(int index) => InnerAlbumCollection.RemoveAlbumItemAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddAlbumItemAvailableAsync(int index) => InnerAlbumCollection.IsAddAlbumItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => InnerAlbumCollection.IsRemoveAlbumItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => InnerAlbumCollection.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => InnerAlbumCollection.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => InnerAlbumCollection.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => InnerAlbumCollection.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? AlbumItemsCountChanged
        {
            add => InnerAlbumCollection.AlbumItemsCountChanged += value;
            remove => InnerAlbumCollection.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreAlbumCollectionItem other) => InnerAlbumCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreAlbumCollection other) => InnerAlbumCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem) => InnerAlbumCollection.PlayAlbumCollectionAsync(albumItem);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => InnerAlbumCollection.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => InnerAlbumCollection.AddAlbumItemAsync(album, index);

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => InnerAlbumCollection.AlbumItemsChanged += value;
            remove => InnerAlbumCollection.AlbumItemsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreArtistCollectionItem other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreArtist other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual IPlayableCollectionGroup? RelatedItems => Inner.RelatedItems;

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerTrackCollection,
                InnerAlbumCollection,
                InnerDownloadable,
                InnerPlayable,
                InnerImageCollection,
                InnerUrlCollection,
                InnerGenreCollection,
            };

            return new ValueTask(uniqueInstances.AsParallel()
                .Select(x => x.DisposeAsync().AsTask())
                .Aggregate((x, y) => Task.WhenAll(x, y)));
        }
    }
}
