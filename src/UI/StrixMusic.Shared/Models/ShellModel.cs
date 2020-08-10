namespace StrixMusic.Models
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
        /// <param name="displayName">The display name for the Shell.</param>
        public ShellModel(string assemblyName, string displayName)
        {
            AssemblyName = assemblyName;
            DisplayName = displayName;
        }

        /// <summary>
        /// The assembly name of the Shell.
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The display name for the Shell.
        /// </summary>
        public string DisplayName { get; }
    }
}
