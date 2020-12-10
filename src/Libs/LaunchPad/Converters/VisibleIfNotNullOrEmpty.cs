using System.Diagnostics.Contracts;
using Windows.UI.Xaml;

namespace LaunchPad
{
    /// <summary>
    /// Contains various converters that can be used in x:Bind functions.
    /// </summary>
    public static partial class Converters
    {
        /// <summary>
        /// Returns visible if the string is not null or empty.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <returns>Collapsed if null or empty, otherwise Visible.</returns>
        [Pure]
        public static Visibility VisibleIfNotNullOrEmpty(string? str) => !string.IsNullOrWhiteSpace(str) ? Visibility.Visible : Visibility.Collapsed;
    }
}