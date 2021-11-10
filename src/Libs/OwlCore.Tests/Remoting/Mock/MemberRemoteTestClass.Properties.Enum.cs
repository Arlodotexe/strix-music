using OwlCore.Remoting;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteProperty, RemoteOptions(RemotingDirection.None),]
        public RemotingDirection RemoteProperty_Enum_None { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Bidirectional)]
        public RemotingDirection RemoteProperty_Enum_Bidirectional { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.HostToClient)]
        public RemotingDirection RemoteProperty_Enum_HostToClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.ClientToHost)]
        public RemotingDirection RemoteProperty_Enum_ClientToHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Inbound)]
        public RemotingDirection RemoteProperty_Enum_Inbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundClient)]
        public RemotingDirection RemoteProperty_Enum_InboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundHost)]
        public RemotingDirection RemoteProperty_Enum_InboundHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Outbound)]
        public RemotingDirection RemoteProperty_Enum_Outbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundClient)]
        public RemotingDirection RemoteProperty_Enum_OutboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundHost)]
        public RemotingDirection RemoteProperty_Enum_OutboundHost { get; set; }
    }
}
