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
        public static int GetHue(this Color color)
        {
            double delta = GetDelta(color);

            if (delta == 0)
                return 0;

            double r = (double)color.R / 255;
            double g = (double)color.G / 255;
            double b = (double)color.B / 255;

            switch (GetCMaxChannel(color))
            {
                case RGBChannel.R:
                    return (int)(60 * (((g - b) / delta) % 6));
                case RGBChannel.G:
                    return (int)(60 * (((b - r) / delta) + 2));
                case RGBChannel.B:
                    return (int)(60 * (((r - g) / delta) + 4));
            }

            return 0; // Not possible
        }

        /// <summary>
        /// Gets the Saturation of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Saturation.</returns>
        public static float GetSaturation(this Color color)
        {
            float value = color.GetValue();

            if (value == 0)
            {
                return 0;
            }

            float delta = GetDelta(color);
            return delta / value;
        }

        /// <summary>
        /// Gets the Value of a <see cref="Color"/> in hex format.
        /// </summary>
        /// <returns>The hex Value.</returns>
        public static float GetValue(this Color color)
        {
            return (float)GetRawCMax(color) / 255;
        }

        /// <summary>
        /// Adjusts the Value of the <see cref="Color"/>.
        /// </summary>
        /// <returns>A <see cref="Color"/> with the same Hue and Saturation of this, but a value of <paramref name="value"/>.</returns>
        public static Color AdjustValue(this Color color, float value)
        {
            float oldValue = GetValue(color);
            if (oldValue == 0)
                return Color.FromArgb(color.A, (byte)value, (byte)value, (byte)value);

            float adjustmentFactor = value / oldValue;
            byte r = (byte)(color.R * adjustmentFactor);
            byte g = (byte)(color.G * adjustmentFactor);
            byte b = (byte)(color.B * adjustmentFactor);
            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// Gets the value of the smallest RGB channel.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The value of the smallest channel.</returns>
        internal static float GetCMin(this Color color)
        {
            return (float)GetRawCMin(color) / 255;
        }

        private static float GetDelta(this Color color)
        {
            return GetValue(color) - GetCMin(color);
        }

        private static int GetRawCMax(this Color color)
        {
            int max = Math.Max(color.R, color.G);
            return Math.Max(max, color.B);
        }

        private static int GetRawCMin(this Color color)
        {
            int min = Math.Min(color.R, color.G);
            return Math.Min(min, color.B);
        }

        private static RGBChannel GetCMaxChannel(this Color color)
        {
            int maxValue = GetRawCMax(color);
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
