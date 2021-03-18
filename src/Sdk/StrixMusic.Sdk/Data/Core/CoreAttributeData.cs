using System;

namespace StrixMusic.Sdk.Data.Core
{
    /// <summary>
    /// The data given to the attribute in the AssemblyInfo.cs file.
    /// </summary>
    public class CoreAttributeData
    {
        /// <summary>
        /// Creates a new instance of <see cref="CoreAttributeData"/>.
        /// </summary>
        /// <param name="name">The user-friendly name of this core.</param>
        /// <param name="logoSvgUrl">The logo for this core.</param>
        /// <param name="coreTypeAssemblyQualifiedName">The full name of the core type associated with this attribute.</param>
        public CoreAttributeData(string name, string logoSvgUrl, string coreTypeAssemblyQualifiedName)
        {
            Name = name;
            LogoSvgUrl = new Uri(logoSvgUrl);
            CoreTypeAssemblyQualifiedName = coreTypeAssemblyQualifiedName;
        }

        /// <summary>
        /// The fully qualified assembly name for the core's type.
        /// </summary>
        public string CoreTypeAssemblyQualifiedName { get; }

        /// <summary>
        /// A local path or url pointing to a SVG file containing the logo for this core.
        /// </summary>
        public Uri LogoSvgUrl { get; }

        /// <summary>
        /// The user-friendly name of the core.
        /// </summary>
        public string Name { get; }
    }
}