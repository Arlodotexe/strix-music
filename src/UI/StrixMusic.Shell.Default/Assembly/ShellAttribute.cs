using System;
using StrixMusic.Shell.Default.Assembly.Enums;

namespace StrixMusic.Shell.Default.Assembly
{
    /// <summary>
    /// An attribute for the shell's name.
    /// </summary>
    public class ShellAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAttribute"/> class.
        /// </summary>
        /// <param name="displayName"></param>
        public ShellAttribute(
            string displayName,
            DeviceFamily deviceFamily = (DeviceFamily)int.MaxValue,
            InputMethod inputMethod = (InputMethod)int.MaxValue)
        {
            DisplayName = displayName;
            DeviceFamily = deviceFamily;
            InputMethod = inputMethod;
        }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public DeviceFamily DeviceFamily { get; }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public InputMethod InputMethod { get; }
    }
}
