// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;
using OwlCore.Provisos;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// An <see cref="ICore"/> is a common API surface that can be implemented to allow interfacing with any arbitrary music provider.
    /// </summary>
    /// <remarks> In a core's constructor, only do basic object initialization. For the rest, use <see cref="IAsyncInit.InitAsync"/>.
    /// 
    /// <para/> During <see cref="IAsyncInit.InitAsync"/>, if the core state is changed to <see cref="CoreState.Loading"/>, this task will be canceled
    ///         and the app will display your current <see cref="AbstractConfigPanel"/> to the user for configuration and setup.
    ///         After the user has completed setup, change the core state back to <see cref="CoreState.Configured"/> using the AbstractUI elements.
    ///         <see cref="IAsyncInit.InitAsync"/> will fire again, at which point you can make sure you have the data you need to finish initialization.
    /// 
    /// <para/> There is a 10 minute time limit for the user to complete setup.
    ///         If it takes longer, the SDK will assume something has gone wrong and unload the core until the user manually initiates setup or restarts the app.
    /// </remarks>
    /// <seealso cref="IAppCore"/>
    public interface ICore : IAsyncInit, ICoreMember, IAsyncDisposable
    {
        /// <summary>
        /// The registered metadata for this core. Contains information to identify the core before creating an instance.
        /// </summary>
        public CoreMetadata Registration { get; }

        /// <summary>
        /// Identifies this instance of the core. This is given to each core via the constructor.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// A string of text to display to the user to help identify which core instance this is, such as a username or the path to a file location. Longer strings will be truncated as needed.
        /// </summary>
        public string InstanceDescriptor { get; }

        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        public AbstractUICollection AbstractConfigPanel { get; }

        /// <summary>
        /// The player type supported by this core.
        /// </summary>
        public MediaPlayerType PlaybackType { get; }

        /// <summary>
        /// The user that is authenticated with this core, if any. Only one user is supported per core.
        /// </summary>
        public ICoreUser? User { get; }

        /// <summary>
        /// The available devices. These should only be populated with remote devices, if supported by the core.
        /// Local playback is handled by the SDK by calling <see cref="GetMediaSource(ICoreTrack)"/>.
        /// </summary>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core. This must never be null, but it may return 0 items if needed.
        /// </summary>
        public ICoreLibrary Library { get; }

        /// <summary>
        /// Pins act as "bookmarks" or "shortcuts", things that the user has chosen to "pin" somewhere for easy access.
        /// </summary>
        /// <remarks>
        /// If this is left null, this will be managed by the app. If not, the core will be entirely responsible for managing this.
        /// </remarks>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <summary>
        /// Contains various search-related data and activities.
        /// </summary>
        /// <remarks>
        /// If this is left null, this will be managed by the app. If not, the core will be entirely responsible for managing this.
        /// </remarks>
        public ICoreSearch? Search { get; }

        /// <summary>
        /// The items that the user has been recently played by the user. 
        /// </summary>
        /// <remarks>
        /// If this is left null, this will be managed by the app. If not, the core will be entirely responsible for managing this.
        /// </remarks>
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public ICoreDiscoverables? Discoverables { get; }

        /// <summary>
        /// The current state of the core.
        /// </summary>
        public CoreState CoreState { get; }

        /// <summary>
        /// Given the ID of an instance created by this core, return the fully constructed instance.
        /// </summary>
        /// <returns>The requested instance, cast down to <see cref="ICoreMember"/>.</returns>
        public Task<ICoreMember?> GetContextById(string id);

        /// <summary>
        /// Converts a <see cref="ICoreTrack"/> into a <see cref="IMediaSourceConfig"/> that can be used to play the track.
        /// </summary>
        /// <param name="track">The track to convert.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The value is an <see cref="IMediaSourceConfig"/> that can be used to play the track.</returns>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track);

        /// <summary>
        /// Raised when the <see cref="Models.CoreState"/> has changed.
        /// </summary>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <summary>
        /// Raised when the contents of <see cref="Devices"/> is changed.
        /// </summary>
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <summary>
        /// Raised when <see cref="AbstractUIElement"/> is changed.
        /// </summary>
        public event EventHandler? AbstractConfigPanelChanged;

        /// <summary>
        /// Raised when <see cref="InstanceDescriptor"/> is changed.
        /// </summary>
        public event EventHandler<string>? InstanceDescriptorChanged;
    }
}
