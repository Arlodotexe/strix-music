using Microsoft.Extensions.DependencyInjection;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Data.Core
{
    internal class MockCore : ICore
    {
        public string CoreRegistryId => throw new NotImplementedException();

        public string InstanceId =>"MockCore";

        public string InstanceDescriptor => throw new NotImplementedException();

        public ICoreConfig CoreConfig => throw new NotImplementedException();

        public ICoreUser User => throw new NotImplementedException();

        public IReadOnlyList<ICoreDevice> Devices => throw new NotImplementedException();

        public ICoreLibrary Library => throw new NotImplementedException();

        public ICorePlayableCollectionGroup Pins => throw new NotImplementedException();

        public ICoreSearch Search => throw new NotImplementedException();

        public ICoreRecentlyPlayed RecentlyPlayed => throw new NotImplementedException();

        public ICoreDiscoverables Discoverables => throw new NotImplementedException();

        public CoreState CoreState => throw new NotImplementedException();

        public ICore SourceCore => throw new NotImplementedException();

        public event EventHandler<CoreState> CoreStateChanged;
        public event CollectionChangedEventHandler<ICoreDevice> DevicesChanged;
        public event EventHandler<string> InstanceDescriptorChanged;

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICoreMember> GetContextById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IMediaSourceConfig> GetMediaSource(ICoreTrack track)
        {
            throw new NotImplementedException();
        }

        public Task InitAsync(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
