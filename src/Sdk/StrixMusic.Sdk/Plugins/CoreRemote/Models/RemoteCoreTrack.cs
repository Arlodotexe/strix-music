// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using OwlCore.Events;
using OwlCore.Remoting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICoreArtist"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    public sealed class RemoteCoreTrack : ICoreTrack
    {
        private readonly MemberRemote _memberRemote;
        private readonly ICoreTrack? _track;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreTrack"/>.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The instance ID of the core that created this object.</param>
        /// <param name="id">A unique identifier for this instance.</param>
        /// <param name="name">The name of the data.</param>
        internal RemoteCoreTrack(string sourceCoreInstanceId, string name, string id)
        {
            Name = name;
            Id = id;

            // Properties assigned before MemberRemote is created won't be set remotely.
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, RemotingMode.Client); // should be set remotely by the ctor.

            _memberRemote = new MemberRemote(this, $"{sourceCoreInstanceId}.{nameof(RemoteCoreTrack)}.{id}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Wraps around and remotely relays events, property changes and method calls (with return data) from an track instance.
        /// </summary>
        /// <param name="coreTrack"></param>
        internal RemoteCoreTrack(ICoreTrack coreTrack)
        {
            _track = coreTrack;
            Name = coreTrack.Name;
            Id = coreTrack.Id;
            SourceCore = RemoteCore.GetInstance(coreTrack.SourceCore.InstanceId, RemotingMode.Host);

            _memberRemote = new MemberRemote(this, $"{coreTrack.SourceCore.InstanceId}.{nameof(RemoteCoreTrack)}.{coreTrack.Id}", RemoteCoreMessageHandler.SingletonHost);
        }

        /// <inheritdoc/>
        public event EventHandler<ICoreAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<ICoreLyrics?>? LyricsChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsExplicitChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

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
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? GenresCountChanged;

        /// <inheritdoc/>
        public ICore SourceCore { get; set; }

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public DateTime? LastPlayed { get; set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; set; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; set; }

        /// <inheritdoc/>
        public ICoreAlbum? Album { get; set; }

        /// <inheritdoc/>
        public ICoreLyrics? Lyrics { get; set; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; set; }

        /// <inheritdoc/>
        public TrackType Type { get; set; }

        /// <inheritdoc/>
        public int? TrackNumber { get; set; }

        /// <inheritdoc/>
        public int? DiscNumber { get; set; }

        /// <inheritdoc/>
        public CultureInfo? Language { get; set; }

        /// <inheritdoc/>
        public bool IsExplicit { get; set; }

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; set; }

        /// <inheritdoc/>
        public bool IsPlayArtistCollectionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsPauseArtistCollectionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public DateTime? AddedAt { get; set; }

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable { get; set; }

        /// <inheritdoc/>
        public int TotalImageCount { get; set; }

        /// <inheritdoc/>
        public int TotalUrlCount { get; set; }

        /// <inheritdoc/>
        public int TotalGenreCount { get; set; }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ICoreLyrics? lyrics)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(ICoreAlbum? albums)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddUrlAsync(ICoreUrl url, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveImageAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveUrlAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task AddGenreAsync(ICoreGenre genre, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveGenreAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync() => default;
    }
}
