using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using StrixMusic.Sdk.Tests.Mock.Core.Library;
using StrixMusic.Sdk.Tests.Mock.Core.Search;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    internal class MockCore : ICore
    {
        private List<ICoreDevice> _devices;
        private string instanceDescriptor = "For testing only";

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

        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(500);

            ((MockCoreLibrary)Library).PopulateMockItems();
        }

        public bool IsInitialized { get; }

        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        public event EventHandler<string>? InstanceDescriptorChanged;
        public event EventHandler<ICoreImage?>? LogoChanged;
        public event EventHandler<string>? DisplayNameChanged;

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
        public MediaPlayerType PlaybackType { get; }

        public ICoreUser? User { get; set; } = new MockCoreUser();

        public IReadOnlyList<ICoreDevice> Devices => _devices;

        public ICoreLibrary Library { get; set; }

        public ICorePlayableCollectionGroup? Pins { get; set; }

        public ICoreSearch? Search { get; set; }

        public ICoreRecentlyPlayed? RecentlyPlayed { get; set; }

        public ICoreDiscoverables? Discoverables { get; set; }

        public ICore SourceCore { get; set; }

        public string Id => throw new NotImplementedException();

        public string DisplayName => throw new NotImplementedException();

        public ICoreImage? Logo => throw new NotImplementedException();

        public Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ICoreModel?>(id switch
            {
                MockContextIds.Album => new MockCoreAlbum(this, id, "Album"),
                MockContextIds.Artist => new MockCoreArtist(this, id, "Artist"),
                MockContextIds.Device => new MockCoreDevice(this),
                MockContextIds.Discoverables => new MockCoreDiscoverables(this),
                MockContextIds.Image => new MockCoreImage(this),
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

        public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default)
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

        public ValueTask DisposeAsync() => default;
    }
}
