// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IPlayableCollectionGroup"/> with the provided plugins.
/// </summary>
public abstract class PlayableCollectionGroupPluginWrapperBase : IPlayableCollectionGroup
{
    private readonly IPlayableCollectionGroup _playableCollectionGroup;
    private readonly SdkModelPlugin[] _plugins;
    private readonly SdkModelPlugin _activePlugins = new(PluginModelWrapperInfo.Metadata);

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayableCollectionGroupPluginWrapperBase"/> class.
    /// </summary>
    /// <param name="playableCollectionGroup">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal PlayableCollectionGroupPluginWrapperBase(IPlayableCollectionGroup playableCollectionGroup, params SdkModelPlugin[] plugins)
    {
        _playableCollectionGroup = playableCollectionGroup;
        _plugins = plugins;

        foreach (var item in plugins)
            _activePlugins.Import(item);

        AttachEvents(playableCollectionGroup);
    }

    private void AttachEvents(IPlayableCollectionGroup playableCollectionGroup)
    {
        playableCollectionGroup.SourcesChanged += OnSourcesChanged;
        playableCollectionGroup.ImagesCountChanged += OnImagesCountChanged;
        playableCollectionGroup.UrlsCountChanged += OnUrlsCountChanged;
        playableCollectionGroup.PlaybackStateChanged += OnPlaybackStateChanged;
        playableCollectionGroup.NameChanged += OnNameChanged;
        playableCollectionGroup.DescriptionChanged += OnDescriptionChanged;
        playableCollectionGroup.DurationChanged += OnDurationChanged;
        playableCollectionGroup.LastPlayedChanged += OnLastPlayedChanged;
        playableCollectionGroup.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        playableCollectionGroup.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        playableCollectionGroup.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        playableCollectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged += OnIsPlayPlaylistCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged += OnIsPausePlaylistCollectionAsyncAvailableChanged;
        playableCollectionGroup.PlaylistItemsCountChanged += OnPlaylistItemsCountChanged;
        playableCollectionGroup.IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
        playableCollectionGroup.TracksCountChanged += OnTracksCountChanged;
        playableCollectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
        playableCollectionGroup.AlbumItemsCountChanged += OnAlbumItemsCountChanged;
        playableCollectionGroup.IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
        playableCollectionGroup.ArtistItemsCountChanged += OnArtistItemsCountChanged;
        playableCollectionGroup.ChildrenCountChanged += OnChildrenCountChanged;
        playableCollectionGroup.ImagesChanged += OnImagesChanged;
        playableCollectionGroup.UrlsChanged += OnUrlsChanged;
        playableCollectionGroup.DownloadInfoChanged += OnDownloadInfoChanged;
        playableCollectionGroup.PlaylistItemsChanged += OnPlaylistItemsChanged;
        playableCollectionGroup.TracksChanged += OnTracksChanged;
        playableCollectionGroup.AlbumItemsChanged += OnAlbumItemsChanged;
        playableCollectionGroup.ArtistItemsChanged += OnArtistItemsChanged;
        playableCollectionGroup.ChildItemsChanged += OnChildItemsChanged;
    }

    private void DetachEvents(IPlayableCollectionGroup playableCollectionGroup)
    {
        playableCollectionGroup.SourcesChanged -= OnSourcesChanged;
        playableCollectionGroup.ImagesCountChanged -= OnImagesCountChanged;
        playableCollectionGroup.UrlsCountChanged -= OnUrlsCountChanged;
        playableCollectionGroup.PlaybackStateChanged -= OnPlaybackStateChanged;
        playableCollectionGroup.NameChanged -= OnNameChanged;
        playableCollectionGroup.DescriptionChanged -= OnDescriptionChanged;
        playableCollectionGroup.DurationChanged -= OnDurationChanged;
        playableCollectionGroup.LastPlayedChanged -= OnLastPlayedChanged;
        playableCollectionGroup.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        playableCollectionGroup.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        playableCollectionGroup.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        playableCollectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged -= OnIsPlayPlaylistCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged -= OnIsPausePlaylistCollectionAsyncAvailableChanged;
        playableCollectionGroup.PlaylistItemsCountChanged -= OnPlaylistItemsCountChanged;
        playableCollectionGroup.IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
        playableCollectionGroup.TracksCountChanged -= OnTracksCountChanged;
        playableCollectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
        playableCollectionGroup.AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
        playableCollectionGroup.IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
        playableCollectionGroup.IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
        playableCollectionGroup.ArtistItemsCountChanged -= OnArtistItemsCountChanged;
        playableCollectionGroup.ChildrenCountChanged -= OnChildrenCountChanged;
        playableCollectionGroup.ImagesChanged -= OnImagesChanged;
        playableCollectionGroup.UrlsChanged -= OnUrlsChanged;
        playableCollectionGroup.DownloadInfoChanged -= OnDownloadInfoChanged;
        playableCollectionGroup.PlaylistItemsChanged -= OnPlaylistItemsChanged;
        playableCollectionGroup.TracksChanged -= OnTracksChanged;
        playableCollectionGroup.AlbumItemsChanged -= OnAlbumItemsChanged;
        playableCollectionGroup.ArtistItemsChanged -= OnArtistItemsChanged;
        playableCollectionGroup.ChildItemsChanged -= OnChildItemsChanged;
    }

    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

    private void OnChildItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IPlayableCollectionGroup>(new PlayableCollectionGroupPluginWrapper(x.Data, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IPlayableCollectionGroup>(new PlayableCollectionGroupPluginWrapper(x.Data, _plugins), x.Index)).ToList();

        ChildItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IArtistCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IArtistCollectionItem>(Transform(x.Data), x.Index)).ToList();

        ArtistItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnAlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IAlbumCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IAlbumCollectionItem>(Transform(x.Data), x.Index)).ToList();

        AlbumItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnTracksChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<ITrack>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<ITrack>(Transform(x.Data), x.Index)).ToList();

        TracksChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnPlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IPlaylistCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IPlaylistCollectionItem>(Transform(x.Data), x.Index)).ToList();

        PlaylistItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnUrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IUrl>(new UrlPluginWrapper(x.Data, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IUrl>(new UrlPluginWrapper(x.Data, _plugins), x.Index)).ToList();

        UrlsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IImage>(new ImagePluginWrapper(x.Data, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IImage>(new ImagePluginWrapper(x.Data, _plugins), x.Index)).ToList();

        ImagesChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnDownloadInfoChanged(object sender, DownloadInfo e) => DownloadInfoChanged?.Invoke(sender, e);

    private void OnChildrenCountChanged(object sender, int e) => ChildrenCountChanged?.Invoke(sender, e);

    private void OnArtistItemsCountChanged(object sender, int e) => ArtistItemsCountChanged?.Invoke(sender, e);

    private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnAlbumItemsCountChanged(object sender, int e) => AlbumItemsCountChanged?.Invoke(sender, e);

    private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnTracksCountChanged(object sender, int e) => TracksCountChanged?.Invoke(sender, e);

    private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnPlaylistItemsCountChanged(object sender, int e) => PlaylistItemsCountChanged?.Invoke(sender, e);

    private void OnIsPausePlaylistCollectionAsyncAvailableChanged(object sender, bool e) => IsPausePlaylistCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayPlaylistCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayPlaylistCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => IsChangeDurationAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => IsChangeDescriptionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => IsChangeNameAsyncAvailableChanged?.Invoke(sender, e);

    private void OnLastPlayedChanged(object sender, DateTime? e) => LastPlayedChanged?.Invoke(sender, e);

    private void OnDurationChanged(object sender, TimeSpan e) => DurationChanged?.Invoke(sender, e);

    private void OnDescriptionChanged(object sender, string? e) => DescriptionChanged?.Invoke(sender, e);

    private void OnNameChanged(object sender, string e) => NameChanged?.Invoke(sender, e);

    private void OnPlaybackStateChanged(object sender, PlaybackState e) => PlaybackStateChanged?.Invoke(sender, e);

    private void OnUrlsCountChanged(object sender, int e) => UrlsCountChanged?.Invoke(sender, e);

    private void OnImagesCountChanged(object sender, int e) => ImagesCountChanged?.Invoke(sender, e);
    
    /// <inheritdoc cref="IMerged.SourcesChanged"/>
    public event EventHandler? SourcesChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? ImagesCountChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? UrlsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<PlaybackState>? PlaybackStateChanged;

    /// <inheritdoc/>
    public event EventHandler<string>? NameChanged;

    /// <inheritdoc/>
    public event EventHandler<string?>? DescriptionChanged;

    /// <inheritdoc/>
    public event EventHandler<TimeSpan>? DurationChanged;

    /// <inheritdoc/>
    public event EventHandler<DateTime?>? LastPlayedChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? PlaylistItemsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? AlbumItemsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? ArtistItemsCountChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? ChildrenCountChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IImage>? ImagesChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event EventHandler<DownloadInfo>? DownloadInfoChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<ITrack>? TracksChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _playableCollectionGroup.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _playableCollectionGroup.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _playableCollectionGroup.Id;

    /// <inheritdoc/>
    public string Name => _playableCollectionGroup.Name;

    /// <inheritdoc/>
    public string? Description => _playableCollectionGroup.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _playableCollectionGroup.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _playableCollectionGroup.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _playableCollectionGroup.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _playableCollectionGroup.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _playableCollectionGroup.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _playableCollectionGroup.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _playableCollectionGroup.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _playableCollectionGroup.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _playableCollectionGroup.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _playableCollectionGroup.AddedAt;

    /// <inheritdoc/>
    public int TotalPlaylistItemsCount => _playableCollectionGroup.TotalPlaylistItemsCount;

    /// <inheritdoc/>
    public bool IsPlayPlaylistCollectionAsyncAvailable => _playableCollectionGroup.IsPlayPlaylistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPausePlaylistCollectionAsyncAvailable => _playableCollectionGroup.IsPausePlaylistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayPlaylistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PausePlaylistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemovePlaylistItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddPlaylistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemovePlaylistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalTrackCount => _playableCollectionGroup.TotalTrackCount;

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => _playableCollectionGroup.IsPlayTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => _playableCollectionGroup.IsPauseTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PauseTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemoveTrackAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemoveTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalAlbumItemsCount => _playableCollectionGroup.TotalAlbumItemsCount;

    /// <inheritdoc/>
    public bool IsPlayAlbumCollectionAsyncAvailable => _playableCollectionGroup.IsPlayAlbumCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseAlbumCollectionAsyncAvailable => _playableCollectionGroup.IsPauseAlbumCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayAlbumCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PauseAlbumCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemoveAlbumItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddAlbumItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemoveAlbumItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalArtistItemsCount => _playableCollectionGroup.TotalArtistItemsCount;

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable => _playableCollectionGroup.IsPlayArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable => _playableCollectionGroup.IsPauseArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PauseArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemoveArtistItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayPlayableCollectionGroupAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => _playableCollectionGroup.PausePlayableCollectionGroupAsync(cancellationToken);

    /// <inheritdoc/>
    public int TotalChildrenCount => _playableCollectionGroup.TotalChildrenCount;

    /// <inheritdoc/>
    public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.RemoveChildAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsAddChildAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.IsRemoveChildAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => ((IMerged<ICorePlaylistCollectionItem>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => ((IMerged<ICorePlaylistCollection>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => ((IMerged<ICoreTrackCollection>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbumCollectionItem>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => ((IMerged<ICoreAlbumCollection>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => ((IMerged<ICoreArtistCollection>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => ((IMerged<ICorePlayableCollectionGroupChildren>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources => ((IMerged<ICorePlayableCollectionGroup>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _playableCollectionGroup.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _playableCollectionGroup.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollectionItem other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollection other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayPlaylistCollectionAsync(playlistItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetPlaylistItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddPlaylistItemAsync(playlistItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreTrackCollection other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayTrackCollectionAsync(track, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetTracksAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddTrackAsync(track, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollectionItem other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollection other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayAlbumCollectionAsync(albumItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetAlbumItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddAlbumItemAsync(IAlbumCollectionItem albumItem, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddAlbumItemAsync(albumItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollectionItem other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollection other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayArtistCollectionAsync(artistItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetArtistItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddArtistItemAsync(artistItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlayableCollectionGroupChildren other) => _playableCollectionGroup.Equals(other);

    /// <inheritdoc/>
    public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default) => _playableCollectionGroup.PlayPlayableCollectionGroupAsync(collectionGroup, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playableCollectionGroup.GetChildrenAsync(limit, offset, cancellationToken).Select(x => new PlayableCollectionGroupPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default) => _playableCollectionGroup.AddChildAsync(child, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlayableCollectionGroup other) => _playableCollectionGroup.Equals(other);

    private IArtistCollectionItem Transform(IArtistCollectionItem item) => item switch
    {
        IArtist artist => new ArtistPluginWrapper(artist, _plugins),
        IArtistCollection artistCollection => new ArtistCollectionPluginWrapper(artistCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IArtistCollectionItem>()
    };

    private IAlbumCollectionItem Transform(IAlbumCollectionItem item) => item switch
    {
        IAlbum album => new AlbumPluginWrapper(album, _plugins),
        IAlbumCollection albumCollection => new AlbumCollectionPluginWrapper(albumCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IAlbumCollectionItem>()
    };

    private IPlaylistCollectionItem Transform(IPlaylistCollectionItem item) => item switch
    {
        IPlaylist playlist => new PlaylistPluginWrapper(playlist, _plugins),
        IPlaylistCollection playlistCollection => new PlaylistCollectionPluginWrapper(playlistCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IPlaylistCollectionItem>()
    };

    private ITrack Transform(ITrack item) => new TrackPluginWrapper(item, _plugins);
}
