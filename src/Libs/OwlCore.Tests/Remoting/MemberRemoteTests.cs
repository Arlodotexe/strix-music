using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Remoting;
using OwlCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using OwlCore.Tests.Remoting.Transfer;
using OwlCore.Tests.Remoting.Mock;

namespace OwlCore.Tests.Remoting
{
    /// <remarks>
    /// These tests assume that the code running on both machines are identical, which might not always be the case.
    /// <para/>
    /// In the scenarios defined above,
    /// anything defining only OutboundHost/OutboundClient will send but never receive and
    /// anything defining only InboundHost/InboundClient will receive but never send.
    /// </remarks>
    [TestClass]
    public partial class MemberRemoteTests
    {
        private List<MemberRemoteTestClass> CreateMemberRemoteTestClasses(IEnumerable<RemotingMode> nodesModes)
        {
            var testClasses = new List<MemberRemoteTestClass>();
            var handlers = new List<LoopbackMockMessageHandler>();

            foreach (var mode in nodesModes)
            {
                var loopbackHandler = new LoopbackMockMessageHandler(mode);

                handlers.Add(loopbackHandler);
                testClasses.Add(new MemberRemoteTestClass("TestClass", loopbackHandler));
            }

            foreach (var handler in handlers)
                handler.LoopbackListeners.AddRange(handlers.Except(handler.IntoList()));

            return testClasses;
        }
    }
}
