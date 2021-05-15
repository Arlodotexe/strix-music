using StrixMusic.Sdk.Uno.Assembly.Enums;
using Windows.Foundation;

namespace StrixMusic.Sdk.Uno.Models
{
    /// <summary>
    /// The data given to the attribute in the AssemblyInfo.cs file.
    /// </summary>
    public class ShellAttributeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellAttributeData"/> class.
        /// </summary>
        /// <param name="shellClassAssemblyQualifiedName">The full name of the Shell child Type.</param>
        /// <param name="displayName">The display name for the shell.</param>
        /// <param name="description"> A brief summary of the shell that will be displayed to the user.</param>
        /// <param name="deviceFamily">The supported device families for the shell.</param>
        /// <param name="inputMethod">The supported input methods.</param>
        /// <param name="maxWidth">The maximum width of the window for the shell.</param>
        /// <param name="maxHeight">The maximum height of the window for the shell.</param>
        /// <param name="minWidth">The minimum width of the window for the shell.</param>
        /// <param name="minHeight">The minimum height of the window for the shell.</param>
        public ShellAttributeData(
            string shellClassAssemblyQualifiedName,
            string displayName,
            string description,
            DeviceFamily deviceFamily = (DeviceFamily)int.MaxValue,
            InputMethod inputMethod = (InputMethod)int.MaxValue,
            double maxWidth = double.MaxValue,
            double maxHeight = double.MaxValue,
            double minWidth = 0,
            double minHeight = 0)
        {
            ShellTypeAssemblyQualifiedName = shellClassAssemblyQualifiedName;
            DisplayName = displayName;
            Description = description;
            DeviceFamily = deviceFamily;
            InputMethod = inputMethod;
            MaxWindowSize = new Size(maxWidth, maxHeight);
            MinWindowSize = new Size(minWidth, minHeight);
        }

        /// <summary>
        /// The full name of the ShellBase child Type.
        /// </summary>
        public string ShellTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A brief summary of the shell that will be displayed to the user.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public DeviceFamily DeviceFamily { get; set; }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public InputMethod InputMethod { get; set; }

        /// <summary>
        /// The maximum window size for the shell
        /// </summary>
        public Size MaxWindowSize { get; set; }

        /// <summary>
        /// The minimum window size for the shell
        /// </summary>
        public Size MinWindowSize { get; set; }
    }
}
