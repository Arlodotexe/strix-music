namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to commumicate a remotely changed member.
    /// </summary>
    public interface IRemoteMemberMessage : IRemoteMessage
    {
        /// <summary>
        /// A unique identifier for this instance, consistent between hosts and clients.
        /// </summary>
        public string MemberRemoteId { get; set; }

        /// <summary>
        /// The signature of the target member being changed or invoked.
        /// </summary>
        public string TargetMemberSignature { get; set; }
    }
}
