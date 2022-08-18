// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="ITrackCollection"/> with the provided plugins.
/// </summary>
public class TrackCollectionPluginWrapper : ITrackCollection, IPluginWrapper
{
    private readonly ITrackCollection _trackCollection;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackCollectionPluginWrapper"/> class.
    /// </summary>
    /// <param name="trackCollection">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal TrackCollectionPluginWrapper(ITrackCollection trackCollection, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _trackCollection = ActivePlugins.TrackCollection.Execute(trackCollection);
        _plugins = plugins;

        AttachEvents(_trackCollection);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(ITrackCollection trackCollection)
    {
        trackCollection.SourcesChanged += OnSourcesChanged;
        trackCollection.ImagesCountChanged += OnImagesCountChanged;
        trackCollection.UrlsCountChanged += OnUrlsCountChanged;
        trackCollection.PlaybackStateChanged += OnPlaybackStateChanged;
        trackCollection.NameChanged += OnNameChanged;
        trackCollection.DescriptionChanged += OnDescriptionChanged;
        trackCollection.DurationChanged += OnDurationChanged;
        trackCollection.LastPlayedChanged += OnLastPlayedChanged;
        trackCollection.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        trackCollection.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        trackCollection.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        trackCollection.IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
        trackCollection.IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
        trackCollection.TracksCountChanged += OnTracksCountChanged;
        trackCollection.ImagesChanged += OnImagesChanged;
        trackCollection.UrlsChanged += OnUrlsChanged;
        trackCollection.DownloadInfoChanged += OnDownloadInfoChanged;
        trackCollection.TracksChanged += OnTracksChanged;
    }

    private void DetachEvents(ITrackCollection trackCollection)
    {
        trackCollection.SourcesChanged -= OnSourcesChanged;
        trackCollection.ImagesCountChanged -= OnImagesCountChanged;
        trackCollection.UrlsCountChanged -= OnUrlsCountChanged;
        trackCollection.PlaybackStateChanged -= OnPlaybackStateChanged;
        trackCollection.NameChanged -= OnNameChanged;
        trackCollection.DescriptionChanged -= OnDescriptionChanged;
        trackCollection.DurationChanged -= OnDurationChanged;
        trackCollection.LastPlayedChanged -= OnLastPlayedChanged;
        trackCollection.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        trackCollection.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        trackCollection.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        trackCollection.IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
        trackCollection.IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
        trackCollection.TracksCountChanged -= OnTracksCountChanged;
        trackCollection.ImagesChanged -= OnImagesChanged;
        trackCollection.UrlsChanged -= OnUrlsChanged;
        trackCollection.DownloadInfoChanged -= OnDownloadInfoChanged;
        trackCollection.TracksChanged -= OnTracksChanged;
    }
    
    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

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

    private void OnDownloadInfoChanged(object sender, DownloadInfo e) => DownloadInfoChanged?.Invoke(sender, e);

    private void OnTracksCountChanged(object sender, int e) => TracksCountChanged?.Invoke(sender, e);

    private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(sender, e);

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
    
    /// <inheritdoc cref="IMerged.SourcesChanged"/>
    public event EventHandler? SourcesChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _trackCollection.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _trackCollection.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _trackCollection.Id;

    /// <inheritdoc/>
    public string Name => _trackCollection.Name;

    /// <inheritdoc/>
    public string? Description => _trackCollection.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _trackCollection.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _trackCollection.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _trackCollection.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _trackCollection.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _trackCollection.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _trackCollection.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _trackCollection.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _trackCollection.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _trackCollection.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _trackCollection.AddedAt;

    /// <inheritdoc/>
    public int TotalTrackCount => _trackCollection.TotalTrackCount;

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => _trackCollection.IsPlayTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => _trackCollection.IsPauseTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.RemoveTrackAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.IsAddTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollection.IsRemoveTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _trackCollection.PlayTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _trackCollection.PauseTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection other) => _trackCollection.Equals(other);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_trackCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_trackCollection).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => ((IMerged<ICoreTrackCollection>)_trackCollection).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _trackCollection.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _trackCollection.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection other) => _trackCollection.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _trackCollection.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _trackCollection.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _trackCollection.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _trackCollection.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(ITrack trackItem, CancellationToken cancellationToken = default) => _trackCollection.PlayTrackCollectionAsync(trackItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _trackCollection.GetTracksAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddTrackAsync(ITrack trackItem, int index, CancellationToken cancellationToken = default) => _trackCollection.AddTrackAsync(trackItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreTrackCollection other) => _trackCollection.Equals(other);

    private ITrack Transform(ITrack track) => new TrackPluginWrapper(track, _plugins);
}
