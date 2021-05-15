using StrixMusic.Sdk.Uno.Assembly;

namespace StrixMusic.Sdk.Uno.Models
{
    /// <summary>
    /// Holds the assembly info and additional metadata defined by a shell's AssemblyInfo.cs file.
    /// </summary>
    public class ShellAssemblyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAssemblyInfo"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name of the Shell.</param>
        /// <param name="attributeData">An instance of a <see cref="ShellAttributeData"/> with metadata about the shell.</param>
        public ShellAssemblyInfo(string assemblyName, ShellAttributeData attributeData)
        {
            AssemblyName = assemblyName;
            AttributeData = attributeData;
        }

        /// <summary>
        /// The assembly name of the Shell.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The display name for the Shell.
        /// </summary>
        public string DisplayName => AttributeData.DisplayName;

        /// <summary>
        /// A brief summary of the shell that will be displayed to the user.
        /// </summary>
        public string Description => AttributeData.Description;

        /// <summary>
        /// Gets the <see cref="ShellAttribute"/> for the <see cref="ShellAssemblyInfo"/>.
        /// </summary>
        public ShellAttributeData AttributeData { get; set;  }
    }
}
