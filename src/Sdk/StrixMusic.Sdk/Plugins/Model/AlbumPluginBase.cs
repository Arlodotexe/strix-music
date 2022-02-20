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
    /// An implementation of <see cref="IAlbum"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class AlbumPluginBase : IModelPlugin, IAlbum, IDelegatable<IAlbum>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AlbumPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal AlbumPluginBase(ModelPluginMetadata registration, IAlbum inner)
        {
            Registration = registration;
            Inner = inner;
            InnerArtistCollection = inner;
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
        public IAlbum Inner { get; set; }

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
        public IArtistCollection InnerArtistCollection { get; set; }

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
        public virtual int TotalGenreCount => InnerGenreCollection.TotalGenreCount;

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreAlbum>)Inner).SourceCores;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreAlbum> Sources => ((IMerged<ICoreAlbum>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => ((IMerged<ICoreAlbum>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbum>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources =>
            ((IMerged<ICoreTrackCollection>)InnerTrackCollection).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources =>
            ((IMerged<ICoreArtistCollectionItem>)InnerArtistCollection).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources =>
            ((IMerged<ICoreArtistCollection>)InnerArtistCollection).Sources;

        /// <inheritdoc/>
        public virtual int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc/>
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc/>
        public virtual int TotalTrackCount => InnerTrackCollection.TotalTrackCount;

        /// <inheritdoc/>
        public virtual bool IsPlayTrackCollectionAsyncAvailable => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseTrackCollectionAsyncAvailable => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailable;

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
        public virtual int TotalArtistItemsCount => InnerArtistCollection.TotalArtistItemsCount;

        /// <inheritdoc/>
        public virtual bool IsPlayArtistCollectionAsyncAvailable => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseArtistCollectionAsyncAvailable =>
            InnerArtistCollection.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => InnerGenreCollection.GenresChanged += value;
            remove => InnerGenreCollection.GenresChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? GenresCountChanged
        {
            add => InnerGenreCollection.GenresCountChanged += value;
            remove => InnerGenreCollection.GenresCountChanged -= value;
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
        public virtual event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => InnerTrackCollection.TracksChanged += value;
            remove => InnerTrackCollection.TracksChanged -= value;
        }

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
        public virtual event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => InnerArtistCollection.ArtistItemsChanged += value;
            remove => InnerArtistCollection.ArtistItemsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? ArtistItemsCountChanged
        {
            add => InnerArtistCollection.ArtistItemsCountChanged += value;
            remove => InnerArtistCollection.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task AddArtistItemAsync(IArtistCollectionItem artist, int index) =>
            InnerArtistCollection.AddArtistItemAsync(artist, index);

        /// <inheritdoc/>
        public virtual Task AddGenreAsync(IGenre genre, int index) => InnerGenreCollection.AddGenreAsync(genre, index);

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index) => InnerImageCollection.AddImageAsync(image, index);

        /// <inheritdoc/>
        public virtual Task AddTrackAsync(ITrack track, int index) => InnerTrackCollection.AddTrackAsync(track, index);

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual Task ChangeDescriptionAsync(string? description) =>
            InnerPlayable.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public virtual Task ChangeDurationAsync(TimeSpan duration) => InnerPlayable.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public virtual Task ChangeNameAsync(string name) => InnerPlayable.ChangeNameAsync(name);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerTrackCollection,
                InnerArtistCollection,
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

        /// <inheritdoc/>
        public virtual bool Equals(ICoreGenreCollection other) => InnerGenreCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreTrackCollection other) => InnerTrackCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreArtistCollectionItem other) => InnerArtistCollection.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreArtistCollection other) => InnerArtistCollection.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) =>
            InnerArtistCollection.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) =>
            InnerGenreCollection.GetGenresAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) =>
            InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) =>
            InnerTrackCollection.GetTracksAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) =>
            InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddArtistItemAvailableAsync(int index) =>
            InnerArtistCollection.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddGenreAvailableAsync(int index) => InnerGenreCollection.IsAddGenreAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddTrackAvailableAsync(int index) => InnerTrackCollection.IsAddTrackAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveArtistItemAvailableAsync(int index) =>
            InnerArtistCollection.IsRemoveArtistItemAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveGenreAvailableAsync(int index) =>
            InnerGenreCollection.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) =>
            InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveTrackAvailableAsync(int index) =>
            InnerTrackCollection.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task PauseArtistCollectionAsync() => InnerArtistCollection.PauseArtistCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PauseTrackCollectionAsync() => InnerTrackCollection.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) =>
            InnerArtistCollection.PlayArtistCollectionAsync(artistItem);

        /// <inheritdoc/>
        public virtual Task PlayArtistCollectionAsync() => InnerArtistCollection.PlayArtistCollectionAsync();

        /// <inheritdoc/>
        public virtual Task PlayTrackCollectionAsync(ITrack track) => InnerTrackCollection.PlayTrackCollectionAsync(track);

        /// <inheritdoc/>
        public virtual Task PlayTrackCollectionAsync() => InnerTrackCollection.PlayTrackCollectionAsync();

        /// <inheritdoc/>
        public virtual Task RemoveArtistItemAsync(int index) => InnerArtistCollection.RemoveArtistItemAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveGenreAsync(int index) => InnerGenreCollection.RemoveGenreAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveTrackAsync(int index) => InnerTrackCollection.RemoveTrackAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index) => InnerUrlCollection.RemoveUrlAsync(index);

        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation) =>
            InnerDownloadable.StartDownloadOperationAsync(operation);

        /// <inheritdoc/>
        public virtual DateTime? DatePublished => Inner.DatePublished;

        /// <inheritdoc />
        public virtual bool IsChangeDatePublishedAsyncAvailable => Inner.IsChangeDatePublishedAsyncAvailable;

        /// <inheritdoc />
        public virtual Task ChangeDatePublishedAsync(DateTime datePublished) => Inner.ChangeDatePublishedAsync(datePublished);

        /// <inheritdoc />
        public virtual event EventHandler<DateTime?>? DatePublishedChanged
        {
            add => Inner.DatePublishedChanged += value;
            remove => Inner.DatePublishedChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged
        {
            add => Inner.IsChangeDatePublishedAsyncAvailableChanged += value;
            remove => Inner.IsChangeDatePublishedAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public virtual bool Equals(ICoreAlbumCollectionItem other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreAlbum other) => Inner.Equals(other);

        /// <inheritdoc />
        public virtual IPlayableCollectionGroup? RelatedItems => Inner.RelatedItems;
    }
}
