using System;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Provides extension methods for operating on arbitrary types.
    /// </summary>
    public static partial class GenericExtensions
    {
        /// <summary>
        /// Cast from one type to another.
        /// </summary>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <returns>The same object, cast to the requested type.</returns>
        public static TTarget Cast<TTarget>(this object obj)
         where TTarget : class
        {
            return (TTarget)obj;
        }
    }
}