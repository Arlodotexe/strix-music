using System;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Used to specify the direction of data flow when relaying member changes remotely.
    /// </summary>
    [Flags]
    public enum RemotingDirection : byte
    {
        /// <summary>
        /// No changes are sent or received. Remoting is disabled for the target.
        /// </summary>
        None = 0,

        /// <summary>
        /// The target receives changes when the node is in <see cref="RemotingMode.Host"/> mode.
        /// </summary>
        InboundHost = 1,

        /// <summary>
        /// The target receives changes when the node is in <see cref="RemotingMode.Client"/> mode.
        /// </summary>
        InboundClient = 2,

        /// <summary>
        /// The target receives changes when the node is in either <see cref="RemotingMode.Client"/> or <see cref="RemotingMode.Host"/> mode.
        /// </summary>
        Inbound = InboundHost | InboundClient,

        /// <summary>
        /// The target emits changes when the node is in <see cref="RemotingMode.Host"/> mode.
        /// </summary>
        OutboundHost = 4,

        /// <summary>
        /// The target emits changes when the node is in <see cref="RemotingMode.Client"/> mode.
        /// </summary>
        OutboundClient = 8,

        /// <summary>
        /// The target emits changes when the node is in either <see cref="RemotingMode.Client"/> or <see cref="RemotingMode.Host"/> mode.
        /// </summary>
        Outbound = OutboundHost | OutboundClient,

        /// <summary>
        /// Data is only emitted from the host to the client.
        /// </summary>
        HostToClient = InboundClient | OutboundHost,

        /// <summary>
        /// Data is only emitted from the client to the host.
        /// </summary>
        ClientToHost = OutboundClient | InboundHost,

        /// <summary>
        /// The target emits and receives changes when the node is in either <see cref="RemotingMode.Client"/> or <see cref="RemotingMode.Host"/> mode.
        /// </summary>
        Bidirectional = Inbound | Outbound,
    }
}
