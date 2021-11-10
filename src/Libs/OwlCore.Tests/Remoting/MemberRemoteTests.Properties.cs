using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using OwlCore.Remoting;
using OwlCore.Tests.Remoting.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting
{
    public partial class MemberRemoteTests
    {
        [DataRow(RemotingDirection.None, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Full, RemotingMode.Host)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Host, RemotingMode.Client)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.None, false, RemotingMode.Client, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_None(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Full, RemotingMode.Host)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Host, RemotingMode.Full)]

        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Host, RemotingMode.Client)]
        [DataRow(RemotingDirection.Bidirectional, true, RemotingMode.Client, RemotingMode.Host)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_Bidirectional(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.ClientToHost, true, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.ClientToHost, true, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.ClientToHost, true, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.ClientToHost, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_ClientToHost(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.Inbound, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_Inbound(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundHost, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_InboundHost(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.InboundClient, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_InboundClient(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.Outbound, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_Outbound(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundHost, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_OutboundHost(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Client, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Host, RemotingMode.Full)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Full, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Full, RemotingMode.Host)]

        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Host, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Client, RemotingMode.Client)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Client, RemotingMode.Host)]
        [DataRow(RemotingDirection.OutboundClient, false, RemotingMode.Host, RemotingMode.Client)]
        [TestMethod, Timeout(5000)]
        public Task RemoteProperty_OutboundClient(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            return RemotePropertyTest_Internal(direction, allNonFirstNodesShouldReceive, senderMode, listenerModes);
        }

        private Task RemotePropertyTest_Internal(RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            var memberTypesToTest = new List<MemberType>()
            {
                MemberType.Enum,
                MemberType.Struct,
                MemberType.Object,
                MemberType.Primitive,
            };

            return memberTypesToTest.InParallel(x => RemotePropertyTest_Internal(x, direction, allNonFirstNodesShouldReceive, senderMode, listenerModes));
        }

        private async Task RemotePropertyTest_Internal(MemberType memberType, RemotingDirection direction, bool allNonFirstNodesShouldReceive, RemotingMode senderMode, params RemotingMode[] listenerModes)
        {
            // Setup sender and listeners
            var classes = CreateMemberRemoteTestClasses(senderMode.IntoList().Concat(listenerModes));
            var listenerClasses = classes.Skip(1).ToList();
            var senderClass = classes.ElementAt(0);

            var valueToAssign = GetDefaultTestValue<object>(memberType);

            SetTestProperty(senderClass, memberType, direction, valueToAssign);
            var originalValue = GetTestProperty(senderClass, memberType, direction);

            // Internal remoting system is not synchronous, even when code is local.
            // Wait long enough for all member changes to be processed.
            await Task.Delay(250);

            var receivedValues = listenerClasses.Select(x => GetTestProperty(x, memberType, direction));

            foreach (var receivedVal in receivedValues)
            {
                var equal = Helpers.SmartEquals(originalValue, originalValue.GetType(), receivedVal, receivedVal?.GetType());
                Assert.IsTrue(allNonFirstNodesShouldReceive == equal);
            }
        }

        private object GetTestProperty(MemberRemoteTestClass instance, MemberType memberType, RemotingDirection direction)
        {
            return memberType switch
            {
                MemberType.Enum => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Enum_None,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Enum_InboundHost,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Enum_InboundClient,
                    RemotingDirection.Inbound => instance.RemoteProperty_Enum_Inbound,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Enum_OutboundHost,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Enum_OutboundClient,
                    RemotingDirection.Outbound => instance.RemoteProperty_Enum_Outbound,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Enum_HostToClient,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Enum_ClientToHost,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Enum_Bidirectional,
                    _ => throw new NotSupportedException(),
                },
                MemberType.Struct => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Struct_None,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Struct_InboundHost,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Struct_InboundClient,
                    RemotingDirection.Inbound => instance.RemoteProperty_Struct_Inbound,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Struct_OutboundHost,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Struct_OutboundClient,
                    RemotingDirection.Outbound => instance.RemoteProperty_Struct_Outbound,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Struct_HostToClient,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Struct_ClientToHost,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Struct_Bidirectional,
                    _ => throw new NotSupportedException(),
                },
                MemberType.Primitive => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Primitive_None,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Primitive_InboundHost,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Primitive_InboundClient,
                    RemotingDirection.Inbound => instance.RemoteProperty_Primitive_Inbound,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Primitive_OutboundHost,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Primitive_OutboundClient,
                    RemotingDirection.Outbound => instance.RemoteProperty_Primitive_Outbound,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Primitive_HostToClient,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Primitive_ClientToHost,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Primitive_Bidirectional,
                    _ => throw new NotSupportedException(),
                },
                MemberType.Object => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Object_None,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Object_InboundHost,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Object_InboundClient,
                    RemotingDirection.Inbound => instance.RemoteProperty_Object_Inbound,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Object_OutboundHost,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Object_OutboundClient,
                    RemotingDirection.Outbound => instance.RemoteProperty_Object_Outbound,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Object_HostToClient,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Object_ClientToHost,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Object_Bidirectional,
                    _ => throw new NotSupportedException(),
                },
                MemberType.None => throw new NotSupportedException(),
                _ => throw new NotSupportedException(),
            };
        }

        private object SetTestProperty(MemberRemoteTestClass instance, MemberType memberType, RemotingDirection direction, object value)
        {
            return memberType switch
            {
                MemberType.Enum => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Enum_None = (RemotingDirection)value,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Enum_InboundHost = (RemotingDirection)value,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Enum_InboundClient = (RemotingDirection)value,
                    RemotingDirection.Inbound => instance.RemoteProperty_Enum_Inbound = (RemotingDirection)value,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Enum_OutboundHost = (RemotingDirection)value,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Enum_OutboundClient = (RemotingDirection)value,
                    RemotingDirection.Outbound => instance.RemoteProperty_Enum_Outbound = (RemotingDirection)value,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Enum_HostToClient = (RemotingDirection)value,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Enum_ClientToHost = (RemotingDirection)value,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Enum_Bidirectional = (RemotingDirection)value,
                    _ => throw new NotSupportedException(),
                },
                MemberType.Struct => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Struct_None = (DateTime)value,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Struct_InboundHost = (DateTime)value,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Struct_InboundClient = (DateTime)value,
                    RemotingDirection.Inbound => instance.RemoteProperty_Struct_Inbound = (DateTime)value,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Struct_OutboundHost = (DateTime)value,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Struct_OutboundClient = (DateTime)value,
                    RemotingDirection.Outbound => instance.RemoteProperty_Struct_Outbound = (DateTime)value,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Struct_HostToClient = (DateTime)value,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Struct_ClientToHost = (DateTime)value,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Struct_Bidirectional = (DateTime)value,
                    _ => throw new NotSupportedException(),
                },
                MemberType.Primitive => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Primitive_None = (int)value,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Primitive_InboundHost = (int)value,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Primitive_InboundClient = (int)value,
                    RemotingDirection.Inbound => instance.RemoteProperty_Primitive_Inbound = (int)value,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Primitive_OutboundHost = (int)value,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Primitive_OutboundClient = (int)value,
                    RemotingDirection.Outbound => instance.RemoteProperty_Primitive_Outbound = (int)value,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Primitive_HostToClient = (int)value,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Primitive_ClientToHost = (int)value,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Primitive_Bidirectional = (int)value,
                    _ => throw new NotSupportedException(),
                },
                MemberType.Object => direction switch
                {
                    RemotingDirection.None => instance.RemoteProperty_Object_None = value,
                    RemotingDirection.InboundHost => instance.RemoteProperty_Object_InboundHost = value,
                    RemotingDirection.InboundClient => instance.RemoteProperty_Object_InboundClient = value,
                    RemotingDirection.Inbound => instance.RemoteProperty_Object_Inbound = value,
                    RemotingDirection.OutboundHost => instance.RemoteProperty_Object_OutboundHost = value,
                    RemotingDirection.OutboundClient => instance.RemoteProperty_Object_OutboundClient = value,
                    RemotingDirection.Outbound => instance.RemoteProperty_Object_Outbound = value,
                    RemotingDirection.HostToClient => instance.RemoteProperty_Object_HostToClient = value,
                    RemotingDirection.ClientToHost => instance.RemoteProperty_Object_ClientToHost = value,
                    RemotingDirection.Bidirectional => instance.RemoteProperty_Object_Bidirectional = value,
                    _ => throw new NotSupportedException(),
                },
                MemberType.None => throw new NotSupportedException(),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
