﻿using System;
using Windows.UI;

namespace LaunchPad.ColorExtraction
{
    /// <summary>
    /// A Color stored in HSV space.
    /// </summary>
    public class HSVColor
    {
        /// <summary>
        /// Gets an <see cref="HSVColor"/> from the A, H, S and V channels.
        /// </summary>
        /// <param name="a">The Alpha channel.</param>
        /// <param name="h">The Hue channel.</param>
        /// <param name="s">The Saturation channel.</param>
        /// <param name="v">The Value channel.</param>
        /// <returns>The <see cref="HSVColor"/> for the parameters.</returns>
        public static HSVColor FromAhsv(byte a, int h, float s, float v)
        {
            return new HSVColor(a, h, s, v);
        }

        /// <summary>
        /// Gets an <see cref="HSVColor"/> from the A, R, G and B channels.
        /// </summary>
        /// <param name="a">The Alpha channel.</param>
        /// <param name="r">The Red channel.</param>
        /// <param name="g">The Green channel.</param>
        /// <param name="b">The Blue channel.</param>
        /// <returns>The <see cref="HSVColor"/> for the parameters.</returns>
        public static HSVColor FromArgb(byte a, byte r, byte g, byte b)
        {
            return FromColor(Color.FromArgb(a, r, g, b));
        }

        /// <summary>
        /// Gets an <see cref="HSVColor"/> from a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The RGB <see cref="Color"/>.</param>
        /// <returns>The <see cref="HSVColor"/> equivelant for the <see cref="Color"/>.</returns>
        public static HSVColor FromColor(Color color)
        {
            int h = color.GetHexHue();
            float s = color.GetSaturation();
            float v = color.GetValue();
            return new HSVColor(color.A, h, s, v);
        }

        /// <summary>
        /// Returns a clone of an <see cref="HSVColor"/>.
        /// </summary>
        /// <param name="c">The original <see cref="HSVColor"/>.</param>
        /// <returns>A new <see cref="HSVColor"/>.</returns>
        public static HSVColor Clone(HSVColor c)
        {
            return FromAhsv(c.A, c.H, c.S, c.V);
        }

        private HSVColor(byte a, int h, float s, float v)
        {
            A = a;
            H = h;
            S = s;
            V = v;
        }

        /// <summary>
        /// The value of the Alpha channel.
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// The value of the Hue channel.
        /// </summary>
        public int H { get; }

        /// <summary>
        /// The value of the Saturation channel.
        /// </summary>
        public float S { get; }

        /// <summary>
        /// The value of the Value channel.
        /// </summary>
        public float V { get; }

        /// <summary>
        /// Gets the <see cref="HSVColor"/> as a <see cref="Color"/>.
        /// </summary>
        /// <returns>A <see cref="Color"/>.</returns>
        public Color AsArgb()
        {
            double c = V * S;
            double x = c * (1 - Math.Abs(((H / 60) % 2) - 1));
            double m = V - c;

            double r = 0;
            double g = 0;
            double b = 0;

            if (H < 60)
            { r = c; g = x; }
            else if (H < 120)
            { r = x; g = c; }
            else if (H < 180)
            { g = c; b = x; }
            else if (H < 240)
            { g = x; b = c; }
            else if (H < 300)
            { r = x; b = c; }
            else if (H < 360)
            { r = c; b = x; }

            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            return Color.FromArgb(A, (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Finds the different between 2 <see cref="HSVColor"/>s.
        /// </summary>
        /// <param name="start">The first <see cref="HSVColor"/>.</param>
        /// <param name="end">The second <see cref="HSVColor"/>.</param>
        /// <returns>The distance between two <see cref="HSVColor"/>s.</returns>
        internal static double FindDistance(HSVColor start, HSVColor end)
        {
            double sDiff = Math.Abs(start.S - end.S);
            double vDiff = Math.Abs(start.V - end.V);

            // Calculate hue difference to wrap.
            double angle1 = Math.Abs(start.H - end.H);
            double angle2 = Math.Abs(start.H - end.H);
            double hDiff = Math.Min(angle1, angle2);

            double weigthedHueDiff = hDiff * hDiff * 1.5;
            double weigthedSatDiff = sDiff * sDiff;
            double weigthedValueDiff = vDiff * vDiff / 10;

            return weigthedHueDiff + weigthedSatDiff + weigthedValueDiff;
        }
    }
}
