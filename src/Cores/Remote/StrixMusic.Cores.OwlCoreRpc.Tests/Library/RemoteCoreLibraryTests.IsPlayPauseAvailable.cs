using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Cores.OwlCoreRpc.Tests.Mock;
using StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Library;

namespace StrixMusic.Cores.OwlCoreRpc.Tests
{
    public partial class RemoteCoreLibraryTests
    {
        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayAlbumCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPlayAlbumCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPlayAlbumCollectionAsyncAvailable, remoteClientCore.Library.IsPlayAlbumCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPlayAlbumCollectionAsyncAvailable, remoteHostCore.Library.IsPlayAlbumCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayAlbumCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPlayAlbumCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPlayAlbumCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPlayAlbumCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPlayAlbumCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPlayAlbumCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPauseAlbumCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPauseAlbumCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPauseAlbumCollectionAsyncAvailable, remoteClientCore.Library.IsPauseAlbumCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPauseAlbumCollectionAsyncAvailable, remoteHostCore.Library.IsPauseAlbumCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPauseAlbumCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPauseAlbumCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPauseAlbumCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPauseAlbumCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPauseAlbumCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPauseAlbumCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayArtistCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPlayArtistCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPlayArtistCollectionAsyncAvailable, remoteClientCore.Library.IsPlayArtistCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPlayArtistCollectionAsyncAvailable, remoteHostCore.Library.IsPlayArtistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayArtistCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPlayArtistCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPlayArtistCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPlayArtistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPlayArtistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPlayArtistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPauseArtistCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPauseArtistCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPauseArtistCollectionAsyncAvailable, remoteClientCore.Library.IsPauseArtistCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPauseArtistCollectionAsyncAvailable, remoteHostCore.Library.IsPauseArtistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPauseArtistCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPauseArtistCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPauseArtistCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPauseArtistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPauseArtistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPauseArtistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayPlaylistCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPlayPlaylistCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPlayPlaylistCollectionAsyncAvailable, remoteClientCore.Library.IsPlayPlaylistCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPlayPlaylistCollectionAsyncAvailable, remoteHostCore.Library.IsPlayPlaylistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayPlaylistCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPlayPlaylistCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPlayPlaylistCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPlayPlaylistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPlayPlaylistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPlayPlaylistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPausePlaylistCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPausePlaylistCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPausePlaylistCollectionAsyncAvailable, remoteClientCore.Library.IsPausePlaylistCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPausePlaylistCollectionAsyncAvailable, remoteHostCore.Library.IsPausePlaylistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPausePlaylistCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPausePlaylistCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPausePlaylistCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPausePlaylistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPausePlaylistCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPausePlaylistCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayTrackCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPlayTrackCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPlayTrackCollectionAsyncAvailable, remoteClientCore.Library.IsPlayTrackCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPlayTrackCollectionAsyncAvailable, remoteHostCore.Library.IsPlayTrackCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPlayTrackCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPlayTrackCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPlayTrackCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPlayTrackCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPlayTrackCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPlayTrackCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPauseTrackCollectionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsPauseTrackCollectionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.IsPauseTrackCollectionAsyncAvailable, remoteClientCore.Library.IsPauseTrackCollectionAsyncAvailable);
            Assert.AreEqual(core.Library.IsPauseTrackCollectionAsyncAvailable, remoteHostCore.Library.IsPauseTrackCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsPauseTrackCollectionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newValue = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsPauseTrackCollectionAsyncAvailable, newValue);

            core.Library.Cast<MockCoreLibrary>().IsPauseTrackCollectionAsyncAvailable = newValue;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(newValue, core.Library.IsPauseTrackCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteClientCore.Library.IsPauseTrackCollectionAsyncAvailable);
            Assert.AreEqual(newValue, remoteHostCore.Library.IsPauseTrackCollectionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
