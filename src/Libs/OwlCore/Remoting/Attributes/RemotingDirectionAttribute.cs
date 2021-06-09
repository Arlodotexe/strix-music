using System;

namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Attribute used in conjuction with <see cref="RemoteViewModel"/>.
    /// Mark any member with this to control the data flow direction when changes are relayed remotely.
    /// </summary>
    public class RemotingDirectionAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemotingDirectionAttribute"/>.
        /// </summary>
        /// <param name="direction">The remoting direction to use when relaying class changes remotely.</param>
        public RemotingDirectionAttribute(RemotingDirection direction)
        {
            Direction = direction;
        }

        /// <inheritdoc cref="RemotingDirection"/>
        public RemotingDirection Direction { get; }
    }
}
