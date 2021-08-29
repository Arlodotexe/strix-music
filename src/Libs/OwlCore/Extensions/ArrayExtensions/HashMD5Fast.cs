using System;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Array"/>s.
    /// </summary>
    public static partial class ArrayExtensions
    {
        /// <summary>
        /// Hash a byte array with MD5, using 16 bytes instead of 32 for speed. Collisions are more likely.
        /// </summary>
        /// <param name="array">Input array.</param>
        /// <returns>A 16-bit MD5 hash.</returns>
        public static string HashMD5Fast(this byte[] array)
        {
            unchecked
            {
                int hash = 23;

                foreach (byte c in array)
                {
                    hash = hash * 31 + c;
                }

                return Convert.ToString(hash, 16);
            }
        }
    }
}
