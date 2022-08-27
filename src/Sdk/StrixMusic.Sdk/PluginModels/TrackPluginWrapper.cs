// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
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
/// Wraps an instance of <see cref="ITrack"/> with the provided plugins.
/// </summary>
public class TrackPluginWrapper : ITrack, IPluginWrapper
{
    private readonly ITrack _track;
    private readonly SdkModelPlugin[] _plugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackPluginWrapper"/> class.
    /// </summary>
    /// <param name="track">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal TrackPluginWrapper(ITrack track, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _track = ActivePlugins.Track.Execute(track);
        _plugins = plugins;

        AttachEvents(_track);

        if (_track.Album is not null)
            Album = new AlbumPluginWrapper(_track.Album, _plugins);

        if (_track.RelatedItems is not null)
            RelatedItems = new PlayableCollectionGroupPluginWrapper(_track.RelatedItems, _plugins);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(ITrack track)
    {
        track.SourcesChanged += OnSourcesChanged;
        track.ImagesCountChanged += OnImagesCountChanged;
        track.UrlsCountChanged += OnUrlsCountChanged;
        track.PlaybackStateChanged += OnPlaybackStateChanged;
        track.NameChanged += OnNameChanged;
        track.DescriptionChanged += OnDescriptionChanged;
        track.DurationChanged += OnDurationChanged;
        track.LastPlayedChanged += OnLastPlayedChanged;
        track.IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
        track.IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
        track.IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
        track.IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
        track.IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
        track.ArtistItemsCountChanged += OnArtistItemsCountChanged;
        track.ImagesChanged += OnImagesChanged;
        track.UrlsChanged += OnUrlsChanged;
        track.DownloadInfoChanged += OnDownloadInfoChanged;
        track.ArtistItemsChanged += OnArtistItemsChanged;
        track.GenresCountChanged += OnGenresCountChanged;
        track.TrackNumberChanged += OnTrackNumberChanged;
        track.LanguageChanged += OnLanguageChanged;
        track.IsExplicitChanged += OnIsExplicitChanged;
        track.GenresChanged += OnGenresChanged;
        track.LyricsChanged += OnLyricsChanged;
        track.AlbumChanged += OnAlbumChanged;
    }

    private void DetachEvents(ITrack track)
    {
        track.SourcesChanged -= OnSourcesChanged;
        track.ImagesCountChanged -= OnImagesCountChanged;
        track.UrlsCountChanged -= OnUrlsCountChanged;
        track.PlaybackStateChanged -= OnPlaybackStateChanged;
        track.NameChanged -= OnNameChanged;
        track.DescriptionChanged -= OnDescriptionChanged;
        track.DurationChanged -= OnDurationChanged;
        track.LastPlayedChanged -= OnLastPlayedChanged;
        track.IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
        track.IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
        track.IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
        track.IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
        track.IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
        track.ArtistItemsCountChanged -= OnArtistItemsCountChanged;
        track.ImagesChanged -= OnImagesChanged;
        track.UrlsChanged -= OnUrlsChanged;
        track.DownloadInfoChanged -= OnDownloadInfoChanged;
        track.ArtistItemsChanged -= OnArtistItemsChanged;
        track.GenresCountChanged -= OnGenresCountChanged;
        track.TrackNumberChanged -= OnTrackNumberChanged;
        track.LanguageChanged -= OnLanguageChanged;
        track.IsExplicitChanged -= OnIsExplicitChanged;
        track.GenresChanged -= OnGenresChanged;
        track.LyricsChanged -= OnLyricsChanged;
        track.AlbumChanged -= OnAlbumChanged;
    }

    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

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

    private void OnAlbumChanged(object sender, IAlbum? e)
    {
        if (e is not null)
            Album = new AlbumPluginWrapper(e, _plugins);
        else
            Album = null;
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

    private void OnLyricsChanged(object sender, ILyrics? e) => LyricsChanged?.Invoke(sender, e);

    private void OnGenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems) 
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IGenre>(new GenrePluginWrapper(x.Data, _plugins), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IGenre>(new GenrePluginWrapper(x.Data, _plugins), x.Index)).ToList();

        GenresChanged?.Invoke(sender, wrappedAdded, wrappedRemoved);
    }
    
    private void OnIsExplicitChanged(object sender, bool e) => IsExplicitChanged?.Invoke(sender, e);

    private void OnLanguageChanged(object sender, CultureInfo? e) => LanguageChanged?.Invoke(sender, e);

    private void OnTrackNumberChanged(object sender, int? e) => TrackNumberChanged?.Invoke(sender, e);

    private void OnGenresCountChanged(object sender, int e) => GenresCountChanged?.Invoke(sender, e);

    /// <inheritdoc cref="IMerged.SourcesChanged" />
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
    public event EventHandler<int>? GenresCountChanged;

    /// <inheritdoc/>
    public event EventHandler<int?>? TrackNumberChanged;

    /// <inheritdoc/>
    public event EventHandler<CultureInfo?>? LanguageChanged;

    /// <inheritdoc/>
    public event EventHandler<bool>? IsExplicitChanged;

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IGenre>? GenresChanged;

    /// <inheritdoc/>
    public event EventHandler<IAlbum?>? AlbumChanged;

    /// <inheritdoc/>
    public event EventHandler<ILyrics?>? LyricsChanged;

    /// <inheritdoc/>
    public int TotalImageCount => _track.TotalImageCount;

    /// <inheritdoc/>
    public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsAddImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsRemoveImageAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _track.RemoveImageAsync(index, cancellationToken);

    /// <inheritdoc/>
    public int TotalUrlCount => _track.TotalUrlCount;

    /// <inheritdoc/>
    public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _track.RemoveUrlAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsAddUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsRemoveUrlAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public string Id => _track.Id;

    /// <inheritdoc/>
    public string Name => _track.Name;

    /// <inheritdoc/>
    public string? Description => _track.Description;

    /// <inheritdoc/>
    public DateTime? LastPlayed => _track.LastPlayed;

    /// <inheritdoc/>
    public PlaybackState PlaybackState => _track.PlaybackState;

    /// <inheritdoc/>
    public TimeSpan Duration => _track.Duration;

    /// <inheritdoc/>
    public bool IsChangeNameAsyncAvailable => _track.IsChangeNameAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDescriptionAsyncAvailable => _track.IsChangeDescriptionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeDurationAsyncAvailable => _track.IsChangeDurationAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _track.ChangeNameAsync(name, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _track.ChangeDescriptionAsync(description, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _track.ChangeDurationAsync(duration, cancellationToken);

    /// <inheritdoc/>
    public DateTime? AddedAt => _track.AddedAt;

    /// <inheritdoc/>
    public int TotalArtistItemsCount => _track.TotalArtistItemsCount;

    /// <inheritdoc/>
    public bool IsPlayArtistCollectionAsyncAvailable => _track.IsPlayArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public bool IsPauseArtistCollectionAsyncAvailable => _track.IsPauseArtistCollectionAsyncAvailable;

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _track.PlayArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _track.PauseArtistCollectionAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _track.RemoveArtistItemAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsAddArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreImageCollection other) => _track.Equals(other);

    /// <inheritdoc/>
    IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => ((IMerged<ICoreImageCollection>)_track).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => ((IMerged<ICoreUrlCollection>)_track).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => ((IMerged<ICoreArtistCollectionItem>)_track).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => ((IMerged<ICoreArtistCollection>)_track).Sources;

    /// <inheritdoc/>
    IReadOnlyList<ICoreTrack> IMerged<ICoreTrack>.Sources => ((IMerged<ICoreTrack>)_track).Sources;

    /// <inheritdoc/>
    public IReadOnlyList<ICoreGenreCollection> Sources => ((IMerged<ICoreGenreCollection>)_track).Sources;

    /// <inheritdoc/>
    public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _track.GetImagesAsync(limit, offset, cancellationToken).Select(x => new ImagePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _track.AddImageAsync(image, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreUrlCollection other) => _track.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _track.GetUrlsAsync(limit, offset, cancellationToken).Select(x => new UrlPluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _track.AddUrlAsync(url, index, cancellationToken);

    /// <inheritdoc/>
    public DownloadInfo DownloadInfo => _track.DownloadInfo;

    /// <inheritdoc/>
    public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _track.StartDownloadOperationAsync(operation, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollectionItem other) => _track.Equals(other);

    /// <inheritdoc/>
    public bool Equals(ICoreArtistCollection other) => _track.Equals(other);

    /// <inheritdoc/>
    public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _track.PlayArtistCollectionAsync(artistItem, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _track.GetArtistItemsAsync(limit, offset, cancellationToken).Select(Transform);

    /// <inheritdoc/>
    public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _track.AddArtistItemAsync(artistItem, index, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreTrack other) => _track.Equals(other);

    /// <inheritdoc/>
    public int TotalGenreCount => _track.TotalGenreCount;

    /// <inheritdoc/>
    public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _track.RemoveGenreAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsAddGenreAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _track.IsRemoveGenreAvailableAsync(index, cancellationToken);

    /// <inheritdoc/>
    public TrackType Type => _track.Type;

    /// <inheritdoc/>
    public int? TrackNumber => _track.TrackNumber;

    /// <inheritdoc/>
    public int? DiscNumber => _track.DiscNumber;

    /// <inheritdoc/>
    public CultureInfo? Language => _track.Language;

    /// <inheritdoc/>
    public bool IsExplicit => _track.IsExplicit;

    /// <inheritdoc/>
    public bool IsChangeAlbumAsyncAvailable => _track.IsChangeAlbumAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeTrackNumberAsyncAvailable => _track.IsChangeTrackNumberAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeLanguageAsyncAvailable => _track.IsChangeLanguageAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeLyricsAsyncAvailable => _track.IsChangeLyricsAsyncAvailable;

    /// <inheritdoc/>
    public bool IsChangeIsExplicitAsyncAvailable => _track.IsChangeIsExplicitAsyncAvailable;

    /// <inheritdoc/>
    public Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default) => _track.ChangeTrackNumberAsync(trackNumber, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default) => _track.ChangeLanguageAsync(language, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default) => _track.ChangeIsExplicitAsync(isExplicit, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreGenreCollection other) => _track.Equals(other);

    /// <inheritdoc/>
    public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _track.GetGenresAsync(limit, offset, cancellationToken).Select(x => new GenrePluginWrapper(x, _plugins));

    /// <inheritdoc/>
    public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _track.AddGenreAsync(genre, index, cancellationToken);

    /// <inheritdoc/>
    public IAlbum? Album { get; private set; }

    /// <inheritdoc/>
    public ILyrics? Lyrics => _track.Lyrics;

    /// <inheritdoc/>
    public IPlayableCollectionGroup? RelatedItems { get; }

    /// <inheritdoc/>
    public Task ChangeLyricsAsync(ILyrics? lyrics, CancellationToken cancellationToken = default) => _track.ChangeLyricsAsync(lyrics, cancellationToken);

    /// <inheritdoc/>
    public Task ChangeAlbumAsync(IAlbum? album, CancellationToken cancellationToken = default) => _track.ChangeAlbumAsync(album, cancellationToken);

    private IArtistCollectionItem Transform(IArtistCollectionItem item) => item switch
    {
        IArtist artist => new ArtistPluginWrapper(artist, _plugins),
        IArtistCollection artistCollection => new ArtistCollectionPluginWrapper(artistCollection, _plugins),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<IArtistCollectionItem>()
    };
}
