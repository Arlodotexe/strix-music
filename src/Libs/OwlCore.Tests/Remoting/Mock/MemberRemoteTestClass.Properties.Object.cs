using OwlCore.Remoting;

namespace OwlCore.Tests.Remoting.Mock
{
    public partial class MemberRemoteTestClass
    {
        [RemoteProperty, RemoteOptions(RemotingDirection.None)]
        public object RemoteProperty_Object_None { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Bidirectional)]
        public object RemoteProperty_Object_Bidirectional { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.HostToClient)]
        public object RemoteProperty_Object_HostToClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.ClientToHost)]
        public object RemoteProperty_Object_ClientToHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Inbound)]
        public object RemoteProperty_Object_Inbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundClient)]
        public object RemoteProperty_Object_InboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.InboundHost)]
        public object RemoteProperty_Object_InboundHost { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.Outbound)]
        public object RemoteProperty_Object_Outbound { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundClient)]
        public object RemoteProperty_Object_OutboundClient { get; set; }

        [RemoteProperty, RemoteOptions(RemotingDirection.OutboundHost)]
        public object RemoteProperty_Object_OutboundHost { get; set; }
    }
}
