using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using OwlCore.Remoting;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.CoreRemote.Models;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <inheritdoc />
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public class RemoteCore : ICore
    {
        private static readonly ConcurrentDictionary<string, RemoteCore> _externalCoreInstances
            = new ConcurrentDictionary<string, RemoteCore>();

        private readonly MemberRemote _memberRemote;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCore"/>.
        /// </summary>
        /// <param name="instanceId"></param>
        public RemoteCore(string instanceId)
        {
            _externalCoreInstances.TryAdd(instanceId, this);

            InstanceId = instanceId;

            Devices = new List<ICoreDevice>();
            RecentlyPlayed = new RemoteCoreRecentlyPlayed(instanceId);
            Discoverables = new RemoteCoreDiscoverables(instanceId);
            Library = new RemoteCoreLibrary(instanceId);
            Pins = new RemoteCorePins(instanceId);

            CoreConfig = new RemoteCoreConfig(instanceId);

            _memberRemote = new MemberRemote(this, $"{instanceId}.{nameof(RemoteCore)}", RemoteCoreRemoteMessageHandler.Singleton); 
        }

        /// <summary>
        /// Wraps around and remotely relays a core instance.
        /// </summary>
        /// <param name="core"></param>
        public RemoteCore(ICore core)
        {
            _externalCoreInstances.TryAdd(core.InstanceId, this);

            InstanceId = core.InstanceId;
            InstanceDescriptor = core.InstanceDescriptor;

            Devices = core.Devices;
            RecentlyPlayed = core.RecentlyPlayed;
            Library = core.Library;
            Pins = core.Pins;

            CoreConfig = core.CoreConfig;

            _memberRemote = new MemberRemote(this, $"{InstanceId}.{nameof(RemoteCore)}", RemoteCoreRemoteMessageHandler.Singleton);
        }

        /// <summary>
        /// Gets a created <see cref="RemoteCore"/> instance by instance ID.
        /// </summary>
        /// <returns>The core instance.</returns>
        /// <exception cref="InvalidOperationException"/>
        public static RemoteCore GetInstance(string instanceId)
        {
            if (!_externalCoreInstances.TryGetValue(instanceId, out var value))
                return ThrowHelper.ThrowInvalidOperationException<RemoteCore>($"Could not find a registered {nameof(RemoteCore)} with {nameof(instanceId)} of {instanceId}");

            return value;
        }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public string CoreRegistryId => nameof(RemoteCore);

        /// <inheritdoc />
        [RemoteProperty]
        public string? DisplayName { get; } // TODO

        /// <inheritdoc />
        [RemoteProperty]
        public Uri? LogoPath { get; } // TODO

        /// <inheritdoc/>
        [RemoteProperty]
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        [RemoteProperty]
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc />
        [RemoteProperty]
        public string InstanceDescriptor { get; private set; } = string.Empty;

        /// <inheritdoc/>
        [RemoteProperty]
        public ICoreUser? User { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public IReadOnlyList<ICoreDevice> Devices { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public ICoreLibrary Library { get; set; }

        /// <inheritdoc />
        [RemoteProperty]
        public ICoreSearch? Search { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc/>
        [RemoteProperty]
        public ICoreDiscoverables? Discoverables { get; }

        /// <inheritdoc/>
        [RemoteProperty]
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc/>
        [RemoteMethod]
        public ValueTask DisposeAsync()
        {
            // Dispose any resources not known to the SDK.
            // Do not dispose Library, Devices, etc. manually. The SDK will dispose these for you.
            return default;
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public async Task InitAsync(IServiceCollection services)
        {
            // ==========================
            // WARNING
            // ==========================
            // This class is still very WIP. Do not attempt to use it

            // ==========================
            // Train of thought for later:
            // ==========================

            // Services - these are pre-injected and provided by the SDK.
            // Are they all needed? Should be provided remotely?

            // Any interface would need a Bidirectional-capable remote proxy class available
            // for all SDK implementors (same class used in client/host from SDK)

            // INotificationsService -- YES! (Done)
            // ILocalizationService -- No.
            // IFileSystemService -- No.
            //  - For picking a file and keeping access.
            //  - Requires remoting all IFileData and IFolderData implementations

            // ---------------------------

            // Should we call InitAsync manually on every model?
            // Do we even need InitAsync on everything?
            // They should be able to call InitAsync when they need it, like when (/if) an item is retrieved from the API but needs extra init beyond the constructor
            // Their own API should determine if init async is even needed at all

            // answer:
            // No.
            // The asynchronous nature of how items returned from an API + remote lock mean they always have the chance to call initasync and we always wait for async calls on their end to finish.
            // Remote properties mean we don't care how or when it's called or what it even does

            // ---------------------------

            // Should the dev need to create their own ExternalCore from scratch every time, or can we reduce the work?
            // Base classes?
            //   - Would allow us to update core/remoting functionality with a nuget package (!!)
            //   - Remoting may not work if the name of the derived classes are different in host/client (fixed)
            //       - Need to adjust OwlCore.Remoting to have a "loose" mode option where only the ID and property/method names need to match.
            //   - Use the same ExternalCore project for both host/client, or create a new project with separate code for host / client?

            // ============
            // TODO:
            // Remaining remoting implementation for ExternalCore models
            // Generic PlayableCollectionGroup implementation for e.g. RelatedItems
            // Set up remote notification service here.
            // ============
            await _memberRemote.RemoteWaitAsync(nameof(InitAsync));
        }

        /// <inheritdoc/>
        public async Task<ICoreMember?> GetContextById(string id)
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            return Task.FromResult<IMediaSourceConfig?>(null);
        }
    }
}
