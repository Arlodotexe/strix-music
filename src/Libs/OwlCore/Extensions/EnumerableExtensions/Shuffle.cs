using System;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Enumerable-related extension methods.
    /// </summary>
    public static partial class EnumerableExtensions
    {
        private static readonly Random _rng = new Random();

        /// <summary>Shuffles the given array in place.</summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        public static void Shuffle<T>(this T[] array)
        {
            for (var n = array.Length; n > 1;)
            {
                var k = _rng.Next(n);
                --n;
                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}