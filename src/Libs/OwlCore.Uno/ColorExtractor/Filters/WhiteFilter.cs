using OwlCore.Uno.ColorExtractor.ColorSpaces;

namespace OwlCore.Uno.ColorExtractor.Filters
{
    public class WhiteFilter : IFilter
    {
        public WhiteFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        float Tolerance { get; set; } = .4f;

        public RGBColor Clamp(RGBColor color)
        {
            // Unclampable
            return color;
        }

        public HSVColor Clamp(HSVColor color)
        {
            // Unclampable
            return color;
        }

        public bool TakeColor(RGBColor color)
        {
            return color.GetCMin() < Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return TakeColor(color.ToRgb());
        }
    }
}
