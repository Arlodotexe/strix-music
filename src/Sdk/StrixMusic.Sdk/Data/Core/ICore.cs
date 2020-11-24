using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="ICoreBase"/>
    /// <remarks>In a core's constructor, only do basic object initialization. For heavy work, use <see cref="InitAsync"/>.</remarks>
    public interface ICore : ICoreMember, ICoreBase, IAsyncDisposable, IAsyncInit
    {
        /// <summary>
        /// Identifies this instance of the core.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The name of the core.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc cref="ICoreConfigBase" />
        public ICoreConfig CoreConfig { get; }

        /// <summary>
        /// The user that is authenticated with this core.
        /// </summary>
        public ICoreUser User { get; }

        /// <summary>
        /// The available devices.
        /// </summary>
        public SynchronizedObservableCollection<ICoreDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ICoreLibrary Library { get; }

        /// <summary>
        /// A list of pinned playable items.
        /// </summary>
        public ICorePlayableCollectionGroup Pins { get; }

        /// <summary>
        /// Gets the recently played items for this core.
        /// </summary>
        public ICoreRecentlyPlayed RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public ICoreDiscoverables Discoverables { get; }

        /// <inheritdoc cref="Data.CoreState" />
        public CoreState CoreState { get; }

        /// <summary>
        /// Initializes the <see cref="ICore"/> asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAsync(IServiceCollection services);

        /// <summary>
        /// Gets the object against a context.
        /// </summary>
        /// <returns>Returns the requested context, cast down to an <see cref="object"/>.</returns>
        public IAsyncEnumerable<ICoreMember> GetContextById(string id);

        /// <summary>
        /// Converts a <see cref="ICoreTrack"/> into a <see cref="IMediaSourceConfig"/> that can be used to play the track.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The value is an <see cref="IMediaSourceConfig"/> that can be used to play the track.</returns>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track);

        /// <summary>
        /// Fires when the <see cref="Data.CoreState"/> has changed.
        /// </summary>
        public event EventHandler<CoreState>? CoreStateChanged;
    }
}
