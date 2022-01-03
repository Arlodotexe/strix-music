using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="ICoreBase"/>
    /// <remarks>In a core's constructor, only do basic object initialization. For heavy work, use <see cref="InitAsync"/>.</remarks>
    public interface ICore : ICoreMember, ICoreBase, IAsyncDisposable
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

        /// <inheritdoc cref="ICoreConfigBase" />
        public ICoreConfig CoreConfig { get; }

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

#pragma warning disable 1574, 1584, 1581, 1580
        /// <summary>
        /// Initializes the core asynchronously, allowing for API setup, service configuration, and other asynchronous tasks to be performed.
        /// </summary>
        /// <remarks>
        /// <para> If the core state is changed to <see cref="CoreState.Configuring"/>, this task will be canceled
        /// and the app will display your current <see cref="ICoreConfigBase.AbstractUIElements"/> to the user for configuration and setup.
        /// After the user has completed setup, change the core state back to <see cref="CoreState.Configured"/> using the AbstractUI elements.
        /// Once complete, this method will fire again, at which point you should have the data you need to finish initialization.</para>
        /// 
        /// <para> There is a 10 minute time limit for the user to complete setup.
        /// If it takes longer, the SDK will assume something has gone wrong and unload the core until the user manually initiates setup or restarts the app. </para>
        /// </remarks>
        /// <param name="services">
        /// <para> Here, various <paramref name="services"/> are injected into each core by the SDK. You may use these or discard them. These include:</para>
        /// <list type="bullet|number|table">
        /// <listheader>
        /// <term>IFileSystemService</term>
        /// <description>An abstracted service that deals with file system operations, including those not handled by System.IO (such as File/Folder pickers)</description>
        /// </listheader>
        /// <item>
        /// <term>INotificationsService</term>
        /// <description>Provides ways for cores to raise notifications to the user to see in the UI. Use sparingly.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAsync(IServiceCollection services);

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
        /// Raised when <see cref="InstanceDescriptor"/> is changed.
        /// </summary>
        public event EventHandler<string>? InstanceDescriptorChanged;
    }
}
