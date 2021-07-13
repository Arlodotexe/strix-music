using System;
using System.Collections.Generic;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to communicate a remotely invoked method.
    /// </summary>
    public class RemoteMethodCallMessage : RemoteMessageBase, IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodCallMessage"/>.
        /// </summary>
        /// <param name="memberInstanceId">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="targetMemberSignature">The signature of the method being invoked.</param>
        /// <param name="parameters">The parameters to pass to the remote method, if any.</param>
        public RemoteMethodCallMessage(string memberInstanceId, string targetMemberSignature, IEnumerable<ParameterData> parameters)
        {
            MemberRemoteId = memberInstanceId;
            TargetMemberSignature = targetMemberSignature;
            Parameters = parameters;

            Action = RemotingAction.MethodCall;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodCallMessage"/>.
        /// </summary>
        /// <remarks>Should only be used by deserializers.</remarks>
        public RemoteMethodCallMessage()
        {
            MemberRemoteId = string.Empty;
            TargetMemberSignature = string.Empty;
            Parameters = new List<ParameterData>();

            Action = RemotingAction.MethodCall;
        }

        /// <inheritdoc />
        public string MemberRemoteId { get; set; }

        /// <inheritdoc/>
        public string TargetMemberSignature { get; set; }

        /// <summary>
        /// The arguments being passed to the remotely called method. Key is the <see cref="Type.AssemblyQualifiedName"/>, value is the data.
        /// </summary>
        public IEnumerable<ParameterData> Parameters { get; set; }
    }

    /// <summary>
    /// Holds data about a parameter passed to a remote method call.
    /// </summary>
    public class ParameterData
    {
        /// <summary>
        /// The value being passed to the parameter, if any.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// The assembly qualified name of the parameter type, if any
        /// </summary>
        public string? AssemblyQualifiedName { get; set; }
    }
}
