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
/// Wraps an instance of <see cref="IArtist"/> with the provided plugins.
/// </summary>
public class ArtistPluginWrapper : IArtist, IPluginWrapper
{
    private readonly SdkModelPlugin[] _plugins;
    private readonly IArtist _artist;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtistPluginWrapper"/> class.
    /// </summary>
    /// <param name="artist">An existing instance to wrap around and provide plugins on top of.</param>
    /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
    internal ArtistPluginWrapper(IArtist artist, params SdkModelPlugin[] plugins)
    {
        foreach (var plugin in plugins)
            ActivePlugins.Import(plugin);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _plugins = plugins;
        _artist = ActivePlugins.Artist.Execute(artist);

        AttachEvents(_artist);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IArtist artist)
    {
        artist.SourcesChanged += OnSourcesChanged;
        artist.ImagesCountChanged += OnImagesCountChanged;
        artist.UrlsCountChanged += OnUrlsCountChanged;
        artist.PlaybackStateChanged += OnPlaybackStateChanged;
        artist.NameChanged += OnNameChanged;
        artist.DescriptionChanged += OnDescriptionChanged;
        artist.DurationChanged += OnDurationChanged;
        artist.LastPlayedChanged += OnLastPlayedChanged;
        artist.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        artist.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        artist.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        artist.IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
        artist.IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
        artist.TracksCountChanged += OnTracksCountChanged;
        artist.IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
        artist.IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
        artist.AlbumItemsCountChanged += OnAlbumItemsCountChanged;
        artist.ImagesChanged += OnImagesChanged;
        artist.UrlsChanged += OnUrlsChanged;
        artist.DownloadInfoChanged += OnDownloadInfoChanged;
        artist.TracksChanged += OnTracksChanged;
        artist.AlbumItemsChanged += OnAlbumItemsChanged;
        artist.GenresChanged += OnGenresChanged;
        artist.GenresCountChanged += OnGenresCountChanged;
    }

    private void DetachEvents(IArtist artist)
    {
        artist.SourcesChanged -= OnSourcesChanged;
        artist.ImagesCountChanged -= OnImagesCountChanged;
        artist.UrlsCountChanged -= OnUrlsCountChanged;
        artist.PlaybackStateChanged -= OnPlaybackStateChanged;
        artist.NameChanged -= OnNameChanged;
        artist.DescriptionChanged -= OnDescriptionChanged;
        artist.DurationChanged -= OnDurationChanged;
        artist.LastPlayedChanged -= OnLastPlayedChanged;
        artist.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        artist.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        artist.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        artist.IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
        artist.IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
        artist.TracksCountChanged -= OnTracksCountChanged;
        artist.IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
        artist.IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
        artist.AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
        artist.ImagesChanged -= OnImagesChanged;
        artist.UrlsChanged -= OnUrlsChanged;
        artist.DownloadInfoChanged -= OnDownloadInfoChanged;
        artist.TracksChanged -= OnTracksChanged;
        artist.AlbumItemsChanged -= OnAlbumItemsChanged;
        artist.GenresChanged -= OnGenresChanged;
        artist.GenresCountChanged -= OnGenresCountChanged;
    }

    private void OnSourcesChanged(object? sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

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

    private void OnGenresCountChanged(object? sender, int e) => GenresCountChanged?.Invoke(sender, e);

    private void OnGenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems) 
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IGenre>(new GenrePluginWrapper(x.Data, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IGenre>(new GenrePluginWrapper(x.Data, _plugins), x.Index)).ToList();

        GenresChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }
    
    private void OnDownloadInfoChanged(object? sender, DownloadInfo e) => DownloadInfoChanged?.Invoke(sender, e);

    private void OnAlbumItemsCountChanged(object? sender, int e) => AlbumItemsCountChanged?.Invoke(sender, e);

    private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object? sender, bool e) => IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object? sender, bool e) => IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(sender, e);

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
    public event CollectionChangedEventHandler<ITrack>? TracksChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IGenre>? GenresChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? GenresCountChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _artist.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _artist.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _artist.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _artist.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _artist.Id;

    /// <inheritdoc/>
    public string Name => _artist.Name;

    /// <inheritdoc/>
    public string? Description => _artist.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _artist.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _artist.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _artist.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _artist.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _artist.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _artist.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _artist.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _artist.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _artist.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _artist.AddedAt;

    /// <inheritdoc/>
    public int TotalTrackCount => _artist.TotalTrackCount;

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => _artist.IsPlayTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => _artist.IsPauseTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _artist.PlayTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _artist.PauseTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _artist.RemoveTrackAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsAddTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsRemoveTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalAlbumItemsCount => _artist.TotalAlbumItemsCount;

    /// <inheritdoc/>
    public bool IsPlayAlbumCollectionAsyncAvailable => _artist.IsPlayAlbumCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseAlbumCollectionAsyncAvailable => _artist.IsPauseAlbumCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => _artist.PlayAlbumCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => _artist.PauseAlbumCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => _artist.RemoveAlbumItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsAddAlbumItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsRemoveAlbumItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => ((IMerged<ICoreTrackCollection>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbumCollectionItem>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => ((IMerged<ICoreAlbumCollection>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => ((IMerged<ICoreGenreCollection>)_artist).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtist> IMerged<ICoreArtist>.Sources => ((IMerged<ICoreArtist>)_artist).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artist.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _artist.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artist.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _artist.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _artist.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _artist.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreTrackCollection? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _artist.PlayTrackCollectionAsync(track, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artist.GetTracksAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _artist.AddTrackAsync(track, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollectionItem? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollection? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => _artist.PlayAlbumCollectionAsync(albumItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artist.GetAlbumItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddAlbumItemAsync(IAlbumCollectionItem albumItem, int index, CancellationToken cancellationToken = default) => _artist.AddAlbumItemAsync(albumItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollectionItem? other) => _artist.Equals(other!);

    private IAlbumCollectionItem Transform(IAlbumCollectionItem item) => item switch
    {
        IAlbum album => new AlbumPluginWrapper(album, _plugins),
        IAlbumCollection albumCollection => new AlbumCollectionPluginWrapper(albumCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IAlbumCollectionItem>()
    };

    private ITrack Transform(ITrack item) => new TrackPluginWrapper(item, _plugins);

    /// <inheritdoc/>
    public int TotalGenreCount => _artist.TotalGenreCount;

    /// <inheritdoc/>
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _artist.RemoveGenreAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsAddGenreAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _artist.IsRemoveGenreAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreGenreCollection? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artist.GetGenresAsync(limit, offset, cancellationToken).Select(x => new GenrePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _artist.AddGenreAsync(genre, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreArtist? other) => _artist.Equals(other!);

    /// <inheritdoc/>
    public IPlayableCollectionGroup? RelatedItems => _artist.RelatedItems is not null ? new PlayableCollectionGroupPluginWrapper(_artist.RelatedItems, _plugins) : null;
}
