using OwlCore.Remoting;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteProperty, RemoteOptions(RemotingDirection.None)]
        public int RemoteProperty_Primitive_None { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Bidirectional)]
        public int RemoteProperty_Primitive_Bidirectional { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.HostToClient)]
        public int RemoteProperty_Primitive_HostToClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.ClientToHost)]
        public int RemoteProperty_Primitive_ClientToHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Inbound)]
        public int RemoteProperty_Primitive_Inbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundClient)]
        public int RemoteProperty_Primitive_InboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundHost)]
        public int RemoteProperty_Primitive_InboundHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Outbound)]
        public int RemoteProperty_Primitive_Outbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundClient)]
        public int RemoteProperty_Primitive_OutboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundHost)]
        public int RemoteProperty_Primitive_OutboundHost { get; set; }
    }
}
