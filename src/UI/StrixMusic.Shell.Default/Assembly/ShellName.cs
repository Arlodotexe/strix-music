using System;

namespace StrixMusic.Shell.Default.Assembly
{
    /// <summary>
    /// An attribute for the shell's name.
    /// </summary>
    public class ShellName : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellName"/> class.
        /// </summary>
        /// <param name="shellName"></param>
        public ShellName(string shellName)
        {
            ShellDisplayName = shellName;
        }

        /// <summary>
        /// The DisplayName of the shell in the assembly.
        /// </summary>
        public string ShellDisplayName { get; }
    }
}
