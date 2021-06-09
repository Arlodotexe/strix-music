namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Used to specify the direction of data flow when relaying class changes remotely.
    /// </summary>
    public enum RemotingDirection
    {
        // Note to self
        // This is perfectly fine
        // but since the source code is expected to be identical most of the time
        // Then a normal attribute won't for inbound/outbound
        // since both sides expect the other to do something
        // Need to be able to set it per member at runtime,
        // without mucking up the cleanliness of an attribute


        /// <summary>
        /// Remote data relay happens in both directions.
        /// </summary>
        Bidirectional,

        /// <summary>
        /// The target only receives changes, does not send them.
        /// </summary>
        Inbound,

        /// <summary>
        /// The target only sends changes, does not receive them.
        /// </summary>
        Outbound,

        /// <summary>
        /// No changes are sent or received. Remoting is effectively disabled for the target.
        /// </summary>
        None,
    }
}
