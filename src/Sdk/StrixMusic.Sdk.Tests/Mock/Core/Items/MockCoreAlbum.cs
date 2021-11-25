﻿using OwlCore.Events;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core.Items
{
    public class MockCoreAlbum : ICoreAlbum
    {
        public MockCoreAlbum(ICore sourceCore, string id, string name)
        {
            SourceCore = sourceCore;
            Id = id;
            Name = name;
        }

        public ICorePlayableCollectionGroup? RelatedItems { get; set; }

        public DateTime? DatePublished { get; set; }

        public bool IsChangeDatePublishedAsyncAvailable { get; set; }

        public int TotalArtistItemsCount { get; set; }

        public bool IsPlayArtistCollectionAsyncAvailable { get; set; }

        public bool IsPauseArtistCollectionAsyncAvailable { get; set; }

        public int TotalTrackCount { get; set; }

        public bool IsPlayTrackCollectionAsyncAvailable { get; set; }

        public bool IsPauseTrackCollectionAsyncAvailable { get; set; }

        public DateTime? AddedAt { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? LastPlayed { get; set; }

        public PlaybackState PlaybackState { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsChangeNameAsyncAvailable { get; set; }

        public bool IsChangeDescriptionAsyncAvailable { get; set; }

        public bool IsChangeDurationAsyncAvailable { get; set; }

        public int TotalImageCount { get; set; }

        public int TotalUrlCount { get; set; }

        public int TotalGenreCount { get; set; }

        public ICore SourceCore { get; set; }

        public event EventHandler<DateTime?>? DatePublishedChanged;
        public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;
        public event EventHandler<int>? ArtistItemsCountChanged;
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;
        public event EventHandler<int>? TracksCountChanged;
        public event EventHandler<PlaybackState>? PlaybackStateChanged;
        public event EventHandler<string>? NameChanged;
        public event EventHandler<string?>? DescriptionChanged;
        public event EventHandler<TimeSpan>? DurationChanged;
        public event EventHandler<DateTime?>? LastPlayedChanged;
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
        public event EventHandler<int>? ImagesCountChanged;
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;
        public event EventHandler<int>? UrlsCountChanged;
        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;
        public event EventHandler<int>? GenresCountChanged;

        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddGenreAsync(ICoreGenre genre, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddUrlAsync(ICoreUrl url, int index)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield return MockItemFactory.CreateArtist(SourceCore);
        }

        public IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICoreGenre>();
        }

        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICoreImage>();
        }

        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield return MockItemFactory.CreateTrack(SourceCore);
        }

        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddTrackAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveTrackAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task PauseArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PauseTrackCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            throw new NotImplementedException();
        }

        public Task PlayTrackCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveGenreAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTrackAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUrlAsync(int index)
        {
            throw new NotImplementedException();
        }
    }
}