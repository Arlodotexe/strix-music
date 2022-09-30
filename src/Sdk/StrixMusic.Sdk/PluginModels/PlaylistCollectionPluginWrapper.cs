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
/// Wraps an instance of <see cref="IPlaylistCollection"/> with the provided plugins.
/// </summary>
public class PlaylistCollectionPluginWrapper : IPlaylistCollection, IPluginWrapper
{
    private readonly IPlaylistCollection _playlistCollection;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistCollectionPluginWrapper"/> class.
    /// </summary>
    /// <param name="playlistCollection">The instance to wrap around and apply plugins to.</param>
    /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal PlaylistCollectionPluginWrapper(IPlaylistCollection playlistCollection, IStrixDataRoot pluginRoot, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(pluginRoot, ActivePlugins);
        
        _playlistCollection = ActivePlugins.PlaylistCollection.Execute(playlistCollection);
        Root = pluginRoot;
        _plugins = plugins;

        AttachEvents(_playlistCollection);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IPlaylistCollection playlistCollection)
    {
        playlistCollection.SourcesChanged += OnSourcesChanged;
        playlistCollection.ImagesCountChanged += OnImagesCountChanged;
        playlistCollection.UrlsCountChanged += OnUrlsCountChanged;
        playlistCollection.PlaybackStateChanged += OnPlaybackStateChanged;
        playlistCollection.NameChanged += OnNameChanged;
        playlistCollection.DescriptionChanged += OnDescriptionChanged;
        playlistCollection.DurationChanged += OnDurationChanged;
        playlistCollection.LastPlayedChanged += OnLastPlayedChanged;
        playlistCollection.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        playlistCollection.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        playlistCollection.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        playlistCollection.IsPlayPlaylistCollectionAsyncAvailableChanged += OnIsPlayPlaylistCollectionAsyncAvailableChanged;
        playlistCollection.IsPausePlaylistCollectionAsyncAvailableChanged += OnIsPausePlaylistCollectionAsyncAvailableChanged;
        playlistCollection.PlaylistItemsCountChanged += OnPlaylistItemsCountChanged;
        playlistCollection.ImagesChanged += OnImagesChanged;
        playlistCollection.UrlsChanged += OnUrlsChanged;
        playlistCollection.DownloadInfoChanged += OnDownloadInfoChanged;
        playlistCollection.PlaylistItemsChanged += OnPlaylistItemsChanged;
    }

    private void DetachEvents(IPlaylistCollection playlistCollection)
    {
        playlistCollection.SourcesChanged -= OnSourcesChanged;
        playlistCollection.ImagesCountChanged -= OnImagesCountChanged;
        playlistCollection.UrlsCountChanged -= OnUrlsCountChanged;
        playlistCollection.PlaybackStateChanged -= OnPlaybackStateChanged;
        playlistCollection.NameChanged -= OnNameChanged;
        playlistCollection.DescriptionChanged -= OnDescriptionChanged;
        playlistCollection.DurationChanged -= OnDurationChanged;
        playlistCollection.LastPlayedChanged -= OnLastPlayedChanged;
        playlistCollection.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        playlistCollection.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        playlistCollection.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        playlistCollection.IsPlayPlaylistCollectionAsyncAvailableChanged -= OnIsPlayPlaylistCollectionAsyncAvailableChanged;
        playlistCollection.IsPausePlaylistCollectionAsyncAvailableChanged -= OnIsPausePlaylistCollectionAsyncAvailableChanged;
        playlistCollection.PlaylistItemsCountChanged -= OnPlaylistItemsCountChanged;
        playlistCollection.ImagesChanged -= OnImagesChanged;
        playlistCollection.UrlsChanged -= OnUrlsChanged;
        playlistCollection.DownloadInfoChanged -= OnDownloadInfoChanged;
        playlistCollection.PlaylistItemsChanged -= OnPlaylistItemsChanged;
    }

    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

    private void OnPlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IPlaylistCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IPlaylistCollectionItem>(Transform(x.Data), x.Index)).ToList();

        PlaylistItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnUrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IUrl>(new UrlPluginWrapper(x.Data, Root, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IUrl>(new UrlPluginWrapper(x.Data, Root, _plugins), x.Index)).ToList();

        UrlsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IImage>(new ImagePluginWrapper(x.Data, Root, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IImage>(new ImagePluginWrapper(x.Data, Root, _plugins), x.Index)).ToList();

        ImagesChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnDownloadInfoChanged(object sender, DownloadInfo e) => DownloadInfoChanged?.Invoke(sender, e);

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
    public event CollectionChangedEventHandler<IImage>? ImagesChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event EventHandler<DownloadInfo>? DownloadInfoChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _playlistCollection.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _playlistCollection.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _playlistCollection.Id;

    /// <inheritdoc/>
    public string Name => _playlistCollection.Name;

    /// <inheritdoc/>
    public string? Description => _playlistCollection.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _playlistCollection.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _playlistCollection.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _playlistCollection.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _playlistCollection.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _playlistCollection.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _playlistCollection.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _playlistCollection.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _playlistCollection.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _playlistCollection.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _playlistCollection.AddedAt;

    /// <inheritdoc/>
    public int TotalPlaylistItemsCount => _playlistCollection.TotalPlaylistItemsCount;

    /// <inheritdoc/>
    public bool IsPlayPlaylistCollectionAsyncAvailable => _playlistCollection.IsPlayPlaylistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPausePlaylistCollectionAsyncAvailable => _playlistCollection.IsPausePlaylistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.RemovePlaylistItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.IsAddPlaylistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollection.IsRemovePlaylistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => _playlistCollection.PlayPlaylistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => _playlistCollection.PausePlaylistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection other) => _playlistCollection.Equals(other);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_playlistCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_playlistCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => ((IMerged<ICorePlaylistCollectionItem>)_playlistCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => ((IMerged<ICorePlaylistCollection>)_playlistCollection).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlistCollection.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, Root, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _playlistCollection.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection other) => _playlistCollection.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlistCollection.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, Root, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _playlistCollection.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _playlistCollection.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _playlistCollection.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollectionItem other) => _playlistCollection.Equals(other);

    /// <inheritdoc/>
    public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => _playlistCollection.PlayPlaylistCollectionAsync(playlistItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlistCollection.GetPlaylistItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => _playlistCollection.AddPlaylistItemAsync(playlistItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICorePlaylistCollection other) => _playlistCollection.Equals(other);

    private IPlaylistCollectionItem Transform(IPlaylistCollectionItem item) => item switch
    {
        IPlaylist playlist => new PlaylistPluginWrapper(playlist, Root, _plugins),
        IPlaylistCollection playlistCollection => new PlaylistCollectionPluginWrapper(playlistCollection, Root, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IPlaylistCollectionItem>()
    };

    /// <inheritdoc />
    public IStrixDataRoot Root { get; }
}
