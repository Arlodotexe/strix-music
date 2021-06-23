using System;
using System.Collections.Generic;

namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// Holds data to communicate a remotely invoked method.
    /// </summary>
    public class RemoteMethodCallMessage : IRemoteMemberMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodCallMessage"/>.
        /// </summary>
        /// <param name="memberInstanceId">A unique identifier for this instance, consistent between hosts and clients.</param>
        /// <param name="targetMemberSignature">The signature of the method being invoked.</param>
        /// <param name="parameters">The parameters to pass to the remote method, if any.</param>
        public RemoteMethodCallMessage(string memberInstanceId, string targetMemberSignature, Dictionary<string, object?> parameters)
        {
            MemberRemoteId = memberInstanceId;
            TargetMemberSignature = targetMemberSignature;
            Parameters = parameters;
        }

        /// <inheritdoc />
        public string MemberRemoteId { get; set; }

        /// <inheritdoc/>
        public string TargetMemberSignature { get; set; }

        /// <inheritdoc />
        public RemotingAction Action { get; } = RemotingAction.MethodCall;

        /// <summary>
        /// The arguments being passed to the remotely called method. Key is the <see cref="Type.AssemblyQualifiedName"/>, value is the data.
        /// </summary>
        public Dictionary<string, object?> Parameters { get; set; }

        /// <inheritdoc/>
        public string? CustomActionName { get; set; }
    }


    public class ParameterData
    {
        /// <summary>
        /// The value being passed to the parameter, if any.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// The assembly qualified name of the parameter type, if any
        /// </summary>
        public string? AssemblyQualifiedName { get; }


    }
}
