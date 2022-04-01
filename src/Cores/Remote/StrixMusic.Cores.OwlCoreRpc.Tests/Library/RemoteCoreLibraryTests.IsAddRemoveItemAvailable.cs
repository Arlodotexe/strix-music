using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Cores.OwlCoreRpc.Tests.Mock;

namespace StrixMusic.Cores.OwlCoreRpc.Tests
{
    public partial class RemoteCoreLibraryTests
    {
        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddChildAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddChildAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddChildAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddChildAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddAlbumItemAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddAlbumItemAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddAlbumItemAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddAlbumItemAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddArtistItemAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddArtistItemAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddArtistItemAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddArtistItemAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddTrackAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddTrackAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddTrackAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddTrackAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddPlaylistItemAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddPlaylistItemAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddPlaylistItemAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddPlaylistItemAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddImageAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddImageAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddImageAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddImageAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsAddUrlAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsAddUrlAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsAddUrlAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsAddUrlAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemoveChildAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemoveChildAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemoveChildAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemoveChildAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemoveAlbumItemAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemoveAlbumItemAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemoveAlbumItemAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemoveAlbumItemAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemoveArtistItemAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemoveArtistItemAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemoveArtistItemAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemoveArtistItemAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemoveTrackAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemoveTrackAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemoveTrackAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemoveTrackAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemovePlaylistItemAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemovePlaylistItemAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemovePlaylistItemAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemovePlaylistItemAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemoveImageAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemoveImageAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemoveImageAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemoveImageAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(2), DataRow(5), DataRow(10), DataRow(99)]
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsRemoveUrlAvailableAsync(int index)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var expectedValue = await core.Library.IsRemoveUrlAvailableAsync(index);

            var wrappedValue = await remoteHostCore.Library.IsRemoveUrlAvailableAsync(index);
            var remoteValue = await remoteClientCore.Library.IsRemoveUrlAvailableAsync(index);

            Assert.AreEqual(expectedValue, wrappedValue);
            Assert.AreEqual(expectedValue, remoteValue);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
