using LaunchPad.ColorExtraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ColorExtractor.ColorExtractor.Filters
{
    public class WhiteFilter : IFilter
    {
        public WhiteFilter(float tolerance)
        {
            Tolerance = tolerance;
        }

        float Tolerance { get; set; } = .4f;

        public Color Clamp(Color color)
        {
            // Unclampable
            return color;
        }

        public HSVColor Clamp(HSVColor color)
        {
            // Unclampable
            return color;
        }

        public bool TakeColor(Color color)
        {
            return color.GetCMin() < Tolerance;
        }

        public bool TakeColor(HSVColor color)
        {
            return TakeColor(color.AsArgb());
        }
    }
}
