using OwlCore.Uno.ColorExtractor.ColorSpaces;
using Windows.UI;

namespace OwlCore.Uno.ColorExtractor.Filters
{
    public class SaturationFilter : IFilter
    {
        public SaturationFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        float Tolerance { get; set; } = .6f;

        public RGBColor Clamp(RGBColor color)
        {
            if (color.GetSaturation() > Tolerance)
            {
                return Clamp(color.ToHsv()).ToRgb();
            }
            return color;
        }

        public HSVColor Clamp(HSVColor color)
        {
            if (color.S > Tolerance)
            {
                color.S = Tolerance;
            }
            return color;
        }

        public bool TakeColor(RGBColor color)
        {
            return color.GetSaturation() <= Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return color.S <= Tolerance;
        }
    }
}
