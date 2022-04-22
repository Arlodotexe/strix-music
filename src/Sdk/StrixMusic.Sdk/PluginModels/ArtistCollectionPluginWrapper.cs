// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IArtistCollection"/> with the provided plugins.
/// </summary>
public class ArtistCollectionPluginWrapper : IArtistCollection, IPluginWrapper
{
    private readonly IArtistCollection _artistCollection;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistCollectionPluginWrapper"/> class.
    /// </summary>
    /// <param name="artistCollection">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal ArtistCollectionPluginWrapper(IArtistCollection artistCollection, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _artistCollection = ActivePlugins.ArtistCollection.Execute(artistCollection);
        _plugins = plugins;

        AttachEvents(_artistCollection);
    }

    /// <inheritdoc />
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IArtistCollection artistCollection)
    {
        artistCollection.ImagesCountChanged += OnImagesCountChanged;
        artistCollection.UrlsCountChanged += OnUrlsCountChanged;
        artistCollection.PlaybackStateChanged += OnPlaybackStateChanged;
        artistCollection.NameChanged += OnNameChanged;
        artistCollection.DescriptionChanged += OnDescriptionChanged;
        artistCollection.DurationChanged += OnDurationChanged;
        artistCollection.LastPlayedChanged += OnLastPlayedChanged;
        artistCollection.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        artistCollection.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        artistCollection.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        artistCollection.IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
        artistCollection.IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
        artistCollection.ArtistItemsCountChanged += OnArtistItemsCountChanged;
        artistCollection.ImagesChanged += OnImagesChanged;
        artistCollection.UrlsChanged += OnUrlsChanged;
        artistCollection.DownloadInfoChanged += OnDownloadInfoChanged;
        artistCollection.ArtistItemsChanged += OnArtistItemsChanged;
    }

    private void DetachEvents(IArtistCollection artistCollection)
    {
        artistCollection.ImagesCountChanged -= OnImagesCountChanged;
        artistCollection.UrlsCountChanged -= OnUrlsCountChanged;
        artistCollection.PlaybackStateChanged -= OnPlaybackStateChanged;
        artistCollection.NameChanged -= OnNameChanged;
        artistCollection.DescriptionChanged -= OnDescriptionChanged;
        artistCollection.DurationChanged -= OnDurationChanged;
        artistCollection.LastPlayedChanged -= OnLastPlayedChanged;
        artistCollection.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        artistCollection.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        artistCollection.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        artistCollection.IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
        artistCollection.IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
        artistCollection.ArtistItemsCountChanged -= OnArtistItemsCountChanged;
        artistCollection.ImagesChanged -= OnImagesChanged;
        artistCollection.UrlsChanged -= OnUrlsChanged;
        artistCollection.DownloadInfoChanged -= OnDownloadInfoChanged;
        artistCollection.ArtistItemsChanged -= OnArtistItemsChanged;
    }

    private void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IArtistCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IArtistCollectionItem>(Transform(x.Data), x.Index)).ToList();

        ArtistItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
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

    private void OnArtistItemsCountChanged(object sender, int e) => ArtistItemsCountChanged?.Invoke(sender, e);

    private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(sender, e);

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
    public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? ArtistItemsCountChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IImage>? ImagesChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

    /// <inheritdoc/>
    public event EventHandler<DownloadInfo>? DownloadInfoChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _artistCollection.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _artistCollection.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _artistCollection.Id;

    /// <inheritdoc/>
    public string Name => _artistCollection.Name;

    /// <inheritdoc/>
    public string? Description => _artistCollection.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _artistCollection.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _artistCollection.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _artistCollection.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _artistCollection.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _artistCollection.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _artistCollection.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _artistCollection.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _artistCollection.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _artistCollection.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _artistCollection.AddedAt;

    /// <inheritdoc/>
    public int TotalArtistItemsCount => _artistCollection.TotalArtistItemsCount;

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable => _artistCollection.IsPlayArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable => _artistCollection.IsPauseArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _artistCollection.PlayArtistCollectionAsync(cancellationToken);

    /// <inheritdoc />
    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _artistCollection.PauseArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.RemoveArtistItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.IsAddArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollection.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection other) => _artistCollection.Equals(other);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_artistCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_artistCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)_artistCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => ((IMerged<ICoreArtistCollection>)_artistCollection).Sources;

    /// <inheritdoc cref="IMerged{T}.SourceCores"/>
    public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreArtistCollection>)_artistCollection).SourceCores;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var results = await _artistCollection.GetImagesAsync(limit, offset, cancellationToken);
        return results.Select(x => ActivePlugins.Image.Execute(x)).ToList();
    }

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _artistCollection.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection other) => _artistCollection.Equals(other);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var results = await _artistCollection.GetUrlsAsync(limit, offset, cancellationToken);
        return results.Select(x => ActivePlugins.Url.Execute(x)).ToList();
    }

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _artistCollection.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _artistCollection.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _artistCollection.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollectionItem other) => _artistCollection.Equals(other);

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _artistCollection.PlayArtistCollectionAsync(artistItem, cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var results = await _artistCollection.GetArtistItemsAsync(limit, offset, cancellationToken);
        return results.Select(Transform).ToList();
    }

    /// <inheritdoc/>
    public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _artistCollection.AddArtistItemAsync(artistItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollection other) => _artistCollection.Equals(other);

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DetachEvents(_artistCollection);
        return _artistCollection.DisposeAsync();
    }

    private IArtistCollectionItem Transform(IArtistCollectionItem item) => item switch
    {
        IArtist artist => new ArtistPluginWrapper(artist, _plugins),
        IArtistCollection artistCollection => new ArtistCollectionPluginWrapper(artistCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IArtistCollectionItem>()
    };
}
