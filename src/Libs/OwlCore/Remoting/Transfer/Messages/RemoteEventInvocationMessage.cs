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
            MemberRemoteId = memberInstanceId;
            Action = action;
            TargetMemberSignature = targetName;
        }

        /// <inheritdoc />
        public string MemberRemoteId { get; set; }

        /// <inheritdoc />
        public RemotingAction Action { get; set; }

        /// <inheritdoc />
        public string TargetMemberSignature { get; set; }
    }
}
