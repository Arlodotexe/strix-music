using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Library;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    public partial class RemoteCoreLibraryTests
    {
        [TestMethod, Timeout(2000)]
        public async Task RemotePauseAlbumCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Paused;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PauseAlbumCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayAlbumCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PlayAlbumCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayAlbumCollectionAsync_AtItem()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var item = MockItemFactory.CreateAlbum(remoteClientCore);

            var initialPlaybackState = remoteClientCore.Library.PlaybackState;
            var expectedPlaybackState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedPlaybackState, initialPlaybackState);

            var initialName = remoteClientCore.Library.Name;
            var expectedName = item.Name;
            Assert.AreNotEqual(expectedName, initialName);

            await remoteClientCore.Library.PlayAlbumCollectionAsync(item);

            Assert.AreEqual(expectedPlaybackState, core.Library.PlaybackState);
            Assert.AreEqual(expectedPlaybackState, remoteHostCore.Library.PlaybackState);

            Assert.AreEqual(expectedName, core.Library.Name);
            Assert.AreEqual(expectedName, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePauseArtistCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Paused;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PauseArtistCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayArtistCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PlayArtistCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayArtistCollectionAsync_AtItem()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var item = MockItemFactory.CreateArtist(remoteClientCore);

            var initialPlaybackState = remoteClientCore.Library.PlaybackState;
            var expectedPlaybackState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedPlaybackState, initialPlaybackState);

            var initialName = remoteClientCore.Library.Name;
            var expectedName = item.Name;
            Assert.AreNotEqual(expectedName, initialName);

            await remoteClientCore.Library.PlayArtistCollectionAsync(item);

            Assert.AreEqual(expectedPlaybackState, core.Library.PlaybackState);
            Assert.AreEqual(expectedPlaybackState, remoteHostCore.Library.PlaybackState);

            Assert.AreEqual(expectedName, core.Library.Name);
            Assert.AreEqual(expectedName, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePausePlaylistCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Paused;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PausePlaylistCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayPlaylistCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PlayPlaylistCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayPlaylistCollectionAsync_AtItem()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var item = MockItemFactory.CreatePlaylist(remoteClientCore);

            var initialPlaybackState = remoteClientCore.Library.PlaybackState;
            var expectedPlaybackState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedPlaybackState, initialPlaybackState);

            var initialName = remoteClientCore.Library.Name;
            var expectedName = item.Name;
            Assert.AreNotEqual(expectedName, initialName);

            await remoteClientCore.Library.PlayPlaylistCollectionAsync(item);

            Assert.AreEqual(expectedPlaybackState, core.Library.PlaybackState);
            Assert.AreEqual(expectedPlaybackState, remoteHostCore.Library.PlaybackState);

            Assert.AreEqual(expectedName, core.Library.Name);
            Assert.AreEqual(expectedName, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePauseTrackCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Paused;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PauseTrackCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayTrackCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PlayTrackCollectionAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayTrackCollectionAsync_AtItem()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var item = MockItemFactory.CreateTrack(remoteClientCore);

            var initialPlaybackState = remoteClientCore.Library.PlaybackState;
            var expectedPlaybackState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedPlaybackState, initialPlaybackState);

            var initialName = remoteClientCore.Library.Name;
            var expectedName = item.Name;
            Assert.AreNotEqual(expectedName, initialName);

            await remoteClientCore.Library.PlayTrackCollectionAsync(item);

            Assert.AreEqual(expectedPlaybackState, core.Library.PlaybackState);
            Assert.AreEqual(expectedPlaybackState, remoteHostCore.Library.PlaybackState);

            Assert.AreEqual(expectedName, core.Library.Name);
            Assert.AreEqual(expectedName, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePausePlayableCollectionGroupCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Paused;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PausePlayableCollectionGroupAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayPlayableCollectionGroupCollectionAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var initialState = remoteClientCore.Library.PlaybackState;
            var expectedState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedState, initialState);

            await remoteClientCore.Library.PlayPlayableCollectionGroupAsync();

            Assert.AreEqual(expectedState, core.Library.PlaybackState);
            Assert.AreEqual(expectedState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlayPlayableCollectionGroupCollectionAsync_AtItem()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            var item = MockItemFactory.CreatePlayableCollectionGroup(remoteClientCore);

            var initialPlaybackState = item.PlaybackState;
            var expectedPlaybackState = PlaybackState.Playing;
            Assert.AreNotEqual(expectedPlaybackState, initialPlaybackState);

            var initialName = remoteClientCore.Library.Name;
            var expectedName = item.Name;
            Assert.AreNotEqual(expectedName, initialName);

            await remoteClientCore.Library.PlayPlayableCollectionGroupAsync(item);

            Assert.AreEqual(expectedPlaybackState, core.Library.PlaybackState);
            Assert.AreEqual(expectedPlaybackState, remoteHostCore.Library.PlaybackState);

            Assert.AreEqual(expectedName, core.Library.Name);
            Assert.AreEqual(expectedName, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
