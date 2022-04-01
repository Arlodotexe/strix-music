using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.CoreRemote;

namespace StrixMusic.Cores.Remote.OwlCore.Tests
{
    [TestClass]
    public partial class RemoteCoreLibraryTests
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
