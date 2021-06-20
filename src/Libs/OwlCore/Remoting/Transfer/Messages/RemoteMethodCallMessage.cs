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
        /// <param name="methodName">The name of the method being invoked.</param>
        /// <param name="parameters">The parameters to pass to the remote method, if any.</param>
        public RemoteMethodCallMessage(string memberInstanceId, string methodName, Dictionary<string, object?> parameters)
        {
            MemberInstanceId = memberInstanceId;
            TargetName = methodName;
            Parameters = parameters;
        }

        /// <inheritdoc />
        public string MemberInstanceId { get; set; }

        /// <inheritdoc/>
        public string TargetName { get; set; }

        /// <inheritdoc />
        public RemotingAction Action { get; } = RemotingAction.MethodCall;

        /// <summary>
        /// The arguments being passed to the remotely called method. Key is the <see cref="Type.AssemblyQualifiedName"/>, value is the data.
        /// </summary>
        // TODO: Handle generic types. AssemblyQualifiedName doesn't do this.
        public Dictionary<string, object?> Parameters { get; set; }
    }
}
