using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using StrixMusic.Sdk.Tests.Mock.Core.Library;
using StrixMusic.Sdk.Tests.Mock.Core.Search;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    internal class MockCore : ICore
    {
        private List<ICoreDevice> _devices = new List<ICoreDevice>();
        private string instanceDescriptor = "For testing only";
        private CoreState coreState;

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

        public async Task InitAsync(IServiceCollection services)
        {
            await Task.Delay(100);
        }

        public event EventHandler<CoreState>? CoreStateChanged;

        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        public event EventHandler<string>? InstanceDescriptorChanged;

        public CoreMetadata Registration { get; } = new CoreMetadata(nameof(MockCore), "Mock core", new Uri("https://strixmusic.com/"));

        public string InstanceId { get; set; }

        public string InstanceDescriptor
        {
            get => instanceDescriptor;
            set
            {
                instanceDescriptor = value;
                InstanceDescriptorChanged?.Invoke(this, value);
            }
        }

        public ICoreConfig CoreConfig { get; set; }

        public ICoreUser? User { get; set; } = new MockCoreUser();

        public IReadOnlyList<ICoreDevice> Devices => _devices;

        public ICoreLibrary Library { get; set; }

        public ICorePlayableCollectionGroup? Pins { get; set; }

        public ICoreSearch? Search { get; set; }

        public ICoreRecentlyPlayed? RecentlyPlayed { get; set; }

        public ICoreDiscoverables? Discoverables { get; set; }

        public CoreState CoreState
        {
            get => coreState;
            set
            {
                coreState = value;
                CoreStateChanged?.Invoke(this, value);
            }
        }

        public ICore SourceCore { get; set; }

        public Task<ICoreMember?> GetContextById(string id)
        {
            return Task.FromResult<ICoreMember?>(id switch
            {
                MockContextIds.Album => new MockCoreAlbum(this, id, "Album"),
                MockContextIds.Artist => new MockCoreArtist(this, id, "Artist"),
                MockContextIds.Device => new MockCoreDevice(this),
                MockContextIds.Discoverables => new MockCoreDiscoverables(this),
                MockContextIds.Image => new MockCoreImage(this, new Uri("https://strixmusic.com/favicon.ico")),
                MockContextIds.Library => Library,
                MockContextIds.Pins =>  Pins,
                MockContextIds.PlayableCollectionGroup => new MockCorePlayableCollectionGroup(this, id, "Collection group"),
                MockContextIds.Playlist => new MockCorePlaylist(this, id, "Playlist"),
                MockContextIds.RecentlyPlayed => RecentlyPlayed,
                MockContextIds.SearchHistory => Search?.SearchHistory,
                MockContextIds.SearchResults => new MockCoreSearchResults(this, id),
                MockContextIds.Track => new MockCoreTrack(this, id, "Track"),
                _ => throw new NotImplementedException(),
            });
        }
        
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            return Task.FromResult<IMediaSourceConfig?>(null);
        }

        public void AddMockDevice()
        {
            var newDevice = new MockCoreDevice(SourceCore);
            _devices.Add(newDevice);

            DevicesChanged?.Invoke(this,
                new CollectionChangedItem<ICoreDevice>(newDevice, _devices.Count - 1).IntoList(),
                new List<CollectionChangedItem<ICoreDevice>>());
        }

        public void RemoveMockDevice()
        {
            var device = _devices[0];
            _devices.RemoveAt(0);

            DevicesChanged?.Invoke(this,
                new List<CollectionChangedItem<ICoreDevice>>(),
                new CollectionChangedItem<ICoreDevice>(device, 0).IntoList());
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
