using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Models;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <summary>
    /// A <see langword="class"/> containing keys for all settings throughout the main app.
    /// </summary>
    /// <remarks>
    /// The StrixMusic.Sdk contains the keys that don't return anything UI-dependent, while StrixMusic.Sdk.Uno contains a partial that has the UI-dependent keys.
    /// </remarks>
    public class SettingsKeysUI : SettingsKeysBase
    {
        /// <summary>
        /// Stores the registry id of the user's preferred shell.
        /// </summary>
        public static string PreferredShell => Shells.Sandbox.Registration.Metadata.Id;

        /// <summary>
        /// The registry id of the user's current fallback shell. Used to cover display sizes that the <see cref="PreferredShell"/> doesn't support. 
        /// </summary>
        public static string FallbackShell => Shells.Sandbox.Registration.Metadata.Id;

        /// <summary>
        /// Contains keyed information for the last selected Pivot item in various pivots throughout the app. Key is a unique ID for the pivot, value is the index of the selected pivot.
        /// </summary>
        public static Dictionary<string, int> PivotSelectionMemo => new Dictionary<string, int>();

        /// <inheritdoc/>
        public override object GetDefaultValue(string settingKey)
        {
            return settingKey switch
            {
                nameof(PreferredShell) => PreferredShell,
                nameof(FallbackShell) => FallbackShell,
                nameof(PivotSelectionMemo) => PivotSelectionMemo,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(settingKey)),
            };
        }
    }
}