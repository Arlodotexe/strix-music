namespace OwlCore.Remoting.Transfer
{
    /// <summary>
    /// The action associated with a single <see cref="IRemoteMessage"/>.
    /// </summary>
    public enum RemotingAction
    {
        /// <summary>
        /// Indicates no remoting action should be taken, regardless of data present.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a method invocation.
        /// </summary>
        MethodCall,

        /// <summary>
        /// Represents the setting of a property.
        /// </summary>
        PropertyChange,

        /// <summary>
        /// Indicates that the message delegates a <see cref="RemoteDataMessage"/>.
        /// </summary>
        RemoteDataProxy,

        /// <summary>
        /// Indicates that an exception was thrown by a node.
        /// </summary>
        ExceptionThrown,

        /// <summary>
        /// Indicates that the message contains a custom remoting action. See <see cref="IRemoteMessage.CustomActionName"/>.
        /// </summary>
        Custom,
    }
}
