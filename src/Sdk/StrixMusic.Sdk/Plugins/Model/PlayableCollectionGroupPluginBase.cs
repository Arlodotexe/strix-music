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
    /// An implementation of <see cref="IPlayableCollectionGroup"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class PlayableCollectionGroupPluginBase : IModelPlugin, IPlayableCollectionGroup, IPlaylistCollection, IAlbumCollection, IArtistCollection, ITrackCollection, IPlayable, IImageCollection, IUrlCollection, IDownloadable, IDelegatable<IPlayableCollectionGroup>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlayableCollectionGroupPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal PlayableCollectionGroupPluginBase(ModelPluginMetadata registration, IPlayableCollectionGroup inner)
        {
            Registration = registration;
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
        public ModelPluginMetadata Registration { get; }

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

        /// <inheritdoc />
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc />
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation) => InnerDownloadable.StartDownloadOperationAsync(operation);

        /// <inheritdoc />
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc />
        public virtual Task RemoveUrlAsync(int index) => InnerUrlCollection.RemoveUrlAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => InnerUrlCollection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index);

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
        public IReadOnlyList<ICore> SourceCores => InnerUrlCollection.SourceCores;

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => InnerUrlCollection.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddUrlAsync(IUrl url, int index) => InnerUrlCollection.AddUrlAsync(url, index);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc />
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => InnerImageCollection.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => InnerImageCollection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task RemoveImageAsync(int index) => InnerImageCollection.RemoveImageAsync(index);

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
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => InnerImageCollection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddImageAsync(IImage image, int index) => InnerImageCollection.AddImageAsync(image, index);

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
        public virtual Task ChangeNameAsync(string name) => InnerPlayable.ChangeNameAsync(name);

        /// <inheritdoc />
        public virtual Task ChangeDescriptionAsync(string? description) => InnerPlayable.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public virtual Task ChangeDurationAsync(TimeSpan duration) => InnerPlayable.ChangeDurationAsync(duration);

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
        public virtual Task PlayTrackCollectionAsync() => InnerTrackCollection.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public virtual Task PauseTrackCollectionAsync() => InnerTrackCollection.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public virtual Task RemoveTrackAsync(int index) => InnerTrackCollection.RemoveTrackAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsAddTrackAvailableAsync(int index) => InnerTrackCollection.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveTrackAvailableAsync(int index) => InnerTrackCollection.IsRemoveTrackAvailableAsync(index);

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
        public virtual Task PlayTrackCollectionAsync(ITrack track) => InnerTrackCollection.PlayTrackCollectionAsync(track);

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => InnerTrackCollection.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddTrackAsync(ITrack track, int index) => InnerTrackCollection.AddTrackAsync(track, index);

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
        public virtual Task PlayArtistCollectionAsync() => InnerArtistCollection.PlayArtistCollectionAsync();

        /// <inheritdoc />
        public virtual Task PauseArtistCollectionAsync() => InnerArtistCollection.PauseArtistCollectionAsync();

        /// <inheritdoc />
        public virtual Task RemoveArtistItemAsync(int index) => InnerArtistCollection.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        /// <inheritdoc />
        public virtual Task<bool> IsAddArtistItemAvailableAsync(int index) => InnerArtistCollection.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveArtistItemAvailableAsync(int index) => InnerArtistCollection.IsRemoveArtistItemAvailableAsync(index);

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
        public virtual Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => InnerArtistCollection.PlayArtistCollectionAsync(artistItem);

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => InnerArtistCollection.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => InnerArtistCollection.AddArtistItemAsync(artist, index);

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
        public virtual Task PlayAlbumCollectionAsync() => InnerAlbumCollection.PlayAlbumCollectionAsync();

        /// <inheritdoc />
        public virtual Task PauseAlbumCollectionAsync() => InnerAlbumCollection.PauseAlbumCollectionAsync();

        /// <inheritdoc />
        public virtual Task RemoveAlbumItemAsync(int index) => InnerAlbumCollection.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsAddAlbumItemAvailableAsync(int index) => InnerAlbumCollection.IsAddAlbumItemAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => InnerAlbumCollection.IsRemoveAlbumItemAvailableAsync(index);

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
        public virtual Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem) => InnerAlbumCollection.PlayAlbumCollectionAsync(albumItem);

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => InnerAlbumCollection.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => InnerAlbumCollection.AddAlbumItemAsync(album, index);

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
        public virtual Task PlayPlaylistCollectionAsync() => InnerPlaylistCollection.PlayPlaylistCollectionAsync();

        /// <inheritdoc />
        public virtual Task PausePlaylistCollectionAsync() => InnerPlaylistCollection.PausePlaylistCollectionAsync();

        /// <inheritdoc />
        public virtual Task RemovePlaylistItemAsync(int index) => InnerPlaylistCollection.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsAddPlaylistItemAvailableAsync(int index) => InnerPlaylistCollection.IsAddPlaylistItemAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => InnerPlaylistCollection.IsRemovePlaylistItemAvailableAsync(index);

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
        public virtual Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem) => InnerPlaylistCollection.PlayPlaylistCollectionAsync(playlistItem);

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset) => InnerPlaylistCollection.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => InnerPlaylistCollection.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => InnerPlaylistCollection.PlaylistItemsChanged += value;
            remove => InnerPlaylistCollection.PlaylistItemsChanged -= value;
        }

        /// <inheritdoc />
        public virtual Task PlayPlayableCollectionGroupAsync() => Inner.PlayPlayableCollectionGroupAsync();

        /// <inheritdoc />
        public virtual Task PausePlayableCollectionGroupAsync() => Inner.PausePlayableCollectionGroupAsync();

        /// <inheritdoc />
        public virtual int TotalChildrenCount => Inner.TotalChildrenCount;

        /// <inheritdoc />
        public virtual Task RemoveChildAsync(int index) => Inner.RemoveChildAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsAddChildAvailableAsync(int index) => Inner.IsAddChildAvailableAsync(index);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveChildAvailableAsync(int index) => Inner.IsRemoveChildAvailableAsync(index);

        /// <inheritdoc />
        public virtual event EventHandler<int>? ChildrenCountChanged
        {
            add => Inner.ChildrenCountChanged += value;
            remove => Inner.ChildrenCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICorePlayableCollectionGroupChildren other) => Inner.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup) => Inner.PlayPlayableCollectionGroupAsync(collectionGroup);

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset) => Inner.GetChildrenAsync(limit, offset);

        /// <inheritdoc />
        public virtual Task AddChildAsync(IPlayableCollectionGroup child, int index) => Inner.AddChildAsync(child, index);

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
