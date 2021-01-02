using Windows.UI;

namespace LaunchPad.ColorExtraction
{
    public struct ColorFilterConfig
    {
        public static ColorFilterConfig Default =
            new ColorFilterConfig()
            {
                IgnoreGrays = true,
                IgnoreWhites = true,
                IgnoreBlacks = true,
                IgnoreSaturations = true,
                IgnoreValues = true,
                YellowFilter = true,
                GrayTolerance = .15f,
                WhiteTolerance = .20f,
                BlackTolerance = .10f,
                SaturationValueSatTolerance = .3f,
                SaturationValueValTolerance = .3f,
                MinValue = .3f,
                YellowFilterTolerance = .40f
            };

        public ColorFilterConfig(
            bool ignoreGrays = true,
            bool ignoreWhites = true,
            bool ignoreBlacks = true,
            bool ignoreSaturationVals = true,
            bool ignoreValues = true,
            bool yellowFilter = true,
            float grayTolerance = .15f,
            float whiteTolerance = .40f,
            float blackTolerance = .10f,
            float saturationValueSatTolerance = .3f,
            float saturationValueValTolerance = .3f,
            float minValue = .3f,
            float yellowFilterTolerance = .8f)
        {
            IgnoreGrays = ignoreGrays;
            IgnoreWhites = ignoreWhites;
            IgnoreBlacks = ignoreBlacks;
            IgnoreSaturations = ignoreSaturationVals;
            IgnoreValues = ignoreValues;
            YellowFilter = yellowFilter;
            GrayTolerance = grayTolerance;
            WhiteTolerance = whiteTolerance;
            BlackTolerance = blackTolerance;
            SaturationValueSatTolerance = saturationValueSatTolerance;
            SaturationValueValTolerance = saturationValueValTolerance;
            MinValue = minValue;
            YellowFilterTolerance = yellowFilterTolerance;
        }

        private ColorFilterConfig(ColorFilterConfig clone)
        {
            IgnoreGrays = clone.IgnoreGrays;
            IgnoreBlacks = clone.IgnoreBlacks;
            IgnoreSaturations = clone.IgnoreSaturations;
            IgnoreWhites = clone.IgnoreWhites;
            IgnoreValues = clone.IgnoreValues;
            YellowFilter = clone.YellowFilter;
            GrayTolerance = clone.GrayTolerance;
            BlackTolerance = clone.BlackTolerance;
            WhiteTolerance = clone.WhiteTolerance;
            SaturationValueSatTolerance = clone.SaturationValueSatTolerance;
            SaturationValueValTolerance = clone.SaturationValueValTolerance;
            MinValue = clone.MinValue;
            YellowFilterTolerance = clone.YellowFilterTolerance;
        }

        public bool IgnoreGrays { get; set; }

        public bool IgnoreWhites { get; set; }

        public bool IgnoreBlacks { get; set; }

        public bool IgnoreSaturations { get; set; }

        public bool IgnoreValues { get; set; }

        public bool YellowFilter { get; set; }

        public float GrayTolerance { get; set; }

        public float WhiteTolerance { get; set; }

        public float BlackTolerance { get; set; }

        public float SaturationValueSatTolerance { get; set; }

        public float SaturationValueValTolerance { get; set; }

        public float MinValue { get; set; }

        public float YellowFilterTolerance { get; set; }

        /// <summary>
        /// Runs a color through the filter to decide if it should be used.
        /// </summary>
        /// <param name="color">The color in question.</param>
        /// <returns>Whether or not the color fits the filter.</returns>
        public bool TakeColor(Color color)
        {
            if (IgnoreBlacks && color.GetValue() < BlackTolerance)
                return false;

            if (IgnoreWhites && color.GetCMin() > 1 - WhiteTolerance)
                return false;

            float sat = 0;
            if (IgnoreGrays || IgnoreSaturations)
                sat = color.GetSaturation();

            if (IgnoreGrays && sat < GrayTolerance)
                return false;

            if (IgnoreSaturations &&
                sat > 1 - SaturationValueSatTolerance &&
                color.GetValue() > SaturationValueValTolerance)
                return false;

            if (YellowFilter)
            {
                int hue = color.GetHue();
                int hueMax = 60 + 20;
                int hueMin = 60 - 20;
                if (hue < hueMax && hue > hueMin)
                {
                    if (color.GetValue() > YellowFilterTolerance)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Eases the constraints of the filter.
        /// </summary>
        public ColorFilterConfig Ease()
        {
            ColorFilterConfig newConfig = new ColorFilterConfig(this);
            if (newConfig.IgnoreGrays)
            {
                newConfig.IgnoreGrays = false;
                return newConfig;
            }
            if (newConfig.YellowFilter)
            {
                newConfig.YellowFilter = false;
                return newConfig;
            }

            newConfig.WhiteTolerance /= 2;
            newConfig.BlackTolerance /= 2;
            newConfig.SaturationValueSatTolerance /= 2;

            return newConfig;
        }
    }
}
