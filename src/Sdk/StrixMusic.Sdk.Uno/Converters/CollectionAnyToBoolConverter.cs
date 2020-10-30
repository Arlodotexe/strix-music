using System.Collections;
using Uno.Extensions.Specialized;

namespace StrixMusic.Sdk.Uno.Converters
{
    /// <summary>
    /// A simple converter that converts a given <see cref="ICollection"/> to an bool based on the presence of any items in the <see cref="ICollection"/>.
    /// </summary>
    public sealed class CollectionAnyToBoolConverter
    {
        /// <summary>
        /// Converts an <see cref="ICollection"/> an <see cref="Visibility"/> based on the presence of any items in the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="value">The <see cref="ICollection"/>.</param>
        /// <returns>A <see cref="Visibility"/>.</returns>
        public static bool Convert(object value)
        {
            if (value is ICollection collection)
            {
                return collection.Any();
            }

            return false;
        }
    }
}
