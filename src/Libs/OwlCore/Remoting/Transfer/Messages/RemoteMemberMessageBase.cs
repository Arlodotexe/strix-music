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
            MemberInstanceId = memberInstanceId;
            TargetName = targetName;
        }

        /// <inheritdoc/>
        public string MemberInstanceId { get; set; }

        /// <inheritdoc/>
        public string TargetName { get; set; }

        /// <inheritdoc/>
        public RemotingAction Action { get; set; }
    }
}