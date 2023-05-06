using System;

namespace OwlCore.WinUI.ColorExtractor.ColorSpaces
{
    /// <summary>
    /// A Color in HSV colorspace.
    /// </summary>
    public struct HSVColor : IEquatable<HSVColor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HSVColor"/> struct.
        /// </summary>
        public HSVColor(int h, float s, float v)
        {
            H = h;
            S = s;
            V = v;
        }

        /// <summary>
        /// The Hue channel of the HSV color.
        /// </summary>
        /// <remarks>
        /// Values range from 0-360
        /// </remarks>
        public int H { get; set; }

        /// <summary>
        /// The Saturation channel of the HSV color.
        /// </summary>
        /// <remarks>
        /// Values range from 0-1.
        /// </remarks>
        public float S { get; set; }

        /// <summary>
        /// The Value channel of the HSV color.
        /// </summary>
        /// <remarks>
        /// Values range from 0-1.
        /// </remarks>
        public float V { get; set; }

        /// <inheritdoc/>
        public bool Equals(HSVColor other)
        {
            return H == other.H &&
                   S == other.S &&
                   V == other.V;
        }
    }
}
