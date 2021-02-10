using ClusterNet.Shapes;
using OwlCore.Uno.ColorExtractor.ColorSpaces;

namespace OwlCore.Uno.ColorExtractor.Shapes
{
    public struct RGBShape : IPoint<RGBColor>
    {
        public RGBColor Average(RGBColor[] items)
        {
            float sumR, sumG, sumB;
            sumR = sumG = sumB = 0;
            int count = 0;
            foreach (var item in items)
            {
                sumR += item.R;
                sumG += item.G;
                sumB += item.B;
                count++;
            }

            RGBColor color = new RGBColor()
            {
                R = (float)(sumR / count),
                G = (float)(sumG / count),
                B = (float)(sumB / count),
            };

            return color;
        }

        public double FindDistanceSquared(RGBColor it1, RGBColor it2)
        {
            float r = it1.R - it2.R;
            float g = it1.G - it2.G;
            float b = it1.B - it2.B;

            return r * r + g * g + b * b;
        }

        public RGBColor WeightedAverage((RGBColor, double)[] items)
        {
            double sumR, sumG, sumB;
            sumR = sumG = sumB = 0;
            double totalWeight = 0;
            foreach (var item in items)
            {
                sumR += item.Item1.R * item.Item2;
                sumG += item.Item1.G * item.Item2;
                sumB += item.Item1.B * item.Item2;
                totalWeight += item.Item2;
            }

            RGBColor color = new RGBColor()
            {
                R = (float)(sumR / totalWeight),
                G = (float)(sumG / totalWeight),
                B = (float)(sumB / totalWeight),
            };
            return color;
        }
    }
}
