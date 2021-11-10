using System;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="MemberRemote"/>.
    /// Mark any member with this to control the data flow direction when changes are relayed remotely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
    public class RemoteOptionsAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteMethodAttribute"/>.
        /// </summary>
        /// <param name="direction">The remoting direction to use when relaying class changes remotely.</param>
        public RemoteOptionsAttribute(RemotingDirection direction)
        {
            Direction = direction;
        }

        /// <summary>
        /// The remoting direction to use when relaying class changes remotely.
        /// </summary>
        public RemotingDirection Direction { get; }
    }
}
