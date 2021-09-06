using System;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Contains information for sending and receiving a single object tied to a token.
    /// </summary>
    public class RemoteDataMessage : RemoteMessageBase, IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMessageBase"/>.
        /// </summary>
        /// <param name="memberInstanceId">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="token">The name of the target member being changed or invoked.</param>
        /// <param name="memberSignature"></param>
        /// <param name="result">The result data being transferred.</param>
        public RemoteDataMessage(string memberInstanceId, string token, string memberSignature, object? result)
        {
            MemberRemoteId = memberInstanceId;
            TargetMemberSignature = memberSignature;
            Token = token;
            Result = result;

            Action = RemotingAction.RemoteDataProxy;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteMessageBase"/>.
        /// </summary>
        /// <remarks>Should only be used by deserializers.</remarks>
        public RemoteDataMessage()
        {
            Token = string.Empty;
            MemberRemoteId = string.Empty;
            TargetMemberSignature = string.Empty;

            Action = RemotingAction.RemoteDataProxy;
        }

        /// <inheritdoc/>
        public string MemberRemoteId { get; set; }

        /// <inheritdoc/>
        public string TargetMemberSignature { get; set; }

        /// <summary>
        /// A unique identifier for this member.
        /// </summary>
        public string Token { get; set; }

        /// <summary>s
        /// The result value.
        /// </summary>
        public object? Result { get; set; }
    }
}
