using LaunchPad.ColorExtractor.ColorSpaces;

namespace LaunchPad.ColorExtractor.Filters
{
    public class BlackFilter : IFilter
    {
        public BlackFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        float Tolerance { get; set; } = .15f;

        public RGBColor Clamp(RGBColor color)
        {
            if (color.GetValue() < Tolerance)
            {
                color.AdjustValue(Tolerance);
            }
            return color;
        }

        public HSVColor Clamp(HSVColor color)
        {
            if (color.V < Tolerance)
            {
                color.V = Tolerance;
            }
            return color;
        }

        public bool TakeColor(RGBColor color)
        {
            return color.GetValue() > Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return color.V > Tolerance;
        }
    }
}
