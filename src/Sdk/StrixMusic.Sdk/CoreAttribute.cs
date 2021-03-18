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
        /// A local path or url pointing to a SVG file containing the logo for this core.
        /// </summary>
        public string LogoSvgUrl { get; }

        /// <summary>
        /// The user-friendly name of the core.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAttribute"/> class.
        /// </summary>
        /// <param name="name">The user-friendly name of this core.</param>
        /// <param name="coreType">The derived type for this core. Must implement <see cref="ICore"/>.</param>
        /// <param name="logoSvgUrl">The logo for this core.</param>
        public CoreAttribute(string name, Type coreType, string logoSvgUrl)
        {
            CoreType = coreType;
            LogoSvgUrl = logoSvgUrl;
            Name = name;
        }
    }
}