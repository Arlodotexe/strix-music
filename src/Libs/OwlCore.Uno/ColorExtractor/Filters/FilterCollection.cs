using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwlCore.Uno.ColorExtractor.ColorSpaces;

namespace OwlCore.Uno.ColorExtractor.Filters
{
    /// <summary>
    /// A Collection of <see cref="IFilter"/>s that can be applied to a color or list of colors.
    /// </summary>
    public class FilterCollection : List<IFilter>
    {
        /// <summary>
        /// Filters the color list with a minimum of <paramref name="minCount"/>.
        /// </summary>
        /// <param name="colors">The colors to filter.</param>
        /// <param name="minCount">The minimum output of colors.</param>
        /// <returns>The filtered color list.</returns>
        public RGBColor[] Filter(RGBColor[] colors, int minCount)
        {
            List<RGBColor> colorList = colors.ToList();
            foreach (var filter in this)
            {
                List<RGBColor> filteredColors = new List<RGBColor>();
                foreach(var color in colorList)
                {
                    if (filter.TakeColor(color))
                    {
                        filteredColors.Add(color);
                    }
                }

                if (filteredColors.Count < minCount)
                {
                    return colorList.ToArray();
                }
                colorList = filteredColors;
            }
            return colorList.ToArray();
        }

        /// <summary>
        /// Filters the color list with a minimum of <paramref name="minCount"/>.
        /// </summary>
        /// <param name="colors">The colors to filter.</param>
        /// <param name="minCount">The minimum output of colors.</param>
        /// <returns>The filtered color list.</returns>
        public HSVColor[] Filter(HSVColor[] colors, int minCount)
        {
            List<HSVColor> colorList = colors.ToList();
            foreach (var filter in this)
            {
                List<HSVColor> filteredColors = new List<HSVColor>();
                foreach (var color in colorList)
                {
                    if (filter.TakeColor(color))
                    {
                        filteredColors.Add(color);
                    }
                }

                if (filteredColors.Count < minCount)
                {
                    return colorList.ToArray();
                }
                colorList = filteredColors;
            }
            return colorList.ToArray();
        }

        /// <summary>
        /// Adjusts a color to meet a filter.
        /// </summary>
        /// <param name="color">The color to clamp.</param>
        /// <returns>The clamped color to match the filter.</returns>
        public RGBColor Clamp(RGBColor color)
        {
            foreach (var clamp in this)
            {
                color = clamp.Clamp(color);
            }
            return color;
        }
    }
}
