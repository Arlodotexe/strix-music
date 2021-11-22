using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;
using System.Threading.Tasks;
using System;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    [TestClass]
    public class CoreRemoteTests
    {
        private MockCore? _core;
        private RemoteCore? _remoteClientCore;
        private RemoteCore? _remoteHostCore;

        [TestInitialize]
        public void SetupRemoteMessageLoopback()
        {
            RemoteCoreMessageHandler.SingletonClient.MessageOutbound += SingletonClient_MessageOutbound;
            RemoteCoreMessageHandler.SingletonHost.MessageOutbound += SingletonHost_MessageOutbound;

            _core = new MockCore(nameof(MockCore));

            // Set up for receiving.
            _remoteClientCore = new RemoteCore(_core.InstanceId);

            // Wrap around the actual core
            _remoteHostCore = new RemoteCore(_core);
        }

        [TestCleanup]
        public void CleanupMessageLoopback()
        {
            Assert.IsNotNull(_core);
            Assert.IsNotNull(_remoteClientCore);
            Assert.IsNotNull(_remoteHostCore);

            RemoteCoreMessageHandler.SingletonClient.MessageOutbound -= SingletonClient_MessageOutbound;
            RemoteCoreMessageHandler.SingletonHost.MessageOutbound -= SingletonHost_MessageOutbound;

            _core.DisposeAsync();
            _remoteHostCore.DisposeAsync();
            _remoteClientCore.DisposeAsync();

            _core = null;
            _remoteHostCore = null;
            _remoteClientCore = null;
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteRegistration()
        {
            Assert.IsNotNull(_core);
            Assert.IsNotNull(_remoteClientCore);
            Assert.IsNotNull(_remoteHostCore);

            // Simulate network conditions.
            await Task.Delay(200);

            Assert.AreEqual(_core.Registration.DisplayName, _remoteHostCore.Registration.DisplayName);
            Assert.AreEqual(_core.Registration.DisplayName, _remoteClientCore.Registration.DisplayName);

            Assert.AreEqual(_core.Registration.Id, _remoteHostCore.Registration.Id);
            Assert.AreEqual(_core.Registration.Id, _remoteClientCore.Registration.Id );

            Assert.AreEqual(_core.Registration.LogoUri, _remoteHostCore.Registration.LogoUri);
            Assert.AreEqual(_core.Registration.LogoUri, _remoteClientCore.Registration.LogoUri);
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteInstanceDescriptor()
        {
            Assert.IsNotNull(_core);
            Assert.IsNotNull(_remoteClientCore);
            Assert.IsNotNull(_remoteHostCore);

            _core.InstanceDescriptor = "So remote, much wow.";

            // Simulate network conditions.
            await Task.Delay(200);

            Assert.AreEqual(_core.InstanceDescriptor, _remoteHostCore.InstanceDescriptor);
            Assert.AreEqual(_core.InstanceDescriptor, _remoteClientCore.InstanceDescriptor);
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteCoreState()
        {
            Assert.IsNotNull(_core);
            Assert.IsNotNull(_remoteClientCore);
            Assert.IsNotNull(_remoteHostCore);

            _core.CoreState = Data.CoreState.NeedsSetup;

            // Simulate network conditions.
            await Task.Delay(200);

            Assert.AreEqual(_core.CoreState, _remoteHostCore.CoreState);
            Assert.AreEqual(_core.CoreState, _remoteClientCore.CoreState);
        }

        [TestMethod, Timeout(2000)]
        public async Task RemoteDevices_Add()
        {
            Assert.IsNotNull(_core);
            Assert.IsNotNull(_remoteClientCore);
            Assert.IsNotNull(_remoteHostCore);

            var startingDeviceCount = _core.Devices.Count;

            _core.AddMockDevice();

            // Simulate network conditions.
            await Task.Delay(50);

            Assert.AreEqual(startingDeviceCount + 1, _core.Devices.Count);
            Assert.AreEqual(startingDeviceCount + 1, _remoteHostCore.Devices.Count);
            Assert.AreEqual(startingDeviceCount + 1, _remoteClientCore.Devices.Count);
        }

        [TestMethod, Timeout(1000)]
        public async Task RemoteDevices_AddRemove()
        {
            Assert.IsNotNull(_core);
            Assert.IsNotNull(_remoteClientCore);
            Assert.IsNotNull(_remoteHostCore);

            var startingDeviceCount = _core.Devices.Count;

            _core.AddMockDevice();
            _core.RemoveMockDevice();

            // Simulate network conditions.
            await Task.Delay(50);

            Assert.AreEqual(startingDeviceCount, _core.Devices.Count);
            Assert.AreEqual(startingDeviceCount, _remoteHostCore.Devices.Count);
            Assert.AreEqual(startingDeviceCount, _remoteClientCore.Devices.Count);
        }

        private static void SingletonHost_MessageOutbound(object? sender, OwlCore.Remoting.Transfer.IRemoteMessage e)
        {
            RemoteCoreMessageHandler.SingletonClient.DigestMessageAsync(e);
        }

        private static void SingletonClient_MessageOutbound(object? sender, OwlCore.Remoting.Transfer.IRemoteMessage e)
        {
            RemoteCoreMessageHandler.SingletonHost.DigestMessageAsync(e);
        }
    }
}