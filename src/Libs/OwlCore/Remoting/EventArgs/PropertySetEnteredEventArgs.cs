using Cauldron.Interception;

namespace OwlCore.Remoting
{
    /// <summary>
    /// <see cref="System.EventArgs"/> for <see cref="RemotePropertyAttribute.SetEntered"/>.
    /// </summary>
    public class PropertySetEnteredEventArgs : System.EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="MethodEnteredEventArgs"/>.
        /// </summary>
        public PropertySetEnteredEventArgs(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            PropertyInterceptionInfo = propertyInterceptionInfo;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Contains information about the intercepted property.
        /// </summary>
        public PropertyInterceptionInfo PropertyInterceptionInfo { get; }

        /// <summary>
        /// The previously assigned value of the backing field.
        /// </summary>
        public object? OldValue { get; }

        /// <summary>
        /// The new value being assigned to the backing field.
        /// </summary>
        public object NewValue { get; }
    }
}
