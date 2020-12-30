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
        /// <param name="coreTypeAssemblyQualifiedName">The full name of the core type associated with this attribute.</param>
        public CoreAttributeData(string coreTypeAssemblyQualifiedName)
        {
            CoreTypeAssemblyQualifiedName = coreTypeAssemblyQualifiedName;
        }

        /// <summary>
        /// The fully qualified assembly name for the core's type.
        /// </summary>
        public string CoreTypeAssemblyQualifiedName { get; }
    }
}