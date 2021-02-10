using System.Diagnostics.Contracts;
using Windows.UI.Xaml;

namespace OwlCore.Uno
{
    /// <summary>
    /// Contains various converters that can be used in x:Bind functions.
    /// </summary>
    public static partial class Converters
    {
        /// <summary>
        /// Returns visible if the boolean is false.
        /// </summary>
        /// <param name="data">boolean to check.</param>
        /// <returns>Collapsed if true, otherwise Visible.</returns>
        [Pure]
        public static Visibility NotBoolToVisibility(bool data) => data ? Visibility.Collapsed : Visibility.Visible;
    }
}