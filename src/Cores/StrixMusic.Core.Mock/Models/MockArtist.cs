﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc />
    public class MockArtist : IArtist
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string MockName { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("album_ids")]
        public List<string>? AlbumIdsJson { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalAlbumsCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore => throw new NotImplementedException();

        /// <inheritdoc/>
        public string IdJson => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>> AlbumsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>> TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string> NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string> DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri> UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan> DurationChanged;

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string description)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }
    }
}
