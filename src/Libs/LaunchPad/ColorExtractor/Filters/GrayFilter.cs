using LaunchPad.ColorExtraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI;

namespace ColorExtractor.ColorExtractor.Filters
{
    public class GrayFilter : IFilter
    {
        public GrayFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        public float Tolerance { get; set; } = .3f;

        public Color Clamp(Color color)
        {
            if (color.GetSaturation() < Tolerance)
            {
                return Clamp(HSVColor.FromColor(color)).AsArgb();
            }
            return color;
        }

        public HSVColor Clamp(HSVColor color)
        {
            if (color.S < Tolerance)
            {
                color.S = Tolerance;
            }
            return color;
        }

        public bool TakeColor(Color color)
        {
            return color.GetSaturation() > Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return color.S > Tolerance;
        }
    }
}
