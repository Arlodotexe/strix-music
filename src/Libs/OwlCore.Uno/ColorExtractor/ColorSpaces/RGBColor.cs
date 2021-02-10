using System;
using Windows.UI;

namespace OwlCore.Uno.ColorExtractor.ColorSpaces
{
    /// <summary>
    /// A Color in RGB colorspace.
    /// </summary>
    public struct RGBColor : IEquatable<RGBColor>
    {
        /// <summary>
        /// Gets a <see cref="RGBColor"/> from a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>The resulting <see cref="RGBColor"/>.</returns>
        public static RGBColor FromColor(Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            return new RGBColor(r, g, b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RGBColor"/> struct.
        /// </summary>
        public RGBColor(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// The Red channel of the RGB color.
        /// </summary>
        /// <remarks>
        /// Values range from 0-1.
        /// </remarks>
        public float R { get; set; }

        /// <summary>
        /// The Green channel of the RGB color.
        /// </summary>
        /// <remarks>
        /// Values range from 0-1.
        /// </remarks>
        public float G { get; set; }

        /// <summary>
        /// The Blue channel of the RGB color.
        /// </summary>
        /// <remarks>
        /// Values range from 0-1.
        /// </remarks>
        public float B { get; set; }

        /// <inheritdoc/>
        public bool Equals(RGBColor other)
        {
            return R == other.R &&
                   G == other.G &&
                   B == other.B;
        }
    }
}
