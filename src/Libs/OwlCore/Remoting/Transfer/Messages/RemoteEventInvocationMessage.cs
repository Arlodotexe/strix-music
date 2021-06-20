namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to communicate a method call.
    /// </summary>
    public class RemoteEventInvocationMessage : IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodCallMessage"/>.
        /// </summary>
        /// <param name="memberInstanceId"></param>
        /// <param name="action"></param>
        /// <param name="targetName"></param>
        public RemoteEventInvocationMessage(string memberInstanceId, RemotingAction action, string targetName)
        {
            MemberInstanceId = memberInstanceId;
            Action = action;
            TargetName = targetName;
        }

        /// <inheritdoc />
        public string MemberInstanceId { get; set; }

        /// <inheritdoc />
        public RemotingAction Action { get; set; }

        /// <inheritdoc />
        public string TargetName { get; set; }
    }
}
