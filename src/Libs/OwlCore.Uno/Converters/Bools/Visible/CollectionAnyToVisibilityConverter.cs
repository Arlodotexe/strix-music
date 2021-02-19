using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace OwlCore.Uno.Converters.Bools.Visible
{
    /// <summary>
    /// A simple converter that converts a given <see cref="ICollection"/> to an <see cref="Visibility"/> based on the presence of any items in the <see cref="ICollection"/>.
    /// </summary>
    public sealed class CollectionAnyToVisibilityConverter
    {
        /// <summary>
        /// Converts an <see cref="ICollection"/> an <see cref="Visibility"/> based on the presence of any items in the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="value">The <see cref="ICollection"/>.</param>
        /// <returns>A <see cref="Visibility"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Visibility Convert(ICollection value)
        {
            return BoolToVisibilityConverter.Convert(CollectionAnyToBoolConverter.Convert(value));
        }
    }
}
