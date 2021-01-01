/// http://www.codeding.com/articles/k-means-algorithm

using System.Collections.Generic;
using System.Linq;
using Windows.UI;

namespace LaunchPad.ColorExtraction
{
    internal class PaletteItem : List<HSVColor>
    {
        /// <summary>
        /// The Center of the Color cluster.
        /// </summary>
        public HSVColor Center { get; set; }

        public PaletteItem()
            : base()
        {
            Center = HSVColor.FromColor(Colors.Transparent);
        }

        internal void AddPoint(HSVColor p)
        {
            this.Add(p);
            UpdateCentroid();
        }

        internal HSVColor RemovePoint(HSVColor p)
        {
            HSVColor removedPoint = HSVColor.Clone(p);
            this.Remove(p);
            UpdateCentroid();

            return (removedPoint);
        }

        internal HSVColor GetColorNearestToCenter()
        {
            double minimumDistance = 0.0;
            int nearestPointIndex = -1;

            foreach (HSVColor p in this)
            {
                double distance = HSVColor.FindDistance(p, Center);

                if (this.IndexOf(p) == 0)
                {
                    minimumDistance = distance;
                    nearestPointIndex = this.IndexOf(p);
                }
                else
                {
                    if (minimumDistance > distance)
                    {
                        minimumDistance = distance;
                        nearestPointIndex = this.IndexOf(p);
                    }
                }
            }

            return (this[nearestPointIndex]);
        }

        private void UpdateCentroid()
        {
            byte hAvg = (byte)this.Average(x => x.H);
            byte sAvg = (byte)this.Average(x => x.S);
            byte vAvg = (byte)this.Average(x => x.V);
            Center = HSVColor.FromAhsv(255, hAvg, sAvg, vAvg);
        }
    }
}
