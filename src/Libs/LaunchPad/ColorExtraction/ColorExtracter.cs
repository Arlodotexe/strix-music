/// http://www.codeding.com/articles/k-means-algorithm

using System.Collections.Generic;
using System.Linq;

namespace LaunchPad.ColorExtraction
{
    /// <summary>
    /// A <see langword="static"/> class containing a method to palette colors out of a set.
    /// </summary>
    public static class ColorExtracter
    {
        /// <summary>
        /// Gets a list of palette colors from a list of seen colors.
        /// </summary>
        /// <param name="ogColors">The original color list.</param>
        /// <param name="paletteSize">The amount of colors to extract.</param>
        /// <param name="raw">Whether or not to add extra clusters for more precise colors.</param>
        /// <returns>A color palette.</returns>
        public static List<HSVColor> Palettize(List<HSVColor> ogColors, int paletteSize, bool raw = false)
        {
            List<PaletteItem> results = KMeans(ogColors, raw ? paletteSize : paletteSize + 2);
            return results.Take(paletteSize).Select(x => x.GetColorNearestToCenter()).ToList();
        }

        /// <summary>
        /// Runs a KMeans cluster on a list of <see cref="HSVColor"/>s to determine a color palette.
        /// </summary>
        /// <param name="colors">The list of original colors.</param>
        /// <param name="clusterCount">The amount of colors in the color palette.</param>
        /// <returns></returns>
        internal static List<PaletteItem> KMeans(List<HSVColor> colors, int clusterCount)
        {
            // Create list of clusters
            List<PaletteItem> clusters = new List<PaletteItem>();

            // Split colors into arbitrary clusters
            List<List<HSVColor>> initialClusters = colors.Split(clusterCount);
            foreach(List<HSVColor> cluster in initialClusters)
            {
                PaletteItem item = new PaletteItem();
                item.AddRange(cluster);
                clusters.Add(item);
            }

            int movements = 1;
            while (movements > 0)
            {
                movements = 0;

                foreach (PaletteItem item in clusters) 
                {
                    for (int pointIndex = 0; pointIndex < item.Count; pointIndex++)
                    {
                        HSVColor point = item[pointIndex];

                        int nearestCluster = FindNearestCluster(clusters, point);
                        if (nearestCluster != clusters.IndexOf(item)) 
                        {
                            if (item.Count > 1)
                            {
                                HSVColor removedPoint = item.RemovePoint(point);
                                clusters[nearestCluster].AddPoint(removedPoint);
                                movements += 1;
                            }
                        }
                    }
                }
            }

            return clusters;
        }

        private static int FindNearestCluster(List<PaletteItem> clusters, HSVColor point)
        {
            double minimumDistance = 0.0;
            int nearestClusterIndex = -1;

            for (int k = 0; k < clusters.Count; k++) //find nearest cluster
            {
                double distance = HSVColor.FindDistance(point, clusters[k].Center);
                if (k == 0)
                {
                    minimumDistance = distance;
                    nearestClusterIndex = 0;
                }
                else if (minimumDistance > distance)
                {
                    minimumDistance = distance;
                    nearestClusterIndex = k;
                }
            }

            return (nearestClusterIndex);
        }
    }
}
