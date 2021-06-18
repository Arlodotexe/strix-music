using System;

namespace OwlCore.ClassRemote.Attributes
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark any member with this to control the data flow direction when changes are relayed remotely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RemoteOptionsAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteOptionsAttribute"/>.
        /// </summary>
        /// <param name="direction">The remoting direction to use when relaying class changes remotely.</param>
        public RemoteOptionsAttribute(RemotingDirection direction)
        {
            Direction = direction;
        }

        /// <inheritdoc cref="RemotingDirection"/>
        public RemotingDirection Direction { get; }
    }
}
