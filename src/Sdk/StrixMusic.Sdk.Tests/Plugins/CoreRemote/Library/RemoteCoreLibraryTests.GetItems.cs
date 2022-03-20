using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    public partial class RemoteCoreLibraryTests
    {
        [TestMethod]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetAlbumItemsAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetAlbumItemsAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetAlbumItemsAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetAlbumItemsAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
        
        [TestMethod, Timeout(5000)]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetArtistItemsAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetArtistItemsAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetArtistItemsAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetArtistItemsAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
        
        [TestMethod, Timeout(5000)]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetPlaylistItemsAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetPlaylistItemsAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetPlaylistItemsAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetPlaylistItemsAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
        
        [TestMethod, Timeout(5000)]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetTracksAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetTracksAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetTracksAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetTracksAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetChildrenAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetChildrenAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetChildrenAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetChildrenAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetImagesAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetImagesAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetImagesAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetImagesAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
        
        [TestMethod, Timeout(5000)]
        [DataRow(1), DataRow(5), DataRow(10), DataRow(50)]
        public async Task RemoteGetUrlsAsync(int limit)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedItem = await core.Library.GetUrlsAsync(limit, 0).ToArrayAsync();

            var remoteItem = await remoteClientCore.Library.GetUrlsAsync(limit, 0).ToArrayAsync();
            var wrappedItem = await remoteHostCore.Library.GetUrlsAsync(limit, 0).ToArrayAsync();

            Assert.AreEqual(limit, expectedItem.Length);
            Assert.AreEqual(limit, remoteItem.Length);
            Assert.AreEqual(limit, wrappedItem.Length);

            Helpers.SmartAssertEqual(expectedItem, remoteItem, recursive: false);
            Helpers.SmartAssertEqual(expectedItem, wrappedItem, recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
