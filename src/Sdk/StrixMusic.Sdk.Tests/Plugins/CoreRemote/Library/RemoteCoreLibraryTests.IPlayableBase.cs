using System;
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
        [TestMethod, Timeout(5000)]
        public async Task RemoteName()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.Name);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.Name, remoteClientCore.Library.Name);
            Assert.AreEqual(core.Library.Name, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow("Name"), DataRow("Name1")]
        [TestMethod, Timeout(5000)]
        public async Task RemoteName_Changed(string name)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.Name, name);

            core.Library.Cast<MockCoreLibrary>().Name = name;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(name, core.Library.Name);
            Assert.AreEqual(name, remoteClientCore.Library.Name);
            Assert.AreEqual(name, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemotePlaybackState()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.PlaybackState);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.PlaybackState, remoteClientCore.Library.PlaybackState);
            Assert.AreEqual(core.Library.PlaybackState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(PlaybackState.Failed), DataRow(PlaybackState.Playing), DataRow(PlaybackState.Paused), DataRow(PlaybackState.Loading)]
        [TestMethod, Timeout(5000)]
        public async Task RemotePlaybackState_Changed(PlaybackState playbackState)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.PlaybackState, playbackState);

            core.Library.Cast<MockCoreLibrary>().PlaybackState = playbackState;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(playbackState, core.Library.PlaybackState);
            Assert.AreEqual(playbackState, remoteClientCore.Library.PlaybackState);
            Assert.AreEqual(playbackState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteLastPlayed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.LastPlayed);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.LastPlayed, remoteClientCore.Library.LastPlayed);
            Assert.AreEqual(core.Library.LastPlayed, remoteHostCore.Library.LastPlayed);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteLastPlayed_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newLastPlayed = DateTime.UtcNow;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.LastPlayed, newLastPlayed);

            core.Library.Cast<MockCoreLibrary>().LastPlayed = newLastPlayed;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newLastPlayed, core.Library.LastPlayed);
            Assert.AreEqual(newLastPlayed, remoteClientCore.Library.LastPlayed);
            Assert.AreEqual(newLastPlayed, remoteHostCore.Library.LastPlayed);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteDuration()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.Duration);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.Duration, remoteClientCore.Library.Duration);
            Assert.AreEqual(core.Library.Duration, remoteHostCore.Library.Duration);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteDuration_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newDuration = TimeSpan.MaxValue;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.Duration, newDuration);

            core.Library.Cast<MockCoreLibrary>().Duration = newDuration;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newDuration, core.Library.Duration);
            Assert.AreEqual(newDuration, remoteClientCore.Library.Duration);
            Assert.AreEqual(newDuration, remoteHostCore.Library.Duration);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsChangeNameAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsChangeNameAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.IsChangeNameAsyncAvailable, remoteClientCore.Library.IsChangeNameAsyncAvailable);
            Assert.AreEqual(core.Library.IsChangeNameAsyncAvailable, remoteHostCore.Library.IsChangeNameAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsChangeNameAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newIsChangeNameAsyncAvailable = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsChangeNameAsyncAvailable, newIsChangeNameAsyncAvailable);

            core.Library.Cast<MockCoreLibrary>().IsChangeNameAsyncAvailable = newIsChangeNameAsyncAvailable;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newIsChangeNameAsyncAvailable, core.Library.IsChangeNameAsyncAvailable);
            Assert.AreEqual(newIsChangeNameAsyncAvailable, remoteClientCore.Library.IsChangeNameAsyncAvailable);
            Assert.AreEqual(newIsChangeNameAsyncAvailable, remoteHostCore.Library.IsChangeNameAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsChangeDescriptionAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsChangeDescriptionAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.IsChangeDescriptionAsyncAvailable, remoteClientCore.Library.IsChangeDescriptionAsyncAvailable);
            Assert.AreEqual(core.Library.IsChangeDescriptionAsyncAvailable, remoteHostCore.Library.IsChangeDescriptionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsChangeDescriptionAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newIsChangeDescriptionAsyncAvailable = false;

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsChangeDescriptionAsyncAvailable, newIsChangeDescriptionAsyncAvailable);

            core.Library.Cast<MockCoreLibrary>().IsChangeDescriptionAsyncAvailable = newIsChangeDescriptionAsyncAvailable;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newIsChangeDescriptionAsyncAvailable, core.Library.IsChangeDescriptionAsyncAvailable);
            Assert.AreEqual(newIsChangeDescriptionAsyncAvailable, remoteClientCore.Library.IsChangeDescriptionAsyncAvailable);
            Assert.AreEqual(newIsChangeDescriptionAsyncAvailable, remoteHostCore.Library.IsChangeDescriptionAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsChangeDurationAsyncAvailable()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.IsChangeDurationAsyncAvailable);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.IsChangeDurationAsyncAvailable, remoteClientCore.Library.IsChangeDurationAsyncAvailable);
            Assert.AreEqual(core.Library.IsChangeDurationAsyncAvailable, remoteHostCore.Library.IsChangeDurationAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteIsChangeDurationAsyncAvailable_Changed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newIsChangeDurationAsyncAvailable = false;

            await remoteClientCore.InitAsync(new ServiceCollection());

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.IsChangeDurationAsyncAvailable, newIsChangeDurationAsyncAvailable);

            core.Library.Cast<MockCoreLibrary>().IsChangeDurationAsyncAvailable = newIsChangeDurationAsyncAvailable;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newIsChangeDurationAsyncAvailable, core.Library.IsChangeDurationAsyncAvailable);
            Assert.AreEqual(newIsChangeDurationAsyncAvailable, remoteClientCore.Library.IsChangeDurationAsyncAvailable);
            Assert.AreEqual(newIsChangeDurationAsyncAvailable, remoteHostCore.Library.IsChangeDurationAsyncAvailable);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteChangeName()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newName = "NewName";

            await remoteClientCore.InitAsync(new ServiceCollection());

            // For test to work, must not be a default value or the new value.
            Assert.AreNotEqual(default, core.Library.Name);
            Assert.AreNotEqual(newName, core.Library.Name);

            await remoteClientCore.Library.ChangeNameAsync(newName);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newName, remoteClientCore.Library.Name);
            Assert.AreEqual(newName, remoteHostCore.Library.Name);
            Assert.AreEqual(newName, core.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteChangeDescription()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newDescription = "NewDescription";

            await remoteClientCore.InitAsync(new ServiceCollection());

            // For test to work, must not be a default value or the new value.
            Assert.AreNotEqual(default, core.Library.Description);
            Assert.AreNotEqual(newDescription, core.Library.Description);

            await remoteClientCore.Library.ChangeDescriptionAsync(newDescription);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newDescription, remoteClientCore.Library.Description);
            Assert.AreEqual(newDescription, remoteHostCore.Library.Description);
            Assert.AreEqual(newDescription, core.Library.Description);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        public async Task RemoteChangeDuration()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var newDuration = TimeSpan.FromMilliseconds(100);

            await remoteClientCore.InitAsync(new ServiceCollection());

            // For test to work, must not be a default value or the new value.
            Assert.AreNotEqual(default, core.Library.Duration);
            Assert.AreNotEqual(newDuration, core.Library.Duration);

            await remoteClientCore.Library.ChangeDurationAsync(newDuration);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(newDuration, remoteClientCore.Library.Duration);
            Assert.AreEqual(newDuration, remoteHostCore.Library.Duration);
            Assert.AreEqual(newDuration, core.Library.Duration);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
