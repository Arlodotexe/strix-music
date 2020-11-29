using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{

    /// <summary>
    /// Provides extension methods for operating on arbitrary types.
    /// </summary>
    public static partial class GenericExtensions
    {
        /// <summary>
        /// Wraps any object in a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>A <see cref="List{T}"/> containing the source item.</returns>
        public static List<T> IntoList<T>(this T obj)
        {
            return new List<T>() { obj };
        }
    }
}
