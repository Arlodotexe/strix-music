using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Core.External.Models;
using StrixMusic.Core.External.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.External
{
    /// <inheritdoc />
    public class ExternalCore : ICore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public ExternalCore(string instanceId)
        {
            InstanceId = instanceId;

            Devices = new SynchronizedObservableCollection<ICoreDevice>();
            RecentlyPlayed = new ExternalCoreRecentlyPlayed(this);
            Discoverables = new ExternalCoreDiscoverables(this);
            CoreConfig = new ExternalCoreConfig(this);
            Library = new ExternalCoreLibrary(this);
        }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc />
        public string InstanceDescriptor { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public ICoreUser? User { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library { get; }

        /// <inheritdoc />
        public ICoreSearch? Search { get; }

        /// <inheritdoc/>
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc/>
        public ICoreDiscoverables? Discoverables { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler<string>? InstanceDescriptorChanged;

        /// <summary>
        /// Change the <see cref="CoreState"/>.
        /// </summary>
        /// <param name="state">The new state.</param>
        internal void ChangeCoreState(CoreState state)
        {
            CoreState = state;
            CoreStateChanged?.Invoke(this, state);
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            // Dispose any resources not known to the SDK.
            // Do not dispose Library, Devices, etc. manually. The SDK will dispose these for you.
            return default;
        }

        /// <inheritdoc/>
        public async Task InitAsync(IServiceCollection services)
        {
            Guard.IsNotNull(services, nameof(services));

            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is ExternalCoreConfig coreConfig))
                return;

            await coreConfig.SetupConfigurationServices(services);

            var configuredFolder = await coreConfig.GetConfiguredFolder();
            if (configuredFolder is null)
            {
                ChangeCoreState(CoreState.NeedsSetup);
                return;
            }

            InstanceDescriptor = configuredFolder.Path;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);

            await coreConfig.SetupServices(services);
            await Library.Cast<ExternalCoreLibrary>().InitAsync();

            Guard.IsNotNull(CoreConfig.Services, nameof(CoreConfig.Services));
            ChangeCoreState(CoreState.Loaded);
        }

        /// <inheritdoc/>
        public async Task<ICoreMember?> GetContextById(string id)
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            return null;
        }
    }
}
