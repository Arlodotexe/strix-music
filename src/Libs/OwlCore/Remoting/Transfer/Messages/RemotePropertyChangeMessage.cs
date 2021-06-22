namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to communicate a method call.
    /// </summary>
    public class RemotePropertyChangeMessage : IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodCallMessage"/>.
        /// </summary>
        /// <param name="memberRemoteId">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="targetMemberSignature">The signature of the target being set.</param>
        /// <param name="newValue">The new value being set to the backing field.</param>
        /// <param name="oldValue">The previous value of the backing field.</param>
        public RemotePropertyChangeMessage(string memberRemoteId, string targetMemberSignature, object? newValue, object? oldValue)
        {
            MemberRemoteId = memberRemoteId;
            TargetMemberSignature = targetMemberSignature;
            NewValue = newValue;
            OldValue = oldValue;
        }

        /// <inheritdoc />
        public string MemberRemoteId { get; set; }

        /// <inheritdoc />
        public RemotingAction Action { get; } = RemotingAction.PropertyChange;

        /// <inheritdoc />
        public string TargetMemberSignature { get; set; }

        /// <summary>
        /// The previous value of the backing field.
        /// </summary>
        public object? OldValue { get; set; }

        /// <summary>
        /// The new value being set to the backing field.
        /// </summary>
        public object? NewValue { get; set; }
    }
}
