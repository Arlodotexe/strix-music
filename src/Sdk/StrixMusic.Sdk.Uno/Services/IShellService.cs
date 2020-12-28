using System.Collections.Generic;
using System.Text.RegularExpressions;
using StrixMusic.Sdk.Uno.Models;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <summary>
    /// Information about the loaded shells
    /// </summary>
    public interface IShellService
    {
        /// <summary>
        /// The prefix of the resources path.
        /// </summary>
        string ResourcesPrefix { get; }

        /// <summary>
        /// The <see cref="Regex"/> to determine if an assembly is a shell by the fullname.
        /// </summary>
        string ShellAssemblyRegex { get; }

        /// <summary>
        /// The <see cref="Regex"/> to determine if an assembly is a shell by the fullname.
        /// </summary>
        string ShellResourceDictionaryRegex { get; }

        /// <summary>
        /// The name of the DefaultShell.
        /// </summary>
        string DefaultShellAssemblyName { get; }

        /// <summary>
        /// The name of the DefaultShell.
        /// </summary>
        string DefaultShellDisplayName { get; }

        /// <summary>
        /// The namespace before a shell.
        /// </summary>
        string ShellNamespacePrefix { get; }

        /// <summary>
        /// The name of the resources fill for a shell.
        /// </summary>
        string ShellResourcesSuffix { get; }

        /// <summary>
        /// The <see cref="ShellModel"/> of the DefaultShell.
        /// </summary>
        ShellModel DefaultShellModel { get; }

        /// <summary>
        /// The shells that are currently loaded, keyed by the AssemblyName.
        /// </summary>
        Dictionary<string, ShellModel> LoadedShells { get; }
    }
}