using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using StrixMusic.Models;
using StrixMusic.Shell.Default.Assembly;

namespace StrixMusic.Helpers
{
    /// <summary>
    /// Constants and Assembly data accessed throughout the library.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The prefix of the resources path.
        /// </summary>
        public const string ResourcesPrefix = "ms-appx:///";

        /// <summary>
        /// An accessing class for constant information about Shells.
        /// </summary>
        public static class Shells
        {
            private static List<ShellModel>? _loadedShells = null;

            /// <summary>
            /// The <see cref="Regex"/> to determine if an assembly is a shell by the fullname.
            /// </summary>
            public const string ShellAssemblyRegex = @"^(?:StrixMusic\.Shell\.)(\w{3,})[^.]";

            /// <summary>
            /// The <see cref="Regex"/> to determine if an assembly is a shell by the fullname.
            /// </summary>
            public const string ShellResourceDictionaryRegex = @"^(?:ms-appx:\/\/\/StrixMusic\.Shell\.)(\w{3,})(?:\/Resources\.xaml)$";

            /// <summary>
            /// The name of the DefaultShell.
            /// </summary>
            public const string DefaultShellAssemblyName = "Default";

            /// <summary>
            /// The name of the DefaultShell.
            /// </summary>
            public const string DefaultShellDisplayName = "Default Shell";

            /// <summary>
            /// The namespace before a shell.
            /// </summary>
            public const string ShellNamespacePrefix = "StrixMusic.Shell";

            /// <summary>
            /// The name of the resources fill for a shell.
            /// </summary>
            public const string ShellResourcesSuffix = "Resources.xaml";

            /// <summary>
            /// Gets the loaded shells in the <see cref="Assembly"/> AssemblyName and DisplayName.
            /// </summary>
            public static IEnumerable<ShellModel> LoadedShells
            {
                get
                {
                    if (_loadedShells == null)
                    {
                        List<ShellModel> loadedShells = new List<ShellModel>();
                        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        foreach (Assembly assembly in assemblies)
                        {
                            // Check if the assembly name is shell.
                            Match match = Regex.Match(assembly.FullName, ShellAssemblyRegex);
                            if (match.Success)
                            {
                                // Default shellname is the namespace.
                                string displayName = match.Groups[1].Value;

                                // Check for the ShellName attribute and rip the displayname.
                                ShellName shellName = assembly.GetCustomAttribute<ShellName>();
                                if (shellName != null)
                                {
                                    displayName = shellName.ShellDisplayName;
                                }

                                loadedShells.Add(new ShellModel(match.Groups[1].Value, displayName));
                            }
                        }

                        _loadedShells = loadedShells;
                    }

                    return _loadedShells!;
                }
            }
        }
    }
}
