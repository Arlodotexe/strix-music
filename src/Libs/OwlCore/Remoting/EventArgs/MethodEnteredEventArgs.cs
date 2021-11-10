using System;
using System.Reflection;

namespace OwlCore.Remoting
{
    /// <summary>
    /// <see cref="System.EventArgs"/> for <see cref="RemoteMethodAttribute.Entered"/>.
    /// </summary>
    public class MethodEnteredEventArgs : System.EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="MethodEnteredEventArgs"/>.
        /// </summary>
        /// <param name="declaringType">The type declaring the intercepted method.</param>
        /// <param name="instance">The instance of the class where the member is residing. Will be null if the member is static.</param>
        /// <param name="methodBase">Contains information about the method.</param>
        /// <param name="values">The passed arguments of the method.</param>
        public MethodEnteredEventArgs(Type declaringType, object? instance, MethodBase methodBase, object?[] values)
        {
            DeclaringType = declaringType;
            Instance = instance;
            MethodBase = methodBase;
            Values = values;
        }

        /// <summary>
        /// The type declaring the intercepted method.
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        /// The instance of the class where the member is residing. Will be null if the member is static.
        /// </summary>
        public object? Instance { get; }

        /// <summary>
        /// Contains information about the method.
        /// </summary>
        public MethodBase MethodBase { get; }

        /// <summary>
        /// The passed arguments of the method.
        /// </summary>
        public object?[] Values { get; }
    }
}
