namespace StrixMusic.Sdk.Data.Core
{
    /// <summary>
    /// Holds the assembly info and additional metadata defined by a shell's AssemblyInfo.cs file.
    /// </summary>
    public class CoreAssemblyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAssemblyInfo"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name of the Core.</param>
        /// <param name="attribute">An instance of a <see cref="CoreAttribute"/> with metadata about the Core.</param>
        public CoreAssemblyInfo(string assemblyName, CoreAttributeData attribute)
        {
            AssemblyName = assemblyName;
            AttributeData = attribute;
        }

        /// <summary>
        /// The assembly name of the Core.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets the <see cref="CoreAttribute"/> for the <see cref="CoreAssemblyInfo"/>.
        /// </summary>
        public CoreAttributeData AttributeData { get; set; }
    }
}