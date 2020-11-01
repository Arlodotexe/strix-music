using System;
using System.Collections.Generic;
using System.Text;

namespace Windows.UI
{
    /// <summary>
    /// A collection of extension methods.
    /// </summary>
    public static class ColorExtensions
    {
        private enum RGBChannel
        { R, G, B }

        /// <summary>
        /// Gets the Hue of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Hue.</returns>
        public static int GetHexHue(this Color color)
        {
            int delta = GetDelta(color);

            if (delta == 0)
                return 0;

            switch (GetCMaxChannel(color))
            {
                case RGBChannel.R:
                    return 60 * ((color.G - color.B) / delta) % 6;
                case RGBChannel.G:
                    return 60 * ((color.B - color.R) / delta) + 2;
                case RGBChannel.B:
                    return 60 * ((color.R - color.G) / delta) + 4;
            }

            return 0; // Not possible
        }

        /// <summary>
        /// Gets the Saturation of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Saturation.</returns>
        public static int GetSaturation(this Color color)
        {
            int value = color.GetValue();
            return GetDelta(color) / value;
        }

        /// <summary>
        /// Gets the Value of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Value.</returns>
        public static int GetValue(this Color color)
        {
            int max = Math.Max(color.R, color.G);
            return Math.Max(max, color.B);
        }

        private static int GetCMin(this Color color)
        {
            int min = Math.Min(color.R, color.G);
            return Math.Min(min, color.B);
        }

        private static int GetDelta(this Color color)
        {
            return GetValue(color) - GetCMin(color);
        }

        private static RGBChannel GetCMaxChannel(this Color color)
        {
            int maxValue = GetValue(color);
            if (maxValue == color.R)
                return RGBChannel.R;
            if (maxValue == color.G)
                return RGBChannel.G;
            if (maxValue == color.B)
                return RGBChannel.B;

            return RGBChannel.R;
        }
    }
}
