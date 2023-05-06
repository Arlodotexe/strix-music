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
/// Wraps an instance of <see cref="IAlbumCollection"/> with the provided plugins.
/// </summary>
public class AlbumCollectionPluginWrapper : IAlbumCollection, IPluginWrapper
{
    private readonly IAlbumCollection _albumCollection;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumCollectionPluginWrapper"/> class.
    /// </summary>
    /// <param name="albumCollection">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal AlbumCollectionPluginWrapper(IAlbumCollection albumCollection, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _albumCollection = ActivePlugins.AlbumCollection.Execute(albumCollection);
        _plugins = plugins;

        AttachEvents(_albumCollection);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IAlbumCollection albumCollection)
    {
        albumCollection.SourcesChanged += OnSourcesChanged;
        albumCollection.ImagesCountChanged += OnImagesCountChanged;
        albumCollection.UrlsCountChanged += OnUrlsCountChanged;
        albumCollection.PlaybackStateChanged += OnPlaybackStateChanged;
        albumCollection.NameChanged += OnNameChanged;
        albumCollection.DescriptionChanged += OnDescriptionChanged;
        albumCollection.DurationChanged += OnDurationChanged;
        albumCollection.LastPlayedChanged += OnLastPlayedChanged;
        albumCollection.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        albumCollection.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        albumCollection.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        albumCollection.IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
        albumCollection.IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
        albumCollection.AlbumItemsCountChanged += OnAlbumItemsCountChanged;
        albumCollection.ImagesChanged += OnImagesChanged;
        albumCollection.UrlsChanged += OnUrlsChanged;
        albumCollection.DownloadInfoChanged += OnDownloadInfoChanged;
        albumCollection.AlbumItemsChanged += OnAlbumItemsChanged;
    }

    private void DetachEvents(IAlbumCollection albumCollection)
    {
        albumCollection.SourcesChanged -= OnSourcesChanged;
        albumCollection.ImagesCountChanged -= OnImagesCountChanged;
        albumCollection.UrlsCountChanged -= OnUrlsCountChanged;
        albumCollection.PlaybackStateChanged -= OnPlaybackStateChanged;
        albumCollection.NameChanged -= OnNameChanged;
        albumCollection.DescriptionChanged -= OnDescriptionChanged;
        albumCollection.DurationChanged -= OnDurationChanged;
        albumCollection.LastPlayedChanged -= OnLastPlayedChanged;
        albumCollection.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        albumCollection.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        albumCollection.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        albumCollection.IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
        albumCollection.IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
        albumCollection.AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
        albumCollection.ImagesChanged -= OnImagesChanged;
        albumCollection.UrlsChanged -= OnUrlsChanged;
        albumCollection.DownloadInfoChanged -= OnDownloadInfoChanged;
        albumCollection.AlbumItemsChanged -= OnAlbumItemsChanged;
    }

    private void OnSourcesChanged(object? sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

    private void OnAlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IAlbumCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IAlbumCollectionItem>(Transform(x.Data), x.Index)).ToList();

        AlbumItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
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

    private void OnAlbumItemsCountChanged(object? sender, int e) => AlbumItemsCountChanged?.Invoke(sender, e);

    private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object? sender, bool e) => IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object? sender, bool e) => IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(sender, e);

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
    public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? AlbumItemsCountChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IImage>? ImagesChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event EventHandler<DownloadInfo>? DownloadInfoChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _albumCollection.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _albumCollection.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _albumCollection.Id;

    /// <inheritdoc/>
    public string Name => _albumCollection.Name;

    /// <inheritdoc/>
    public string? Description => _albumCollection.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _albumCollection.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _albumCollection.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _albumCollection.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _albumCollection.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _albumCollection.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _albumCollection.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _albumCollection.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _albumCollection.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _albumCollection.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _albumCollection.AddedAt;

    /// <inheritdoc/>
    public int TotalAlbumItemsCount => _albumCollection.TotalAlbumItemsCount;

    /// <inheritdoc/>
    public bool IsPlayAlbumCollectionAsyncAvailable => _albumCollection.IsPlayAlbumCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseAlbumCollectionAsyncAvailable => _albumCollection.IsPauseAlbumCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.RemoveAlbumItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.IsAddAlbumItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollection.IsRemoveAlbumItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => _albumCollection.PlayAlbumCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => _albumCollection.PauseAlbumCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection? other) => _albumCollection.Equals(other!);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_albumCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_albumCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbumCollectionItem>)_albumCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => ((IMerged<ICoreAlbumCollection>)_albumCollection).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _albumCollection.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _albumCollection.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection? other) => _albumCollection.Equals(other!);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _albumCollection.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _albumCollection.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _albumCollection.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _albumCollection.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollectionItem? other) => _albumCollection.Equals(other!);

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => _albumCollection.PlayAlbumCollectionAsync(albumItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        return _albumCollection.GetAlbumItemsAsync(limit, offset, cancellationToken).Select(Transform);
    }

    /// <inheritdoc/>
    public Task AddAlbumItemAsync(IAlbumCollectionItem albumItem, int index, CancellationToken cancellationToken = default) => _albumCollection.AddAlbumItemAsync(albumItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollection? other) => _albumCollection.Equals(other!);

    private IAlbumCollectionItem Transform(IAlbumCollectionItem item) => item switch
    {
        IAlbum album => new AlbumPluginWrapper(album, _plugins),
        IAlbumCollection albumCollection => new AlbumCollectionPluginWrapper(albumCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IAlbumCollectionItem>()
    };
}
