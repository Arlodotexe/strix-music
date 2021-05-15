using StrixMusic.Sdk.Uno.Assembly.Enums;
using System;
using Windows.Foundation;

namespace StrixMusic.Sdk.Uno.Assembly
{
    /// <summary>
    /// An attribute used to dynamically import a shell and associated metadata.
    /// </summary>
    public class ShellAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAttribute"/> class.
        /// </summary>
        /// <param name="shellClass">The Shell child Type of the shell in the assembly.</param>
        /// <param name="displayName">The display name for the shell.</param>
        /// <param name="description"> A brief summary of the shell that will be displayed to the user.</param>
        /// <param name="deviceFamily">The supported device families for the shell.</param>
        /// <param name="inputMethod">The supported input methods.</param>
        /// <param name="maxWidth">The maximum width of the window for the shell.</param>
        /// <param name="maxHeight">The maximum height of the window for the shell.</param>
        /// <param name="minWidth">The minimum width of the window for the shell.</param>
        /// <param name="minHeight">The minimum height of the window for the shell.</param>
        public ShellAttribute(
            Type shellClass,
            string displayName,
            string description,
            DeviceFamily deviceFamily = (DeviceFamily)int.MaxValue,
            InputMethod inputMethod = (InputMethod)int.MaxValue,
            double maxWidth = double.PositiveInfinity,
            double maxHeight = double.PositiveInfinity,
            double minWidth = 0,
            double minHeight = 0)
        {
            ShellType = shellClass;
            DisplayName = displayName;
            Description = description;
            DeviceFamily = deviceFamily;
            InputMethod = inputMethod;
            MaxWindowSize = new Size(maxWidth, maxHeight);
            MinWindowSize = new Size(minWidth, minHeight);
        }

        /// <summary>
        /// The ShellBase child Type of the shell in the assembly.
        /// </summary>
        public Type ShellType { get; }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// A brief summary of the shell that will be displayed to the user.
        /// </summary>
        public string Description { get; }

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
