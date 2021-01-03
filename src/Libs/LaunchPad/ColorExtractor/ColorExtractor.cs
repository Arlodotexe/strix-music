/// http://www.codeding.com/articles/k-means-algorithm

using ColorExtractor;
using ColorExtractor.ColorExtractor.Filters;
using System.Collections.Generic;
using System.Linq;

namespace LaunchPad.ColorExtraction
{
    /// <summary>
    /// A <see langword="static"/> class containing a method to palette colors out of a set.
    /// </summary>
    public static class ColorExtractor
    {
        private const int MAX_MOVES = 65_536;

        /// <summary>
        /// Gets a list of palette colors from a list of seen colors.
        /// </summary>
        /// <param name="ogColors">The original color list.</param>
        /// <param name="paletteSize">The amount of colors to extract.</param>
        /// <param name="bufferPalettes">How many extra clusters to add for more precise colors.</param>
        /// <returns>A color palette.</returns>
        public static List<HSVColor> Palettize(IEnumerable<HSVColor> ogColors,
            int paletteSize,
            IList<IFilter>? filters = null,
            IList<IFilter>? clamps = null,
            int bufferPalettes = 5)
        {
            int totalClusters = paletteSize + bufferPalettes;

            if (filters != null)
            {
                foreach(var filter in filters)
                {
                    var filerted = ogColors.Where(x => filter.TakeColor(x));
                    if (filerted.Count() >= totalClusters)
                    {
                        ogColors = filerted;
                    } else
                    {
                        break;
                    }
                }
            }

            int n = ogColors.Count() / 5_000;
            n = n == 0 ? 1 : n;
            ogColors = ogColors.Where((x, i) => i % n == 0);

            List<PaletteItem> paletteResults = KMeans(ogColors.ToList(), totalClusters);
            IEnumerable<HSVColor> results = paletteResults
                .OrderByDescending(x => x.Count)
                .Take(paletteSize)
                .Select(x => x.GetColorNearestToCenter());

            if (clamps != null)
            {
                foreach (var clamp in clamps)
                {
                    results = results.Select(x => clamp.Clamp(x));
                }
            }

            return results.ToList();
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

            int totalMoves = 0;

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
                                totalMoves += 1;

                                if (totalMoves > MAX_MOVES)
                                    goto end;
                            }
                        }
                    }
                }
            }

            end: // Escape kMeans

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
