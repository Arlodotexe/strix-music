namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to communicate any remote action. The base for all messages sent from and received by this node.
    /// </summary>
    /// <remarks>
    /// See also <seealso cref="IRemoteMemberMessage"/>.
    /// </remarks>
    public interface IRemoteMessage
    {
        /// <inheritdoc cref="RemotingAction"/>
        public RemotingAction Action { get; }

        /// <summary>
        /// A custom remoting action for custom <see cref="IRemoteMessage"/>s not supplied by the library.
        /// </summary>
        /// <remarks>
        /// Use this to assist in de/serializing any custom <see cref="IRemoteMessage"/>s you may be using.
        /// </remarks>
        public string? CustomActionName { get; }
    }
}
