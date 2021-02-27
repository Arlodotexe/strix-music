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
        /// Returns hash of a string (based on MD5, but only 16 instead of 32 bytes).
        /// </summary>
        /// <param name="seed">Input string.</param>
        /// <returns>MD5 hash.</returns>
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