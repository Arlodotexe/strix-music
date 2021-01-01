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
        public static byte GetHexHue(this Color color)
        {
            int delta = GetDelta(color);

            if (delta == 0)
                return 0;

            switch (GetCMaxChannel(color))
            {
                case RGBChannel.R:
                    return (byte)(60 * ((color.G - color.B) / delta) % 6);
                case RGBChannel.G:
                    return (byte)(60 * ((color.B - color.R) / delta) + 2);
                case RGBChannel.B:
                    return (byte)(60 * ((color.R - color.G) / delta) + 4);
            }

            return 0; // Not possible
        }

        /// <summary>
        /// Gets the Saturation of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Saturation.</returns>
        public static byte GetSaturation(this Color color)
        {
            int value = color.GetValue();

            if (value == 0)
            {
                return 0;
            }

            int delta = GetDelta(color) * 255;
            return (byte)(delta / value);
        }

        /// <summary>
        /// Gets the Value of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Value.</returns>
        public static byte GetValue(this Color color)
        {
            int max = Math.Max(color.R, color.G);
            return (byte)Math.Max(max, color.B);
        }

        /// <summary>
        /// Adjusts the Value of the <see cref="Color"/>.
        /// </summary>
        /// <returns>A <see cref="Color"/> with the same Hue and Saturation of this, but a value of <paramref name="value"/>.</returns>
        public static Color AdjustValue(this Color color, byte value)
        {
            byte oldValue = GetValue(color);
            if (oldValue == 0)
                return Color.FromArgb(color.A, value, value, value);

            float adjustmentFactor = (float)value / oldValue;
            byte r = (byte)(color.R * adjustmentFactor);
            byte g = (byte)(color.G * adjustmentFactor);
            byte b = (byte)(color.B * adjustmentFactor);
            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// Adjusts the Value of the <see cref="Color"/>.
        /// </summary>
        /// <returns>A <see cref="Color"/> with the same Hue and Saturation of this, but a value of <paramref name="value"/>.</returns>
        public static Color AdjustValue(this Color color, double value)
        {
            return AdjustValue(color, (byte)(255 * value));
        }

        private static byte GetCMin(this Color color)
        {
            int min = Math.Min(color.R, color.G);
            return (byte)(Math.Min(min, color.B));
        }

        private static byte GetDelta(this Color color)
        {
            return (byte)(GetValue(color) - GetCMin(color));
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
