using OwlCore.WinUI.ColorExtractor.ColorSpaces;
using System;

namespace OwlCore.WinUI.ColorExtractor
{
    public static class RGBExtensions
    {
        private enum RGBChannel { R, G, B }

        /// <summary>
        /// Adjusts the Value of the <see cref="RGBColor"/>.
        /// </summary>
        /// <returns>A <see cref="RGBColor"/> with the same Hue and Saturation of this, but a value of <paramref name="value"/>.</returns>
        public static RGBColor AdjustValue(this RGBColor color, float value)
        {
            float oldValue = GetValue(color);
            if (oldValue == 0)
                return new RGBColor((byte)value, (byte)value, (byte)value);

            float adjustmentFactor = value / oldValue;
            byte r = (byte)(color.R * adjustmentFactor);
            byte g = (byte)(color.G * adjustmentFactor);
            byte b = (byte)(color.B * adjustmentFactor);
            return new RGBColor(r, g, b);
        }

        public static HSVColor ToHsv(this RGBColor color)
        {
            return new HSVColor()
            {
                H = color.GetHue(),
                S = color.GetSaturation(),
                V = color.GetValue(),
            };
        }

        public static int GetHue(this RGBColor color)
        {
            float delta = GetDelta(color);

            if (delta == 0)
                return 0;

            float r = color.R;
            float g = color.G;
            float b = color.B;

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

        public static float GetSaturation(this RGBColor color)
        {
            float value = color.GetValue();

            if (value == 0)
            {
                return 0;
            }

            float delta = GetDelta(color);
            return delta / value;
        }

        public static float GetValue(this RGBColor color)
        {
            return (float)GetRawCMax(color) / 255;
        }

        internal static float GetCMin(this RGBColor color)
        {
            return GetRawCMin(color);
        }

        private static float GetDelta(this RGBColor color)
        {
            return GetValue(color) - GetCMin(color);
        }

        private static float GetRawCMax(this RGBColor color)
        {
            float max = Math.Max(color.R, color.G);
            return Math.Max(max, color.B);
        }

        private static float GetRawCMin(this RGBColor color)
        {
            float min = Math.Min(color.R, color.G);
            return Math.Min(min, color.B);
        }

        private static RGBChannel GetCMaxChannel(this RGBColor color)
        {
            float maxValue = GetRawCMax(color);

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
