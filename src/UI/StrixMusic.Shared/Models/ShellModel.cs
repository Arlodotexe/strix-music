using StrixMusic.Shell.Default.Assembly;
using Windows.Foundation;

namespace StrixMusic.Models
{
    /// <summary>
    /// A Model for a shell in settings.
    /// </summary>
    public class ShellModel
    {
        private readonly ShellAttribute _shellAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellModel"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name of the Shell.</param>
        /// <param name="attribute">An instance of a <see cref="ShellAttribute"/> with metadata about the shell.</param>
        public ShellModel(string assemblyName, ShellAttribute attribute)
        {
            AssemblyName = assemblyName;
            _shellAttribute = attribute;
        }

        /// <summary>
        /// The assembly name of the Shell.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The display name for the Shell.
        /// </summary>
        public string DisplayName => _shellAttribute.DisplayName;

        /// <summary>
        /// Gets the <see cref="ShellAttribute"/> for the <see cref="ShellModel"/>.
        /// </summary>
        public ShellAttribute ShellAttribute => _shellAttribute;
    }
}
