using OwlCore.Remoting;
using System;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteProperty, RemoteOptions(RemotingDirection.None)]
        public DateTime RemoteProperty_Struct_None { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Bidirectional)]
        public DateTime RemoteProperty_Struct_Bidirectional { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.HostToClient)]
        public DateTime RemoteProperty_Struct_HostToClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.ClientToHost)]
        public DateTime RemoteProperty_Struct_ClientToHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Inbound)]
        public DateTime RemoteProperty_Struct_Inbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundClient)]
        public DateTime RemoteProperty_Struct_InboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundHost)]
        public DateTime RemoteProperty_Struct_InboundHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Outbound)]
        public DateTime RemoteProperty_Struct_Outbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundClient)]
        public DateTime RemoteProperty_Struct_OutboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundHost)]
        public DateTime RemoteProperty_Struct_OutboundHost { get; set; }
    }
}
