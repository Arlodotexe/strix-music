using System;
using System.Collections.Generic;
using System.Text;
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
                GrayTolerance = .15f,
                WhiteTolerance = .40f,
                BlackTolerance = .10f,
                SaturationTolerance = .3f,
                MinValue = .3f
            };

        public ColorFilterConfig(
            bool ignoreGrays = true,
            bool ignoreWhites = true,
            bool ignoreBlacks = true,
            bool ignoreSaturations = true,
            bool ignoreValues = true,
            float grayTolerance = .15f,
            float whiteTolerance = .40f,
            float blackTolerance = .10f,
            float saturationTolerance = .3f,
            float minValue = .3f)
        {
            IgnoreGrays = ignoreGrays;
            IgnoreWhites = ignoreWhites;
            IgnoreBlacks = ignoreBlacks;
            IgnoreSaturations = ignoreSaturations;
            IgnoreValues = ignoreValues;
            GrayTolerance = grayTolerance;
            WhiteTolerance = whiteTolerance;
            BlackTolerance = blackTolerance;
            SaturationTolerance = saturationTolerance;
            MinValue = minValue;
        }

        public bool IgnoreGrays { get; set; }

        public bool IgnoreWhites { get; set; }

        public bool IgnoreBlacks{ get; set; }

        public bool IgnoreSaturations{ get; set; }

        public bool IgnoreValues{ get; set; }

        public float GrayTolerance { get; set; }

        public float WhiteTolerance { get; set; }

        public float BlackTolerance { get; set; }

        public float SaturationTolerance { get; set; }

        public float MinValue { get; set; }

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

            if (IgnoreSaturations && sat > 1 - SaturationTolerance)
                return false;

            return true;
        }

        /// <summary>
        /// Eases the constraints of the filter.
        /// </summary>
        public ColorFilterConfig Ease()
        {
            if (IgnoreGrays)
            {
                IgnoreGrays = false;
                return this;
            }

            WhiteTolerance /= 2;
            BlackTolerance /= 2;

            return this;
        }
    }
}
