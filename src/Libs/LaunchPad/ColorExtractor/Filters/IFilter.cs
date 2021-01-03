using LaunchPad.ColorExtraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ColorExtractor.ColorExtractor.Filters
{
    public interface IFilter
    {
        Color Clamp(Color color);

        HSVColor Clamp(HSVColor color);

        bool TakeColor(Color color);

        bool TakeColor(HSVColor color);
    }
}
