using OwlCore.Uno.ColorExtractor.ColorSpaces;
using System;

namespace OwlCore.Uno.ColorExtractor
{
    public static class HSVExtensions
    {
        public static RGBColor ToRgb(this HSVColor color)
        {
            double hf = color.H / 60.0;
            int i = (int)Math.Floor(hf);
            double f = hf - i;
            double pv = color.V * (1 - color.S);
            double qv = color.V * (1 - color.S * f);
            double tv = color.V * (1 - color.S * (1 - f));

            double r, g, b;
            r = g = b = 0;

            switch (i)
            {
                case 0: // Red is the dominant color
                    r = color.V;
                    g = tv;
                    b = pv;
                    break;
                case 1: // Green is the dominant color
                    r = qv;
                    g = color.V;
                    b = pv;
                    break;
                case 2: // Green is the dominant color
                    r = pv;
                    g = color.V;
                    b = tv;
                    break;
                case 3: // Blue is the dominant color
                    r = pv;
                    g = qv;
                    b = color.V;
                    break;
                case 4: // Blue is the dominant color
                    r = tv;
                    g = pv;
                    b = color.V;
                    break;
                case 5: // Red is the dominant color
                    r = color.V;
                    g = pv;
                    b = qv;
                    break;
                // Extras
                case 6:
                    r = color.V;
                    g = tv;
                    b = pv;
                    break;
                case -1:
                    r = color.V;
                    g = pv;
                    b = qv;
                    break;
            }

            return new RGBColor()
            {
                R = (byte)Clamp((int)(r * 255.0)),
                G = (byte)Clamp((int)(g * 255.0)),
                B = (byte)Clamp((int)(b * 255.0)),
            };
        }

        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}

