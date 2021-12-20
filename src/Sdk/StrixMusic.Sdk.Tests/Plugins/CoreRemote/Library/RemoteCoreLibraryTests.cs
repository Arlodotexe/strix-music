using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Sdk.Tests.Mock.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Plugins.CoreRemote
{
    [TestClass]
    public class RemoteCoreLibraryTests
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
        public async Task RemoteTotalTrackCount()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, core.Library.TotalTrackCount);

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(core.Library.TotalTrackCount, remoteClientCore.Library.TotalTrackCount);
            Assert.AreEqual(core.Library.TotalTrackCount, remoteHostCore.Library.TotalTrackCount);

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();
        }

        [DataRow(1), DataRow(10), DataRow(100)]
        [TestMethod, Timeout(2000)]
        public async Task RemoteTotalTrackCount_Changed(int amount)
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            // For test to work, must not be a default value.
            Assert.AreNotEqual(default, amount);

            core.Library.Cast<MockCoreLibrary>().TotalTrackCount = amount;

            // Wait for changes to propogate
            await Task.Delay(500);

            Assert.AreEqual(amount, core.Library.TotalTrackCount);
            Assert.AreEqual(amount, remoteClientCore.Library.TotalTrackCount);
            Assert.AreEqual(amount, remoteHostCore.Library.TotalTrackCount);

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
