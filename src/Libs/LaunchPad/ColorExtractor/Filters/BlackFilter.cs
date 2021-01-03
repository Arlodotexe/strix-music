using LaunchPad.ColorExtraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ColorExtractor.ColorExtractor.Filters
{
    public class BlackFilter : IFilter
    {
        public BlackFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        float Tolerance { get; set; } = .15f;

        public Color Clamp(Color color)
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

        public bool TakeColor(Color color)
        {
            return color.GetValue() > Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return color.V > Tolerance;
        }
    }
}
