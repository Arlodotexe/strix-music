using System.Globalization;
using System.Linq;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Extensions methods for a <see cref="CultureInfo"/>.
    /// </summary>
    public static partial class CultureInfoExtensions
    {
        /// <summary>
        /// Converts a three letter ISO 636-3 language name to a <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="name">The three letter ISO 636-3 language name.</param>
        /// <returns>The corresponding <see cref="CultureInfo"/>, or <see langword="null"/> if not found.</returns>
        public static CultureInfo? FromIso636_3(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(c => c.ThreeLetterISOLanguageName == name);
        }
    }
}
