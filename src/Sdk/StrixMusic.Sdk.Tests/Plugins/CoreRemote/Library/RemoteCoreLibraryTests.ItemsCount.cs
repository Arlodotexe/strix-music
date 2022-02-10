using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Library;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    public partial class RemoteCoreLibraryTests
    {
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalTrackCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalTrackCount);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.TotalTrackCount, remoteClientCore.Library.TotalTrackCount);
            Assert.AreEqual(core.Library.TotalTrackCount, remoteHostCore.Library.TotalTrackCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalTrackCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.TotalTrackCount, amount);

            core.Library.Cast<MockCoreLibrary>().TotalTrackCount = amount;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(amount, core.Library.TotalTrackCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalTrackCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalTrackCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalAlbumItemsCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalAlbumItemsCount);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.TotalAlbumItemsCount, remoteClientCore.Library.TotalAlbumItemsCount);
            Assert.AreEqual(core.Library.TotalAlbumItemsCount, remoteHostCore.Library.TotalAlbumItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalAlbumItemsCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.TotalAlbumItemsCount, amount);

            core.Library.Cast<MockCoreLibrary>().TotalAlbumItemsCount = amount;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(amount, core.Library.TotalAlbumItemsCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalAlbumItemsCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalAlbumItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalPlaylistItemsCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalPlaylistItemsCount);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.TotalPlaylistItemsCount, remoteClientCore.Library.TotalPlaylistItemsCount);
            Assert.AreEqual(core.Library.TotalPlaylistItemsCount, remoteHostCore.Library.TotalPlaylistItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalPlaylistItemsCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.TotalPlaylistItemsCount, amount);

            core.Library.Cast<MockCoreLibrary>().TotalPlaylistItemsCount = amount;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(amount, core.Library.TotalPlaylistItemsCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalPlaylistItemsCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalPlaylistItemsCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalChildrenCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalChildrenCount);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.TotalChildrenCount, remoteClientCore.Library.TotalChildrenCount);
            Assert.AreEqual(core.Library.TotalChildrenCount, remoteHostCore.Library.TotalChildrenCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalChildrenCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.TotalChildrenCount, amount);

            core.Library.Cast<MockCoreLibrary>().TotalChildrenCount = amount;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(amount, core.Library.TotalChildrenCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalChildrenCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalChildrenCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalImageCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalImageCount);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.TotalImageCount, remoteClientCore.Library.TotalImageCount);
            Assert.AreEqual(core.Library.TotalImageCount, remoteHostCore.Library.TotalImageCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalImageCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.TotalImageCount, amount);

            core.Library.Cast<MockCoreLibrary>().TotalImageCount = amount;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(amount, core.Library.TotalImageCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalImageCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalImageCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalUrlCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalUrlCount);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.TotalUrlCount, remoteClientCore.Library.TotalUrlCount);
            Assert.AreEqual(core.Library.TotalUrlCount, remoteHostCore.Library.TotalUrlCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteTotalUrlCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.TotalUrlCount, amount);

            core.Library.Cast<MockCoreLibrary>().TotalUrlCount = amount;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(amount, core.Library.TotalUrlCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalUrlCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalUrlCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
