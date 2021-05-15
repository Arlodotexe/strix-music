using System.Collections.Generic;
using StrixMusic.Sdk.Uno.Models;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the main app.
    /// </summary>
    /// <remarks>
    /// The StrixMusic.Sdk contains the keys that don't return anything UI-dependent, while StrixMusic.Sdk.Uno contains a partial that has the UI-dependent keys.
    /// </remarks>
    public static partial class SettingsKeysUI
    {
        /// <summary>
        /// Stores the assembly name of the user's preferred shell.
        /// </summary>
        public static readonly string PreferredShell = "Default";

        /// <summary>
        /// The assembly name of the user's current fallback shell. Used to cover display sizes that the <see cref="PreferredShell"/> doesn't support. 
        /// </summary>
        public static readonly string FallbackShell = "Default";

        /// <summary>
        /// Stored assembly information about all available shells.
        /// </summary>
        public static readonly IReadOnlyList<ShellAssemblyInfo> ShellRegistry = new List<ShellAssemblyInfo>();

        /// <summary>
        /// Contains keyed information for the last selected Pivot item in various pivots throughout the app. Key is a unique ID for the pivot, value is the index of the selected pivot.
        /// </summary>
        public static readonly Dictionary<string, int> PivotSelectionMemo = new Dictionary<string, int>();
    }
}