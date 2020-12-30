using System;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk
{
    /// <summary>
    /// An attribute used to dynamically import a core.
    /// </summary>
    public class CoreAttribute : Attribute
    {
        /// <summary>
        /// The derived type for this core. Must implement <see cref="ICore"/>.
        /// </summary>
        public Type CoreType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAttribute"/> class.
        /// </summary>
        /// <param name="coreType">The derived type for this core. Must implement <see cref="ICore"/>.</param>
        public CoreAttribute(Type coreType)
        {
            CoreType = coreType;
        }
    }
}