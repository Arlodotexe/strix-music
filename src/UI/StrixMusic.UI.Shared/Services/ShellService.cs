using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;

namespace StrixMusic.Shared.Services
{
    /// <summary>
    /// Constants and Assembly data accessed throughout the library.
    /// </summary>
    public class ShellService : IShellService
    {
        /// <summary>
        /// Creates a new instance of <see cref="ShellService"/>.
        /// </summary>
        public ShellService()
        {
            SetupLoadedShells();
        }

        /// <summary>
        /// The prefix of the resources path.
        /// </summary>
        public string ResourcesPrefix { get; } = "ms-appx:///";

        /// <summary>
        /// The <see cref="Regex"/> to determine if an assembly is a shell by the fullname.
        /// </summary>
        public string ShellAssemblyRegex { get; } = @"^(?:StrixMusic\.Shells\.)(\w{3,})[^.]";

        /// <summary>
        /// The <see cref="Regex"/> to determine if an assembly is a shell by the fullname.
        /// </summary>
        public string ShellResourceDictionaryRegex { get; } = @"^(?:ms-appx:\/\/\/StrixMusic\.Shells\.)(\w{3,})(?:\/Resources\.xaml)$";

        /// <summary>
        /// The name of the DefaultShell.
        /// </summary>
        public string DefaultShellAssemblyName { get; } = "Default";

        /// <summary>
        /// The name of the DefaultShell.
        /// </summary>
        public string DefaultShellDisplayName { get; } = "Default Shell";

        /// <summary>
        /// The namespace before a shell.
        /// </summary>
        public string ShellNamespacePrefix { get; } = "StrixMusic.Shells";

        /// <summary>
        /// The name of the resources fill for a shell.
        /// </summary>
        public string ShellResourcesSuffix { get; } = "Resources.xaml";

        /// <summary>
        /// The <see cref="ShellModel"/> of the DefaultShell.
        /// </summary>
        public ShellModel DefaultShellModel => LoadedShells[DefaultShellAssemblyName];

        /// <summary>
        /// Gets the loaded shells in the <see cref="Assembly"/> AssemblyName and DisplayName.
        /// </summary>
        public Dictionary<string, ShellModel> LoadedShells { get; } = new Dictionary<string, ShellModel>();

        private void SetupLoadedShells()
        {
            LoadedShells.Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                // Check if the assembly name is shell.
                Match match = Regex.Match(assembly.FullName, ShellAssemblyRegex);
                if (match.Success)
                {
                    // Gets the AssemblyName of the shell from the Regex.
                    string assemblyName = match.Groups[1].Value;

                    // Find the ShellName attribute
                    ShellAttribute shellAttribute = assembly.GetCustomAttribute<ShellAttribute>();
                    LoadedShells.Add(assemblyName, new ShellModel(match.Groups[1].Value, shellAttribute));
                }
            }
        }
    }
}
