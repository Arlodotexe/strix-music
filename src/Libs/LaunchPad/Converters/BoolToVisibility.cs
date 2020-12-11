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
        /// Returns visible if the boolean is true.
        /// </summary>
        /// <param name="data">boolean to check.</param>
        /// <returns>Collapsed if false, otherwise Visible.</returns>
        [Pure]
        public static Visibility BoolToVisibility(bool data) => data ? Visibility.Visible : Visibility.Collapsed;
    }
}