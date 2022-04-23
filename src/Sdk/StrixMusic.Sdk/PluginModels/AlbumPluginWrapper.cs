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
/// Wraps an instance of <see cref="IAlbum"/> with the provided plugins.
/// </summary>
public class AlbumPluginWrapper : IAlbum, IPluginWrapper
{
    private readonly IAlbum _album;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumPluginWrapper"/> class.
    /// </summary>
    /// <param name="album">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal AlbumPluginWrapper(IAlbum album, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _album = ActivePlugins.Album.Execute(album);
        _plugins = plugins;

        if (_album.RelatedItems is not null)
            RelatedItems = new PlayableCollectionGroupPluginWrapper(_album.RelatedItems, plugins);

        AttachEvents(_album);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IAlbum album)
    {
        album.ImagesCountChanged += OnImagesCountChanged;
        album.UrlsCountChanged += OnUrlsCountChanged;
        album.PlaybackStateChanged += OnPlaybackStateChanged;
        album.NameChanged += OnNameChanged;
        album.DescriptionChanged += OnDescriptionChanged;
        album.DurationChanged += OnDurationChanged;
        album.LastPlayedChanged += OnLastPlayedChanged;
        album.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        album.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        album.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        album.IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
        album.IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
        album.TracksCountChanged += OnTracksCountChanged;
        album.IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
        album.IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
        album.ArtistItemsCountChanged += OnArtistItemsCountChanged;
        album.ImagesChanged += OnImagesChanged;
        album.UrlsChanged += OnUrlsChanged;
        album.DownloadInfoChanged += OnDownloadInfoChanged;
        album.TracksChanged += OnTracksChanged;
        album.ArtistItemsChanged += OnArtistItemsChanged;
        album.GenresChanged += OnGenresChanged;
        album.GenresCountChanged += OnGenresCountChanged;
    }

    private void DetachEvents(IAlbum album)
    {
        album.ImagesCountChanged -= OnImagesCountChanged;
        album.UrlsCountChanged -= OnUrlsCountChanged;
        album.PlaybackStateChanged -= OnPlaybackStateChanged;
        album.NameChanged -= OnNameChanged;
        album.DescriptionChanged -= OnDescriptionChanged;
        album.DurationChanged -= OnDurationChanged;
        album.LastPlayedChanged -= OnLastPlayedChanged;
        album.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        album.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        album.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        album.IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
        album.IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
        album.TracksCountChanged -= OnTracksCountChanged;
        album.IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
        album.IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
        album.ArtistItemsCountChanged -= OnArtistItemsCountChanged;
        album.ImagesChanged -= OnImagesChanged;
        album.UrlsChanged -= OnUrlsChanged;
        album.DownloadInfoChanged -= OnDownloadInfoChanged;
        album.TracksChanged -= OnTracksChanged;
        album.ArtistItemsChanged -= OnArtistItemsChanged;
        album.GenresChanged += OnGenresChanged;
        album.GenresCountChanged += OnGenresCountChanged;
    }

    private void OnGenresCountChanged(object sender, int e) => GenresCountChanged?.Invoke(sender, e);

    private void OnGenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IGenre>(new GenrePluginWrapper(x.Data, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IGenre>(new GenrePluginWrapper(x.Data, _plugins), x.Index)).ToList();

        GenresChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }

    private void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IArtistCollectionItem>(Transform(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IArtistCollectionItem>(Transform(x.Data), x.Index)).ToList();

        ArtistItemsChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
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

    private void OnDownloadInfoChanged(object sender, DownloadInfo e) => DownloadInfoChanged?.Invoke(sender, e);

    private void OnArtistItemsCountChanged(object sender, int e) => ArtistItemsCountChanged?.Invoke(sender, e);

    private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(sender, e);

    private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(sender, e);

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
    public event CollectionChangedEventHandler<ITrack>? TracksChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

    /// <inheritdoc/>
    public event EventHandler<DateTime?>? DatePublishedChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IGenre>? GenresChanged;

    /// <inheritdoc/>
    public event EventHandler<int>? GenresCountChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _album.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _album.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _album.Id;

    /// <inheritdoc/>
    public string Name => _album.Name;

    /// <inheritdoc/>
    public string? Description => _album.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _album.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _album.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _album.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _album.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _album.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _album.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _album.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _album.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _album.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _album.AddedAt;

    /// <inheritdoc/>
    public int TotalTrackCount => _album.TotalTrackCount;

    /// <inheritdoc/>
    public bool IsPlayTrackCollectionAsyncAvailable => _album.IsPlayTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseTrackCollectionAsyncAvailable => _album.IsPauseTrackCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _album.PlayTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _album.PauseTrackCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveTrackAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveTrackAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalArtistItemsCount => _album.TotalArtistItemsCount;

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable => _album.IsPlayArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable => _album.IsPauseArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _album.PlayArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _album.PauseArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveArtistItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection other) => _album.Equals(other);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => ((IMerged<ICoreTrackCollection>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => ((IMerged<ICoreAlbumCollectionItem>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => ((IMerged<ICoreArtistCollection>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => ((IMerged<ICoreGenreCollection>)_album).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreAlbum> IMerged<ICoreAlbum>.Sources => ((IMerged<ICoreAlbum>)_album).Sources;

    /// <inheritdoc cref="IMerged{T}.SourceCores"/>
    public IReadOnlyList<ICore> SourceCores => ((IMerged<ICoreAlbum>)_album).SourceCores;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _album.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection other) => _album.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _album.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _album.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _album.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreTrackCollection other) => _album.Equals(other);

    /// <inheritdoc/>
    public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _album.PlayTrackCollectionAsync(track, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetTracksAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _album.AddTrackAsync(track, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbumCollectionItem other) => _album.Equals(other);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollectionItem other) => _album.Equals(other);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollection other) => _album.Equals(other);

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _album.PlayArtistCollectionAsync(artistItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetArtistItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _album.AddArtistItemAsync(artistItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreAlbum other) => _album.Equals(other);

    /// <inheritdoc/>
    public int TotalGenreCount => _album.TotalGenreCount;

    /// <inheritdoc/>
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveGenreAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddGenreAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveGenreAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreGenreCollection other) => _album.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetGenresAsync(limit, offset, cancellationToken).Select(x => new GenrePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _album.AddGenreAsync(genre, index, cancellationToken);

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DetachEvents(_album);
        return _album.DisposeAsync();
    }

    private IArtistCollectionItem Transform(IArtistCollectionItem item) => item switch
    {
        IArtist artist => new ArtistPluginWrapper(artist, _plugins),
        IArtistCollection artistCollection => new ArtistCollectionPluginWrapper(artistCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IArtistCollectionItem>()
    };

    private ITrack Transform(ITrack item) => new TrackPluginWrapper(item, _plugins);

    /// <inheritdoc />
    public DateTime? DatePublished => _album.DatePublished;

    /// <inheritdoc />
    public bool IsChangeDatePublishedAsyncAvailable => _album.IsChangeDatePublishedAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default) => _album.ChangeDatePublishedAsync(datePublished, cancellationToken);

    /// <inheritdoc/>
    public IPlayableCollectionGroup? RelatedItems { get; }
}
