namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to commumicate a remotely changed member.
    /// </summary>
    public interface IRemoteMemberMessage
    {
        // What if the IDs are unknown until they're created?
        // original creator decides ID. So the ID must be sent as part of an "instance registration".
        // Instance reg would mean actually creating the instances on the receiver side to match sender
        // This should be up to the app's method call, which can also be remotely executed with params if needed.
        // Ultimately it's up to the implementor to be smart about how they use it.
        // Verdict: Not our problem

        /// <summary>
        /// A unique identifier for this instance, consistent between hosts and clients.
        /// </summary>
        public string MemberInstanceId { get; set; }

        /// <summary>
        /// The name of the target member being changed or invoked.
        /// </summary>
        // TODO: More unique identifier for members that are consistent between app sessions.
        public string TargetName { get; set; }

        /// <inheritdoc cref="Action"/>
        public RemotingAction Action { get; }
    }
}
