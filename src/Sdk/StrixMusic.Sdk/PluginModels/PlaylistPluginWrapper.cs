// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IPlaylist"/> with the provided plugins.
/// </summary>
public class PlaylistPluginWrapper : IPlaylist, IPluginWrapper
{
    private readonly IPlaylist _playlist;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistPluginWrapper"/> class.
    /// </summary>
    /// <param name="playlist">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal PlaylistPluginWrapper(IPlaylist playlist, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _playlist = ActivePlugins.Playlist.Execute(playlist);
        _plugins = plugins;

        AttachEvents(_playlist);

        if (_playlist.RelatedItems != null)
            RelatedItems = new PlayableCollectionGroupPluginWrapper(_playlist.RelatedItems, plugins);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IPlaylist playlist)
    {
        playlist.SourcesChanged += OnSourcesChanged;
        playlist.ImagesCountChanged += OnImagesCountChanged;
        playlist.UrlsCountChanged += OnUrlsCountChanged;
        playlist.PlaybackStateChanged += OnPlaybackStateChanged;
        playlist.NameChanged += OnNameChanged;
        playlist.DescriptionChanged += OnDescriptionChanged;
        playlist.DurationChanged += OnDurationChanged;
        playlist.LastPlayedChanged += OnLastPlayedChanged;
        playlist.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        playlist.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        playlist.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        playlist.IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
        playlist.IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
        playlist.TracksCountChanged += OnTracksCountChanged;
        playlist.ImagesChanged += OnImagesChanged;
        playlist.UrlsChanged += OnUrlsChanged;
        playlist.DownloadInfoChanged += OnDownloadInfoChanged;
        playlist.TracksChanged += OnTracksChanged;
    }

    private void DetachEvents(IPlaylist playlist)
    {
        playlist.SourcesChanged -= OnSourcesChanged;
        playlist.ImagesCountChanged -= OnImagesCountChanged;
        playlist.UrlsCountChanged -= OnUrlsCountChanged;
        playlist.PlaybackStateChanged -= OnPlaybackStateChanged;
        playlist.NameChanged -= OnNameChanged;
        playlist.DescriptionChanged -= OnDescriptionChanged;
        playlist.DurationChanged -= OnDurationChanged;
        playlist.LastPlayedChanged -= OnLastPlayedChanged;
        playlist.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        playlist.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        playlist.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        playlist.IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
        playlist.IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
        playlist.TracksCountChanged -= OnTracksCountChanged;
        playlist.ImagesChanged -= OnImagesChanged;
        playlist.UrlsChanged -= OnUrlsChanged;
        playlist.DownloadInfoChanged -= OnDownloadInfoChanged;
        playlist.TracksChanged -= OnTracksChanged;
    }

    private void OnSourcesChanged(object? sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

    private void OnTracksChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<ITrack>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<ITrack>(Transform(x.Data), x.Index)).ToList();

        TracksChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
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

    private void OnDownloadInfoChanged(object? sender, DownloadInfo e) => DownloadInfoChanged?.Invoke(sender, e);

    private void OnTracksCountChanged(object? sender, int e) => TracksCountChanged?.Invoke(sender, e);

    private void OnIsPauseTrackCollectionAsyncAvailableChanged(object? sender, bool e) => IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayTrackCollectionAsyncAvailableChanged(object? sender, bool e) => IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsChangeDurationAsyncAvailableChanged(object? sender, bool e) => IsChangeDurationAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsChangeDescriptionAsyncAvailableChanged(object? sender, bool e) => IsChangeDescriptionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsChangeNameAsyncAvailableChanged(object? sender, bool e) => IsChangeNameAsyncAvailableChanged?.Invoke(sender, e);

    private void OnLastPlayedChanged(object? sender, DateTime? e) => LastPlayedChanged?.Invoke(sender, e);

    private void OnDurationChanged(object? sender, TimeSpan e) => DurationChanged?.Invoke(sender, e);

    private void OnDescriptionChanged(object? sender, string? e) => DescriptionChanged?.Invoke(sender, e);

    private void OnNameChanged(object? sender, string e) => NameChanged?.Invoke(sender, e);

    private void OnPlaybackStateChanged(object? sender, PlaybackState e) => PlaybackStateChanged?.Invoke(sender, e);

    private void OnUrlsCountChanged(object? sender, int e) => UrlsCountChanged?.Invoke(sender, e);

    private void OnImagesCountChanged(object? sender, int e) => ImagesCountChanged?.Invoke(sender, e);
    
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
    public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? TracksCountChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IImage>? ImagesChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event EventHandler<DownloadInfo>? DownloadInfoChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<ITrack>? TracksChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _playlist.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _playlist.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _playlist.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _playlist.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _playlist.Id;

    /// <inheritdoc/>
    public string Name => _playlist.Name;

    /// <inheritdoc/>
    public string? Description => _playlist.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _playlist.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _playlist.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _playlist.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _playlist.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _playlist.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _playlist.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _playlist.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _playlist.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _playlist.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _playlist.AddedAt;

    /// <inheritdoc/>
    public int TotalTrackCount => _playlist.TotalTrackCount;

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => _playlist.IsPlayTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => _playlist.IsPauseTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _playlist.PlayTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _playlist.PauseTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _playlist.RemoveTrackAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsAddTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsRemoveTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection? other) => _playlist.Equals(other!);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_playlist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_playlist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => ((IMerged<ICorePlaylistCollectionItem>)_playlist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => ((IMerged<ICoreTrackCollection>)_playlist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylist> IMerged<ICorePlaylist>.Sources => ((IMerged<ICorePlaylist>)_playlist).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlist.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _playlist.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection? other) => _playlist.Equals(other!);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlist.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _playlist.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _playlist.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _playlist.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollectionItem? other) => _playlist.Equals(other!);

    /// <inheritdoc/>
    public bool Equals(ICoreTrackCollection? other) => _playlist.Equals(other!);

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _playlist.PlayTrackCollectionAsync(track, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlist.GetTracksAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _playlist.AddTrackAsync(track, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlaylist? other) => _playlist.Equals(other!);

    private ITrack Transform(ITrack item) => new TrackPluginWrapper(item, _plugins);
    
    /// <inheritdoc/>
    public IUserProfile? Owner => _playlist.Owner;

    /// <inheritdoc/>
    public IPlayableCollectionGroup? RelatedItems { get; }
}
