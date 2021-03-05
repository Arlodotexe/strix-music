using System;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Extension methods for a <see cref="string"/>.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Hash a string with MD5, using 16 bytes instead of 32 for speed. Collisions are more likely.
        /// </summary>
        /// <param name="seed">Input string.</param>
        /// <returns>A 16-bit MD5 hash.</returns>
        public static string HashMD5Fast(this string seed)
        {
            unchecked
            {
                int hash = 23;

                foreach (char c in seed)
                {
                    hash = hash * 31 + c;
                }

                return Convert.ToString(hash, 16);
            }
        }
    }
}