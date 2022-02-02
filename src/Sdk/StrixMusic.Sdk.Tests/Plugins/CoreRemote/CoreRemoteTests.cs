using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;
using System.Threading.Tasks;
using System;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using Microsoft.Extensions.DependencyInjection;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    [TestClass]
    public class CoreRemoteTests
    {
        [ClassInitialize]
        public static void SetupRemoteMessageLoopback(TestContext context)
        {
            RemoteCoreMessageHandler.SingletonClient.MessageOutbound += SingletonClient_MessageOutbound;
            RemoteCoreMessageHandler.SingletonHost.MessageOutbound += SingletonHost_MessageOutbound;
        }

        [ClassCleanup]
        public static void CleanupMessageLoopback()
        {
            RemoteCoreMessageHandler.SingletonClient.MessageOutbound -= SingletonClient_MessageOutbound;
            RemoteCoreMessageHandler.SingletonHost.MessageOutbound -= SingletonHost_MessageOutbound;
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteRegistration()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // Wait for changes to finish
            await Task.Delay(500);

            Assert.AreEqual(core.Registration.DisplayName, remoteHostCore.Registration.DisplayName);
            Assert.AreEqual(core.Registration.DisplayName, remoteClientCore.Registration.DisplayName);

            Assert.AreEqual(core.Registration.Id, remoteHostCore.Registration.Id);
            Assert.AreEqual(core.Registration.Id, remoteClientCore.Registration.Id);

            Assert.AreEqual(core.Registration.LogoUri, remoteHostCore.Registration.LogoUri);
            Assert.AreEqual(core.Registration.LogoUri, remoteClientCore.Registration.LogoUri);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteInitAsync()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync(new ServiceCollection());

            await Task.Delay(200);

            var wrappedResult = remoteHostCore.CoreState;
            var remotelyReceivedResult = remoteClientCore.CoreState;

            Assert.AreEqual(core.CoreState, wrappedResult);
            Assert.AreEqual(core.CoreState, remotelyReceivedResult);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteInstanceDescriptor()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await Task.Delay(200);

            core.InstanceDescriptor = "So remote, much wow.";

            // Wait for changes to propogate
            await Task.Delay(200);

            Assert.AreEqual(core.InstanceDescriptor, remoteHostCore.InstanceDescriptor);
            Assert.AreEqual(core.InstanceDescriptor, remoteClientCore.InstanceDescriptor);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteCoreState()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            core.CoreState = StrixMusic.Sdk.Models.CoreState.NeedsSetup;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.CoreState, remoteHostCore.CoreState);
            Assert.AreEqual(core.CoreState, remoteClientCore.CoreState);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteDevices_InitialSet()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            Assert.AreNotEqual(default, core.Devices.Count);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Devices.Count, remoteHostCore.Devices.Count);
            Assert.AreEqual(core.Devices.Count, remoteClientCore.Devices.Count);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteDevices_Add()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var startingDeviceCount = core.Devices.Count;

            await Task.Delay(500); // Wait for changes to propogate

            core.AddMockDevice();

            await Task.Delay(500); // Wait for changes to propogate

            Assert.AreEqual(startingDeviceCount + 1, core.Devices.Count);
            Assert.AreEqual(startingDeviceCount + 1, remoteHostCore.Devices.Count);
            Assert.AreEqual(startingDeviceCount + 1, remoteClientCore.Devices.Count);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod]
        public async Task RemoteDevices_AddRemove()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var startingDeviceCount = core.Devices.Count;

            await Task.Delay(200);

            core.AddMockDevice();

            await Task.Delay(200);

            core.RemoveMockDevice();

            await Task.Delay(200);

            Assert.AreEqual(startingDeviceCount, core.Devices.Count);
            Assert.AreEqual(startingDeviceCount, remoteHostCore.Devices.Count);
            Assert.AreEqual(startingDeviceCount, remoteClientCore.Devices.Count);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(5000)]
        [DataRow(MockContextIds.Album)]
        [DataRow(MockContextIds.Artist)]
        [DataRow(MockContextIds.Device)]
        [DataRow(MockContextIds.Discoverables)]
        [DataRow(MockContextIds.Image)]
        [DataRow(MockContextIds.Library)]
        [DataRow(MockContextIds.Pins)]
        [DataRow(MockContextIds.PlayableCollectionGroup)]
        [DataRow(MockContextIds.Playlist)]
        [DataRow(MockContextIds.RecentlyPlayed)]
        [DataRow(MockContextIds.SearchHistory)]
        [DataRow(MockContextIds.SearchResults)]
        [DataRow(MockContextIds.Track)]
        public async Task RemoteGetContextById(string id)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var expectedResult = await core.GetContextById(id);
            var wrappedResult = await remoteHostCore.GetContextById(id);
            var remotelyReceivedResult = await remoteClientCore.GetContextById(id);

            // Local core types should not match remote core types.
            Assert.AreNotEqual(expectedResult?.GetType(), wrappedResult?.GetType());
            Assert.AreNotEqual(expectedResult?.GetType(), remotelyReceivedResult?.GetType());

            // Remote core types should match.
            Assert.AreEqual(remotelyReceivedResult?.GetType(), remotelyReceivedResult?.GetType());

            Helpers.SmartAssertEqual(expectedResult, expectedResult?.GetType(), wrappedResult, wrappedResult?.GetType(), recursive: false);
            Helpers.SmartAssertEqual(expectedResult, expectedResult?.GetType(), remotelyReceivedResult, remotelyReceivedResult?.GetType(), recursive: false);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        [DataRow("TestTrack")]
        [DataRow("TestTrack1")]
        public async Task RemoteGetMediaSourceConfig(string id)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            var track = new MockCoreTrack(core, id, id);

            var expectedResult = await core.GetMediaSource(track);
            var wrappedResult = await remoteHostCore.GetMediaSource(track);
            var remotelyReceivedResult = await remoteClientCore.GetMediaSource(track);

            Helpers.SmartAssertEqual(expectedResult, expectedResult?.GetType(), wrappedResult, wrappedResult?.GetType());
            Helpers.SmartAssertEqual(expectedResult, expectedResult?.GetType(), remotelyReceivedResult, remotelyReceivedResult?.GetType());

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteLibrarySetup()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.Id, remoteHostCore.Library.Id);
            Assert.AreEqual(core.Library.Id, remoteClientCore.Library.Id);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteRecentlyPlayedSetup()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.RecentlyPlayed?.Id, remoteHostCore.RecentlyPlayed?.Id);
            Assert.AreEqual(core.RecentlyPlayed?.Id, remoteClientCore.RecentlyPlayed?.Id);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteDiscoverablesSetup()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Discoverables?.Id, remoteHostCore.Discoverables?.Id);
            Assert.AreEqual(core.Discoverables?.Id, remoteClientCore.Discoverables?.Id);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [TestMethod, Timeout(2000)]
        public async Task RemotePinsSetup()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Pins?.Id, remoteHostCore.Pins?.Id);
            Assert.AreEqual(core.Pins?.Id, remoteClientCore.Pins?.Id);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        private static async void SingletonHost_MessageOutbound(object? sender, OwlCore.Remoting.Transfer.IRemoteMessage e)
        {
            // Simulate network conditions.
            await Task.Delay(50);

            RemoteCoreMessageHandler.SingletonClient.DigestMessageAsync(e);
        }

        private static async void SingletonClient_MessageOutbound(object? sender, OwlCore.Remoting.Transfer.IRemoteMessage e)
        {
            // Simulate network conditions.
            await Task.Delay(50);

            RemoteCoreMessageHandler.SingletonHost.DigestMessageAsync(e);
        }
    }
}