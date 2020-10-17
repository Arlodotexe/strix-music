using StrixMusic.Sdk.Uno.Assembly;

namespace StrixMusic.Sdk.Uno.Models
{
    /// <summary>
    /// A Model for a shell in settings.
    /// </summary>
    public class ShellModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellModel"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name of the Shell.</param>
        /// <param name="attribute">An instance of a <see cref="Sdk.Uno.Assembly.ShellAttribute"/> with metadata about the shell.</param>
        public ShellModel(string assemblyName, ShellAttribute attribute)
        {
            AssemblyName = assemblyName;
            ShellAttribute = attribute;
        }

        /// <summary>
        /// The assembly name of the Shell.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The display name for the Shell.
        /// </summary>
        public string DisplayName => ShellAttribute.DisplayName;

        /// <summary>
        /// Gets the <see cref="ShellAttribute"/> for the <see cref="ShellModel"/>.
        /// </summary>
        public ShellAttribute ShellAttribute { get; }
    }
}
