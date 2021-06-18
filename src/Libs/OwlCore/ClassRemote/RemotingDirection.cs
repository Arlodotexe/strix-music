using OwlCore.ClassRemote.Attributes;
using System;

namespace OwlCore.ClassRemote
{
    /// <summary>
    /// Used to specify the direction of data flow when relaying member changes remotely.
    /// </summary>
    [Flags]
    public enum RemotingDirection : byte
    {
        /// <summary>
        /// No changes are sent or received. Remoting is effectively disabled for the target.
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
        /// The target emits and receives changes when the node is in either <see cref="RemotingMode.Client"/> or <see cref="RemotingMode.Host"/> mode.
        /// </summary>
        Bidirectional = Inbound | Outbound,
    }
}
