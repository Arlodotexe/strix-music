// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
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
    /// An implementation of <see cref="IPlayableCollectionGroup"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class PlayableCollectionGroupPluginBase : IModelPlugin, IPlayableCollectionGroup, IDelegatable<IPlayableCollectionGroup>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlayableCollectionGroupPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        internal protected PlayableCollectionGroupPluginBase(ModelPluginMetadata registration, IPlayableCollectionGroup inner)
        {
            Metadata = registration;
            Inner = inner;
            InnerArtistCollection = inner;
            InnerAlbumCollection = inner;
            InnerPlaylistCollection = inner;
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
        public IPlayableCollectionGroup Inner { get; set; }

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
        public IAlbumCollection InnerAlbumCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IPlaylistCollection InnerPlaylistCollection { get; set; }

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
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc />
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc />
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => InnerDownloadable.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc />
        public virtual Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => ((IMerged<ICoreTrackCollection>)InnerTrackCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)InnerArtistCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => ((IMerged<ICoreArtistCollection>)InnerArtistCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbumCollectionItem>)InnerAlbumCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => ((IMerged<ICoreAlbumCollection>)InnerAlbumCollection).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => ((IMerged<ICorePlaylistCollectionItem>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => ((IMerged<ICorePlaylistCollection>)Inner).Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => ((IMerged<ICorePlayableCollectionGroupChildren>)Inner).Sources;

        /// <inheritdoc />
        public IReadOnlyList<ICorePlayableCollectionGroup> Sources => ((IMerged<ICorePlayableCollectionGroup>)Inner).Sources;

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerUrlCollection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => InnerUrlCollection.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc />
        public virtual Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc />
        public virtual event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerImageCollection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => InnerImageCollection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => InnerImageCollection.ImagesChanged += value;
            remove => InnerImageCollection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public virtual string Id => InnerPlayable.Id;

        /// <inheritdoc />
        public virtual string Name => InnerPlayable.Name;

        /// <inheritdoc />
        public virtual string? Description => InnerPlayable.Description;

        /// <inheritdoc />
        public virtual DateTime? LastPlayed => InnerPlayable.LastPlayed;

        /// <inheritdoc />
        public virtual PlaybackState PlaybackState => InnerPlayable.PlaybackState;

        /// <inheritdoc />
        public virtual TimeSpan Duration => InnerPlayable.Duration;

        /// <inheritdoc />
        public virtual bool IsChangeNameAsyncAvailable => InnerPlayable.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeDescriptionAsyncAvailable => InnerPlayable.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeDurationAsyncAvailable => InnerPlayable.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public virtual Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => InnerPlayable.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc />
        public virtual Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public virtual Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => InnerPlayable.PlaybackStateChanged += value;
            remove => InnerPlayable.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<string>? NameChanged
        {
            add => InnerPlayable.NameChanged += value;
            remove => InnerPlayable.NameChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<string?>? DescriptionChanged
        {
            add => InnerPlayable.DescriptionChanged += value;
            remove => InnerPlayable.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<TimeSpan>? DurationChanged
        {
            add => InnerPlayable.DurationChanged += value;
            remove => InnerPlayable.DurationChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => InnerPlayable.LastPlayedChanged += value;
            remove => InnerPlayable.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeNameAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDurationAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual DateTime? AddedAt => Inner.AddedAt;

        /// <inheritdoc />
        public virtual int TotalTrackCount => InnerTrackCollection.TotalTrackCount;

        /// <inheritdoc />
        public virtual bool IsPlayTrackCollectionAsyncAvailable => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsPauseTrackCollectionAsyncAvailable => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => InnerTrackCollection.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => InnerTrackCollection.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => InnerTrackCollection.RemoveTrackAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerTrackCollection.IsAddTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerTrackCollection.IsRemoveTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => InnerTrackCollection.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => InnerTrackCollection.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<int>? TracksCountChanged
        {
            add => InnerTrackCollection.TracksCountChanged += value;
            remove => InnerTrackCollection.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreTrackCollection other) => InnerTrackCollection.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => InnerTrackCollection.PlayTrackCollectionAsync(track, cancellationToken);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerTrackCollection.GetTracksAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => InnerTrackCollection.AddTrackAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => InnerTrackCollection.TracksChanged += value;
            remove => InnerTrackCollection.TracksChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalArtistItemsCount => InnerArtistCollection.TotalArtistItemsCount;

        /// <inheritdoc />
        public virtual bool IsPlayArtistCollectionAsyncAvailable => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsPauseArtistCollectionAsyncAvailable => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => InnerArtistCollection.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => InnerArtistCollection.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => InnerArtistCollection.RemoveArtistItemAsync(index, cancellationToken);

        /// <inheritdoc />
        /// <inheritdoc />
        public virtual Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerArtistCollection.IsAddArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerArtistCollection.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<int>? ArtistItemsCountChanged
        {
            add => InnerArtistCollection.ArtistItemsCountChanged += value;
            remove => InnerArtistCollection.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreArtistCollectionItem other) => InnerArtistCollection.Equals(other);

        /// <inheritdoc />
        public virtual bool Equals(ICoreArtistCollection other) => InnerArtistCollection.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => InnerArtistCollection.PlayArtistCollectionAsync(artistItem, cancellationToken);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerArtistCollection.GetArtistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => InnerArtistCollection.AddArtistItemAsync(artistItem, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => InnerArtistCollection.ArtistItemsChanged += value;
            remove => InnerArtistCollection.ArtistItemsChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalAlbumItemsCount => InnerAlbumCollection.TotalAlbumItemsCount;

        /// <inheritdoc />
        public virtual bool IsPlayAlbumCollectionAsyncAvailable => InnerAlbumCollection.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsPauseAlbumCollectionAsyncAvailable => InnerAlbumCollection.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => InnerAlbumCollection.PlayAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => InnerAlbumCollection.PauseAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => InnerAlbumCollection.RemoveAlbumItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerAlbumCollection.IsAddAlbumItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerAlbumCollection.IsRemoveAlbumItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => InnerAlbumCollection.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => InnerAlbumCollection.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => InnerAlbumCollection.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => InnerAlbumCollection.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<int>? AlbumItemsCountChanged
        {
            add => InnerAlbumCollection.AlbumItemsCountChanged += value;
            remove => InnerAlbumCollection.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreAlbumCollectionItem other) => InnerAlbumCollection.Equals(other);

        /// <inheritdoc />
        public virtual bool Equals(ICoreAlbumCollection other) => InnerAlbumCollection.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => InnerAlbumCollection.PlayAlbumCollectionAsync(albumItem, cancellationToken);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerAlbumCollection.GetAlbumItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddAlbumItemAsync(IAlbumCollectionItem album, int index, CancellationToken cancellationToken = default) => InnerAlbumCollection.AddAlbumItemAsync(album, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => InnerAlbumCollection.AlbumItemsChanged += value;
            remove => InnerAlbumCollection.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalPlaylistItemsCount => InnerPlaylistCollection.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public virtual bool IsPlayPlaylistCollectionAsyncAvailable => InnerPlaylistCollection.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsPausePlaylistCollectionAsyncAvailable => InnerPlaylistCollection.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => InnerPlaylistCollection.PlayPlaylistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => InnerPlaylistCollection.PausePlaylistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => InnerPlaylistCollection.RemovePlaylistItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerPlaylistCollection.IsAddPlaylistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerPlaylistCollection.IsRemovePlaylistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged
        {
            add => InnerPlaylistCollection.IsPlayPlaylistCollectionAsyncAvailableChanged += value;
            remove => InnerPlaylistCollection.IsPlayPlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged
        {
            add => InnerPlaylistCollection.IsPausePlaylistCollectionAsyncAvailableChanged += value;
            remove => InnerPlaylistCollection.IsPausePlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<int>? PlaylistItemsCountChanged
        {
            add => InnerPlaylistCollection.PlaylistItemsCountChanged += value;
            remove => InnerPlaylistCollection.PlaylistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICorePlaylistCollectionItem other) => InnerPlaylistCollection.Equals(other);

        /// <inheritdoc />
        public virtual bool Equals(ICorePlaylistCollection other) => InnerPlaylistCollection.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => InnerPlaylistCollection.PlayPlaylistCollectionAsync(playlistItem, cancellationToken);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerPlaylistCollection.GetPlaylistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => InnerPlaylistCollection.AddPlaylistItemAsync(playlistItem, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => InnerPlaylistCollection.PlaylistItemsChanged += value;
            remove => InnerPlaylistCollection.PlaylistItemsChanged -= value;
        }

        /// <inheritdoc />
        public virtual Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => Inner.PlayPlayableCollectionGroupAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => Inner.PausePlayableCollectionGroupAsync(cancellationToken);

        /// <inheritdoc />
        public virtual int TotalChildrenCount => Inner.TotalChildrenCount;

        /// <inheritdoc />
        public virtual Task RemoveChildAsync(int index, CancellationToken cancellationToken = default) => Inner.RemoveChildAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsAddChildAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsRemoveChildAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<int>? ChildrenCountChanged
        {
            add => Inner.ChildrenCountChanged += value;
            remove => Inner.ChildrenCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICorePlayableCollectionGroupChildren other) => Inner.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default) => Inner.PlayPlayableCollectionGroupAsync(collectionGroup, cancellationToken);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => Inner.GetChildrenAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default) => Inner.AddChildAsync(child, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged
        {
            add => Inner.ChildItemsChanged += value;
            remove => Inner.ChildItemsChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICorePlayableCollectionGroup other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerArtistCollection,
                InnerAlbumCollection,
                InnerPlaylistCollection,
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
