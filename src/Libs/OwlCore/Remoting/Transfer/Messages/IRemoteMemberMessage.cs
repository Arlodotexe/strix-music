namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to commumicate a remotely changed member.
    /// </summary>
    public interface IRemoteMemberMessage
    {
        /// <summary>
        /// A unique identifier for this instance, consistent between hosts and clients.
        /// </summary>
        public string MemberRemoteId { get; set; }

        /// <summary>
        /// The signature of the target member being changed or invoked.
        /// </summary>
        public string TargetMemberSignature { get; set; }

        /// <inheritdoc cref="Action"/>
        public RemotingAction Action { get; }

        /// <summary>
        /// A custom remoting action for custom <see cref="IRemoteMemberMessage"/>s not supplied by the library.
        /// </summary>
        /// <remarks>
        /// Use this to assist in de/serializing any custom <see cref="IRemoteMemberMessage"/>s you may be using.
        /// </remarks>
        public string? CustomActionName { get; set; }
    }
}
