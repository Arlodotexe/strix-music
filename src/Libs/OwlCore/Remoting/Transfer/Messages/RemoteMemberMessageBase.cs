namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// A base implementation of <see cref="IRemoteMemberMessage"/>.
    /// </summary>
    public class RemoteMemberMessageBase : IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMemberMessageBase"/>.
        /// </summary>
        /// <param name="memberInstanceId">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="targetName">The name of the target member being changed or invoked.</param>
        public RemoteMemberMessageBase(string memberInstanceId, string targetName)
        {
            MemberRemoteId = memberInstanceId;
            TargetMemberSignature = targetName;
        }

        /// <inheritdoc/>
        public string MemberRemoteId { get; set; }

        /// <inheritdoc/>
        public string TargetMemberSignature { get; set; }

        /// <inheritdoc/>
        public RemotingAction Action { get; set; }

        /// <inheritdoc/>
        public string? CustomActionName { get; set; }
    }
}