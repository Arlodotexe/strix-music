using System;

namespace OwlCore.Remoting
{
    /// <summary>
    /// The primary kind of remoting used for this node.
    /// </summary>
    [Flags]
    public enum RemotingMode
    {
        /// <summary>
        /// This node acts as neither client nor host. Remoting is effectively disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// This node acts primarily as a listener.
        /// </summary>
        Client = 1,

        /// <summary>
        /// This node acts primarily as a sender.
        /// </summary>
        Host = 2,

        /// <summary>
        /// This node acts as both <see cref="Client"/> and <see cref="Host"/>.
        /// </summary>
        Full = Client | Host,
    }
}
