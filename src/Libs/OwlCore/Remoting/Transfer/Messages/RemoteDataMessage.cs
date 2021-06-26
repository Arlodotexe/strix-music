using System;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Contains information for sending and receiving a single object tied to a token.
    /// </summary>
    public class RemoteDataMessage<T> : IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMemberMessageBase"/>.
        /// </summary>
        /// <param name="memberInstanceId">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="token">The name of the target member being changed or invoked.</param>
        /// <param name="memberSignature"></param>
        /// <param name="result">The result data being transferred.</param>
        public RemoteDataMessage(string memberInstanceId, string token, string memberSignature, T? result)
        {
            MemberRemoteId = memberInstanceId;
            TargetMemberSignature = memberSignature;
            Token = token;
            Result = result;
        }

        /// <inheritdoc/>
        public string MemberRemoteId { get; set; }

        /// <inheritdoc/>
        public string TargetMemberSignature { get; set; }

        /// <inheritdoc/>
        public RemotingAction Action { get; set; } = RemotingAction.RemoteDataProxy;

        /// <summary>
        /// A unique identifier for this member.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The result value.
        /// </summary>
        public T? Result { get; set; } // todo: newtonsoft can't serialize this type properly for some reason

        /// <inheritdoc/>
        public string? CustomActionName { get; set; }
    }
}
