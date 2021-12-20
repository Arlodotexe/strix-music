﻿using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task RemoteName()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.Name);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.Name, remoteClientCore.Library.Name);
            Assert.AreEqual(core.Library.Name, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow("Name"), DataRow("Name1")]
        [TestMethod, Timeout(2000)]
        public async Task RemoteName_Changed(string name)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.Name, name);

            core.Library.Cast<MockCoreLibrary>().Name = name;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(name, core.Library.Name);
            Assert.AreEqual(name, remoteClientCore.Library.Name);
            Assert.AreEqual(name, remoteHostCore.Library.Name);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePlaybackState()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.PlaybackState);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.PlaybackState, remoteClientCore.Library.PlaybackState);
            Assert.AreEqual(core.Library.PlaybackState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(PlaybackState.Loading), DataRow(PlaybackState.Playing), DataRow(PlaybackState.Failed), DataRow(PlaybackState.Queued)]
        [TestMethod, Timeout(2000)]
        public async Task RemotePlaybackState_Changed(PlaybackState playbackState)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be same as current.
            Assert.AreNotEqual(core.Library.PlaybackState, playbackState);

            core.Library.Cast<MockCoreLibrary>().PlaybackState = playbackState;

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(playbackState, core.Library.PlaybackState);
            Assert.AreEqual(playbackState, remoteClientCore.Library.PlaybackState);
            Assert.AreEqual(playbackState, remoteHostCore.Library.PlaybackState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteLastPlayed()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.LastPlayed);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.LastPlayed, remoteClientCore.Library.LastPlayed);
            Assert.AreEqual(core.Library.LastPlayed, remoteHostCore.Library.LastPlayed);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
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
            await Task.Delay(100);

            Assert.AreEqual(newLastPlayed, core.Library.LastPlayed);
            Assert.AreEqual(newLastPlayed, remoteClientCore.Library.LastPlayed);
            Assert.AreEqual(newLastPlayed, remoteHostCore.Library.LastPlayed);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteDuration()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.Duration);

            // Wait for changes to propogate
            await Task.Delay(100);

            Assert.AreEqual(core.Library.Duration, remoteClientCore.Library.Duration);
            Assert.AreEqual(core.Library.Duration, remoteHostCore.Library.Duration);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
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
            await Task.Delay(100);

            Assert.AreEqual(newDuration, core.Library.Duration);
            Assert.AreEqual(newDuration, remoteClientCore.Library.Duration);
            Assert.AreEqual(newDuration, remoteHostCore.Library.Duration);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }
    }
}
