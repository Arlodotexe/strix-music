using OwlCore.WinUI.ColorExtractor.ColorSpaces;
using Windows.UI;

namespace OwlCore.WinUI.ColorExtractor.Filters
{
    public interface IFilter
    {
        RGBColor Clamp(RGBColor color);

        HSVColor Clamp(HSVColor color);

        bool TakeColor(RGBColor color);

        bool TakeColor(HSVColor color);
    }
}
