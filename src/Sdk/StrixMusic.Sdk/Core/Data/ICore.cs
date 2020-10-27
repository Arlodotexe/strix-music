using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A <see cref="ICore"/> is a common API surface that can be implemented to interface Strix with an arbitrary music service provider.
    /// </summary>
    /// <remarks>In a core's constructor, only do basic object initialization. For heavy work, use <see cref="InitAsync"/>.</remarks>
    public interface ICore : IAsyncDisposable
    {
        /// <inheritdoc cref="ICoreConfig" />
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc cref="Data.CoreState" />
        public CoreState CoreState { get; }

        /// <summary>
        /// Identifies this instance of the core.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The name of the core.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Represents the user that is authenticated with this core.
        /// </summary>
        public IUser User { get; }

        /// <summary>
        /// The available devices.
        /// </summary>
        public SynchronizedObservableCollection<IDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ILibrary Library { get; }

        /// <summary>
        /// A list of pinned playable items.
        /// </summary>
        public SynchronizedObservableCollection<IPlayable> Pins { get; }

        /// <summary>
        /// Gets the recently played items for this core.
        /// </summary>
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public IDiscoverables Discoverables { get; }

        /// <summary>
        /// Initializes the <see cref="ICore"/> asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAsync(IServiceCollection services);

        /// <summary>
        /// Given a query, return suggested completed queries.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Suggested completed queries.</returns>
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query);

        /// <summary>
        /// Gets search results for a given query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>A task representing the async operation. Returns <see cref="ISearchResults"/> containing multiple.</returns>
        public Task<ISearchResults> GetSearchResultsAsync(string query);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlayable"/> at a specific position in <see cref="Pins"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlayable"/> can be added.</returns>
        Task<bool> IsAddPinSupported(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlayable"/> from a specific position in <see cref="Pins"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IPlayable"/> can be removed.</returns>
        Task<bool> IsRemovePinSupported(int index);

        /// <summary>
        /// Gets the object against a context.
        /// </summary>
        /// <returns>Returns the requested context, cast down to an <see cref="object"/>.</returns>
        public IAsyncEnumerable<object?> GetContextById(string id);

        /// <summary>
        /// Converts a <see cref="ITrack"/> into a <see cref="IMediaSourceConfig"/> that can be used to play the track.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The value is an <see cref="IMediaSourceConfig"/> that can be used to play the track.</returns>
        public Task<IMediaSourceConfig?> GetMediaSource(ITrack track);

        /// <summary>
        /// Fires when the <see cref="CoreState"/> has changed.
        /// </summary>
        public event EventHandler<CoreState>? CoreStateChanged;
    }
}
