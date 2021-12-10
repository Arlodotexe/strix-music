using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICore"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    /// <remarks>
    /// Passing a core instance will enable remoting for the ENTIRE core, including library, search, playback, devices and all other feature.
    /// </remarks>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public class RemoteCore : ICore
    {
        private static readonly ConcurrentDictionary<string, RemoteCore> _externalCoreInstances
            = new ConcurrentDictionary<string, RemoteCore>();

        private readonly MemberRemote _memberRemote;
        private readonly ICore? _core;

        private readonly List<ICoreDevice> _devices = new List<ICoreDevice>();
        private CoreState _coreState = CoreState.Unloaded;
        private string _instanceDescriptor = string.Empty;

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCore"/>.
        /// </summary>
        /// <param name="instanceId"></param>
        public RemoteCore(string instanceId)
        {
            _externalCoreInstances.TryAdd(instanceId, this);

            InstanceId = instanceId;

            RecentlyPlayed = new RemoteCoreRecentlyPlayed(instanceId, CommonRemoteIds.RootRecentlyPlayed);
            Discoverables = new RemoteCoreDiscoverables(instanceId, CommonRemoteIds.RootDiscoverables);
            Library = new RemoteCoreLibrary(instanceId, CommonRemoteIds.RootLibrary);
            Pins = new RemoteCorePlayableCollectionGroup(instanceId, CommonRemoteIds.Pins);

            CoreConfig = new RemoteCoreConfig(instanceId);

            // Registration is set remotely, use placeholder data here.
            Registration = new CoreMetadata(string.Empty, string.Empty, new Uri("/", UriKind.Relative), sdkVersion: new Version(0, 0, 0));

            _memberRemote = new MemberRemote(this, $"{instanceId}.{nameof(RemoteCore)}", RemoteCoreMessageHandler.SingletonClient);
        }

        /// <summary>
        /// Wraps around and remotely relays events, property changes and method calls (with return data) from a core instance.
        /// </summary>
        /// <param name="core"></param>
        public RemoteCore(ICore core)
        {
            Guard.IsNotNull(core, nameof(core));

            _ = _externalCoreInstances.TryAdd(core.InstanceId, this);
            _core = core;

            RecentlyPlayed = core.RecentlyPlayed;
            Library = new RemoteCoreLibrary(core.Library);
            Pins = core.Pins;

            CoreConfig = core.CoreConfig;

            AttachEvents(core);

            _memberRemote = new MemberRemote(this, $"{core.InstanceId}.{nameof(RemoteCore)}", RemoteCoreMessageHandler.SingletonHost);

            Registration = core.Registration;
            InstanceDescriptor = core.InstanceDescriptor;
            InstanceId = core.InstanceId;
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

        private void AttachEvents(ICore core)
        {
            core.InstanceDescriptorChanged += OnInstanceDescriptorChanged;
            core.DevicesChanged += OnDevicesChanged;
            core.CoreStateChanged += OnCoreStateChanged;
        }

        private void DetachEvents(ICore core)
        {
            core.InstanceDescriptorChanged -= OnInstanceDescriptorChanged;
            core.DevicesChanged -= OnDevicesChanged;
            core.CoreStateChanged -= OnCoreStateChanged;
        }

        private void OnCoreStateChanged(object sender, CoreState e) => CoreState = e;

        private void OnInstanceDescriptorChanged(object sender, string e)
        {
            InstanceDescriptor = e;
        }

        private void OnDevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems)
        {
            ChangeDevices(addedItems, removedItems);
        }

        [RemoteMethod, RemoteOptions(RemotingDirection.HostToClient)]
        private void ChangeDevices(IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems)
        {
            var remoteAddedItems = addedItems.Select(x => new CollectionChangedItem<ICoreDevice>(new RemoteCoreDevice(x.Data), x.Index)).ToList();
            var remoteRemovedItems = removedItems.Select(x => new CollectionChangedItem<ICoreDevice>(new RemoteCoreDevice(x.Data), x.Index)).ToList();

            _devices.ChangeCollection(remoteAddedItems, remoteRemovedItems);
            DevicesChanged?.Invoke(this, remoteAddedItems, remoteRemovedItems);
        }

        /// <inheritdoc/>
        [RemoteProperty]
        public CoreMetadata Registration { get; set; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        [RemoteProperty]
        public CoreState CoreState
        {
            get => _coreState;
            set
            {
                _coreState = value;
                CoreStateChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        [RemoteProperty]
        public string InstanceDescriptor
        {
            get => _instanceDescriptor;
            set
            {
                _instanceDescriptor = value;
                InstanceDescriptorChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc/>
        [RemoteProperty]
        public ICoreUser? User { get; set; }

        /// <inheritdoc/>
        [RemoteProperty]
        public IReadOnlyList<ICoreDevice> Devices => _devices;

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
            _externalCoreInstances.TryRemove(InstanceId, out _);
            return default;
        }

        /// <inheritdoc/>
        public Task InitAsync(IServiceCollection services) => Task.Run(async () =>
        {
            if (_memberRemote.Mode == RemotingMode.Host)
                return;

            SetupRemoteServices(services, InstanceId);
            await RemoteInitAsync();

            await _memberRemote.RemoteWaitAsync(nameof(InitAsync));
        });

        [RemoteMethod, RemoteOptions(RemotingDirection.ClientToHost)]
        private Task RemoteInitAsync() => Task.Run(async () =>
        {
            if (_memberRemote.Mode == RemotingMode.Client)
                return;

            Guard.IsNotNull(_core, nameof(_core));

            var services = SetupRemoteServices(InstanceId);
            await _core.InitAsync(services);

            await _memberRemote.RemoteReleaseAsync(nameof(InitAsync));
        });

        /// <inheritdoc/>
        [RemoteMethod, RemoteOptions(RemotingDirection.ClientToHost)]
        public Task<ICoreMember?> GetContextById(string id) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(GetContextById)}.{id}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_core, nameof(_core));

                var result = await _core.GetContextById(id);

                ICoreMember? remoteEnabledResult = result switch
                {
                    ICore core => GetInstance(core.InstanceId),
                    ICoreAlbum album => new RemoteCoreAlbum(album),
                    ICoreArtist artist => new RemoteCoreArtist(artist),
                    ICoreTrack track => new RemoteCoreTrack(track),
                    ICorePlaylist playlist => new RemoteCorePlaylist(playlist),
                    ICoreDevice device => new RemoteCoreDevice(device),
                    ICoreDiscoverables discoverables => new RemoteCoreDiscoverables(discoverables),
                    ICoreImage image => new RemoteCoreImage(image),
                    ICoreLibrary library => new RemoteCoreLibrary(library),
                    ICoreRecentlyPlayed recentlyPlayed => new RemoteCoreRecentlyPlayed(recentlyPlayed),
                    ICoreSearchHistory searchHistory => new RemoteCoreSearchHistory(searchHistory),
                    ICorePlayableCollectionGroup collectionGroup => new RemoteCorePlayableCollectionGroup(collectionGroup),
                    _ => throw new NotImplementedException(),
                };

                return await _memberRemote.PublishDataAsync(methodCallToken, result);
            }
            else if (_memberRemote.Mode == RemotingMode.Client)
            {
                return await _memberRemote.ReceiveDataAsync<ICoreMember?>(methodCallToken);
            }
            else
            {
                return null;
            }
        });

        /// <inheritdoc/>
        [RemoteMethod, RemoteOptions(RemotingDirection.ClientToHost)]
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track) => Task.Run(async () =>
        {
            var methodCallToken = $"{nameof(GetMediaSource)}.{track.Id}";

            if (_memberRemote.Mode == RemotingMode.Host)
            {
                Guard.IsNotNull(_core, nameof(_core));

                var result = await _core.GetMediaSource(track);
                return await _memberRemote.PublishDataAsync(methodCallToken, result);
            }
            else if (_memberRemote.Mode == RemotingMode.Client)
            {
                return await _memberRemote.ReceiveDataAsync<IMediaSourceConfig?>(methodCallToken);
            }
            else
            {
                return null;
            }
        });

        private static void SetupRemoteServices(IServiceCollection clientServices, string remotingId)
        {
            var notificationService = clientServices.FirstOrDefault(x => x.ServiceType == typeof(INotificationService)) as INotificationService;

            if (notificationService != null)
                _ = new RemoteNotificationService(remotingId, notificationService);
        }

        private static IServiceCollection SetupRemoteServices(string remotingId)
        {
            var services = new ServiceCollection();
            var notificationService = new RemoteNotificationService(remotingId);

            services.AddSingleton<RemoteNotificationService>(x => notificationService);

            return services;
        }
    }
}
