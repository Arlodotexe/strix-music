namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// A base implementation of <see cref="IRemoteMemberMessage"/>.
    /// </summary>
    public class RemoteMessageBase : IRemoteMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMessageBase"/>.
        /// </summary>
        public RemoteMessageBase()
        {
        }

        /// <inheritdoc/>
        public RemotingAction Action { get; set; }

        /// <inheritdoc/>
        public string? CustomActionName { get; set; }
    }
}