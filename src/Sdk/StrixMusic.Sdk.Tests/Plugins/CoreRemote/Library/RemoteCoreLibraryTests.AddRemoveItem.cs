using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    public partial class RemoteCoreLibraryTests
    {
        [TestMethod, Timeout(5000)]
        public async Task RemoteAddChildAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalChildrenCount;

            await remoteClientCore.Library.AddChildAsync(remoteClientCore.Library, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalChildrenCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalChildrenCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteAddAlbumAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalAlbumItemsCount;

            var item = MockCoreItemFactory.CreateAlbum(remoteClientCore);

            await remoteClientCore.Library.AddAlbumItemAsync(item, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalAlbumItemsCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalAlbumItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteAddArtistAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalArtistItemsCount;

            var item = MockCoreItemFactory.CreateArtist(remoteClientCore);

            await remoteClientCore.Library.AddArtistItemAsync(item, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalArtistItemsCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalArtistItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteAddPlaylistAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalPlaylistItemsCount;

            var item = MockCoreItemFactory.CreatePlaylist(remoteClientCore);

            await remoteClientCore.Library.AddPlaylistItemAsync(item, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalPlaylistItemsCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalPlaylistItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteAddTrackAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalTrackCount;

            var item = MockCoreItemFactory.CreateTrack(remoteClientCore);

            await remoteClientCore.Library.AddTrackAsync(item, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalTrackCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalTrackCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteAddImageAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalImageCount;

            var item = MockCoreItemFactory.CreateImage(remoteClientCore);

            await remoteClientCore.Library.AddImageAsync(item, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalImageCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalImageCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteAddUrlAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalUrlCount;

            var item = MockCoreItemFactory.CreateUrl(remoteClientCore);

            await remoteClientCore.Library.AddUrlAsync(item, 0);

            Assert.AreEqual(initialCount + 1, core.Library.TotalUrlCount);
            Assert.AreEqual(initialCount + 1, remoteHostCore.Library.TotalUrlCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemoveChildAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalChildrenCount;

            await remoteClientCore.Library.RemoveChildAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalChildrenCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalChildrenCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemoveAlbumAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalAlbumItemsCount;

            await remoteClientCore.Library.RemoveAlbumItemAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalAlbumItemsCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalAlbumItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemoveArtistAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalArtistItemsCount;

            await remoteClientCore.Library.RemoveArtistItemAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalArtistItemsCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalArtistItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemovePlaylistAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalPlaylistItemsCount;

            await remoteClientCore.Library.RemovePlaylistItemAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalPlaylistItemsCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalPlaylistItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemoveTrackAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalTrackCount;

            await remoteClientCore.Library.RemoveTrackAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalTrackCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalTrackCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemoveImageAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalImageCount;

            await remoteClientCore.Library.RemoveImageAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalImageCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalImageCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteRemoveUrlAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var initialCount = remoteClientCore.Library.TotalUrlCount;

            await remoteClientCore.Library.RemoveUrlAsync(0);

            Assert.AreEqual(initialCount - 1, core.Library.TotalUrlCount);
            Assert.AreEqual(initialCount - 1, remoteHostCore.Library.TotalUrlCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
