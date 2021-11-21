using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    internal class MockCore : ICore
    {
        private List<ICoreDevice> _devices = new List<ICoreDevice>();

        public MockCore(string instanceId)
        {
            SourceCore = this;
            InstanceId = instanceId;

            Library = new MockCoreLibrary(this);
            CoreConfig = new MockCoreConfig(this);
            Pins = new MockCorePins(this);
            RecentlyPlayed = new MockCoreRecentlyPlayed(this);
            Discoverables = new MockCoreDiscoverables(this);
            Search = new MockCoreSearch(this);
        }

        public Task InitAsync(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<CoreState>? CoreStateChanged;

        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        public event EventHandler<string>? InstanceDescriptorChanged;

        public string CoreRegistryId => "MockCore";

        public string InstanceId { get; set; }

        public string InstanceDescriptor { get; set; } = "For testing only";

        public ICoreConfig CoreConfig { get; set; }

        public ICoreUser? User { get; set; } = new MockCoreUser();

        public IReadOnlyList<ICoreDevice> Devices => _devices;

        public ICoreLibrary Library { get; set; }

        public ICorePlayableCollectionGroup? Pins { get; set; }

        public ICoreSearch? Search { get; set; }

        public ICoreRecentlyPlayed? RecentlyPlayed { get; set; }

        public ICoreDiscoverables? Discoverables { get; set; }

        public CoreState CoreState { get; set; }

        public ICore SourceCore { get; set; }

        public Task<ICoreMember?> GetContextById(string id)
        {
            return Task.FromResult<ICoreMember?>(null);
        }

        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            return Task.FromResult<IMediaSourceConfig?>(null);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
