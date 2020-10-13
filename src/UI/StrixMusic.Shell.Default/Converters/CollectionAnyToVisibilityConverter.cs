using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace StrixMusic.Shell.Default.Converters
{
    /// <summary>
    /// A simple converter that converts a given <see cref="ICollection{T}"/> to an <see cref="Visibility"/> based on the presence of any items in the <see cref="ICollection{T}"/>.
    /// </summary>
    public sealed class CollectionAnyToVisibilityConverter
    {
        /// <summary>
        /// Converts an <see cref="ICollection{T}"/> an <see cref="Visibility"/> based on the presence of any items in the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="value">The <see cref="ICollection{T}"/>.</param>
        /// <returns>A <see cref="Visibility"/>.</returns>
        public static Visibility Convert(object value)
        {
            if (value is ICollection<object> collection)
            {
                return collection.Any() ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }
    }
}
