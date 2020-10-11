using System;
using StrixMusic.Shell.Default.Assembly.Enums;
using Windows.Foundation;

namespace StrixMusic.Shell.Default.Assembly
{
    /// <summary>
    /// An attribute for the shell's name and other data.
    /// </summary>
    public class ShellAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAttribute"/> class.
        /// </summary>
        /// <param name="displayName">The display name for the shell.</param>
        /// <param name="deviceFamily">The supported device families for the shell.</param>
        /// <param name="inputMethod">The supported input methods.</param>
        /// <param name="maxWidth">The maximum width of the window for the shell.</param>
        /// <param name="maxHeight">The maximum height of the window for the shell.</param>
        /// <param name="minWidth">The minimum width of the window for the shell.</param>
        /// <param name="minHeight">The minimum height of the window for the shell.</param>
        public ShellAttribute(
            string displayName,
            DeviceFamily deviceFamily = (DeviceFamily)int.MaxValue,
            InputMethod inputMethod = (InputMethod)int.MaxValue,
            double maxWidth = double.MaxValue,
            double maxHeight = double.MaxValue,
            double minWidth = 0,
            double minHeight = 0)
        {
            DisplayName = displayName;
            DeviceFamily = deviceFamily;
            InputMethod = inputMethod;
            MaxWindowSize = new Size(maxWidth, maxHeight);
            MinWindowSize = new Size(minWidth, minHeight);
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

        /// <summary>
        /// The maximum window size for the shell
        /// </summary>
        public Size MaxWindowSize { get; }

        /// <summary>
        /// The minimum window size for the shell
        /// </summary>
        public Size MinWindowSize { get; }
    }
}
