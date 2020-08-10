using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
            private static IEnumerable<string>? _loadedShells = null;

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
            public const string DefaultShellName = "Default";

            /// <summary>
            /// The namespace before a shell.
            /// </summary>
            public const string ShellNamespacePrefix = "StrixMusic.Shell";

            /// <summary>
            /// The name of the resources fill for a shell.
            /// </summary>
            public const string ShellResourcesSuffix = "Resources.xaml";

            /// <summary>
            /// Gets the loaded shells in the <see cref="Assembly"/>.
            /// </summary>
            public static IEnumerable<string> LoadedShells
            {
                get
                {
                    if (_loadedShells == null)
                    {
                        List<string> loadedShells = new List<string>();
                        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        foreach (Assembly assembly in assemblies)
                        {
                            Match match = Regex.Match(assembly.FullName, ShellAssemblyRegex);
                            if (match.Success)
                            {
                                loadedShells.Add(match.Groups[1].Value);
                            }
                        }

                        _loadedShells = loadedShells.AsEnumerable();
                    }

                    return _loadedShells!;
                }
            }
        }
    }
}
