using System;
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
            int h = color.GetHue();
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
        /// <remarks>
        /// http://www.splinter.com.au/converting-hsv-to-rgb-colour-using-c/
        /// </remarks>
        /// <returns>A <see cref="Color"/>.</returns>
        public Color AsArgb()
        {
            double hf = H / 60.0;
            int i = (int)Math.Floor(hf);
            double f = hf - i;
            double pv = V * (1 - S);
            double qv = V * (1 - S * f);
            double tv = V * (1 - S * (1 - f));

            double r, g, b;
            r = g = b = 0;

            switch (i)
            {
                case 0: // Red is the dominant color
                    r = V;
                    g = tv;
                    b = pv;
                    break;
                case 1: // Green is the dominant color
                    r = qv;
                    g = V;
                    b = pv;
                    break;
                case 2: // Green is the dominant color
                    r = pv;
                    g = V;
                    b = tv;
                    break;
                case 3: // Blue is the dominant color
                    r = pv;
                    g = qv;
                    b = V;
                    break;
                case 4: // Blue is the dominant color
                    r = tv;
                    g = pv;
                    b = V;
                    break;
                case 5: // Red is the dominant color
                    r = V;
                    g = pv;
                    b = qv;
                    break;
                // Extras
                case 6:
                    r = V;
                    g = tv;
                    b = pv;
                    break;
                case -1:
                    r = V;
                    g = pv;
                    b = qv;
                    break;
            }

            return Color.FromArgb(A,
                (byte)Clamp((int)(r * 255.0)),
                (byte)Clamp((int)(g * 255.0)),
                (byte)Clamp((int)(b * 255.0)));
        }

        private int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
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
            double angle2 = Math.Abs(end.H - start.H);
            double hDiff = Math.Min(angle1, angle2);

            double weigthedHueDiff = hDiff * hDiff * 1.5;
            double weigthedSatDiff = sDiff * sDiff;
            double weigthedValueDiff = vDiff * vDiff / 10;

            return weigthedHueDiff + weigthedSatDiff + weigthedValueDiff;
        }
    }
}
