using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Cores.Remote.OwlCore.Tests.Mock.Items;
using StrixMusic.Cores.Remote.OwlCore.Tests.Mock.Library;
using StrixMusic.Cores.Remote.OwlCore.Tests.Mock.Search;

namespace StrixMusic.Cores.Remote.OwlCore.Tests.Mock
{
    internal class MockCore : ICore
    {
        private List<ICoreDevice> _devices;
        private string instanceDescriptor = "For testing only";
        private CoreState coreState;

        public MockCore()
        {
            SourceCore = this;
            InstanceId = Guid.NewGuid().ToString();

            Library = new MockCoreLibrary(this);
            Pins = new MockCorePins(this);
            RecentlyPlayed = new MockCoreRecentlyPlayed(this);
            Discoverables = new MockCoreDiscoverables(this);
            Search = new MockCoreSearch(this);

            _devices = new List<ICoreDevice>()
            {
                new MockCoreDevice(SourceCore),
            };
        }

        public async Task InitAsync()
        {
            await Task.Delay(500);

            Library.Cast<MockCoreLibrary>().PopulateMockItems();

            CoreState = CoreState.Loaded;
        }

        public bool IsInitialized { get; }

        public event EventHandler<CoreState>? CoreStateChanged;

        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler? AbstractConfigPanelChanged;

        public event EventHandler<string>? InstanceDescriptorChanged;

        public CoreMetadata Registration { get; } = new CoreMetadata(nameof(MockCore), "Mock core", new Uri("https://strixmusic.com/"), Version.Parse("0.0.0.0"));

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

        /// <inheritdoc />
        public AbstractUICollection AbstractConfigPanel { get; } = new("test");

        /// <inheritdoc />
        public MediaPlayerType PlaybackType { get; }

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
                MockContextIds.Pins => Pins,
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
            return Task.FromResult<IMediaSourceConfig?>(new MockMediaSourceConfig(track.Id, track));
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
