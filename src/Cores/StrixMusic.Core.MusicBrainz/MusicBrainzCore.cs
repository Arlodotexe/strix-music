using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Models;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz
{
    /// <summary>
    /// Mock Core
    /// </summary>
    public class MusicBrainzCore : ICore
    {
        private readonly MusicBrainzClient _musicBrainzClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public MusicBrainzCore(string instanceId)
        {
            InstanceId = instanceId;
            CoreConfig = new MusicBrainzCoreConfig(this);

            // The library created here won't be used by the UI.
            // The UI isn't loaded until InitAsync is called, where we set up the actual library.
            Library = new MusicBrainzLibrary(this);
            Devices = new List<IDevice>();
            RecentlyPlayed = new MusicBrainzRecentlyPlayed(this);
            Discoverables = new MusicBrainzDiscoverables(this);
            User = new MusicBrainzUser(this);

            _musicBrainzClient = CoreConfig.Services.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc/>
        public string Name => "MusicBrainz";

        /// <inheritdoc/>
        public IUser User { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices { get; }

        /// <inheritdoc/>
        public ILibrary Library { get; private set; }

        /// <inheritdoc/>
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <inheritdoc/>
        public IDiscoverables Discoverables { get; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IDevice>>? DevicesChanged;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return default;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<string>?> GetSearchAutoCompleteAsync(string query)
        {
            return Task.FromResult<IReadOnlyList<string>?>(default);
        }

        /// <inheritdoc/>
        public async Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
            var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);

            var results = new MusicBrainzSearchResults(this, query)
            {
                TotalTracksCount = recordings.Count,
                TotalAlbumsCount = releases.Count,
                TotalArtistsCount = artists.Count,
                TotalPlaylistCount = 0,
                TotalChildrenCount = 0,
            };

            return results;
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            CoreState = CoreState.Loading;
            CoreStateChanged?.Invoke(this, CoreState);

            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
            var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);

            Library = new MusicBrainzLibrary(this)
            {
                TotalTracksCount = recordings.Count,
                TotalAlbumsCount = releases.Count,
                TotalArtistsCount = artists.Count,
                TotalPlaylistCount = 0,
                TotalChildrenCount = 0,
            };

            CoreState = CoreState.Loaded;
            CoreStateChanged?.Invoke(this, CoreState);
        }
    }
}
