using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls
{
    /// <summary>
    /// A panel that arranges objects in a very **very** visualy striking way.
    /// </summary>
    public partial class QuickplayPanel
        : Panel
    {
        /// <summary>
        /// An array of potential item sizes.
        /// </summary>
        private double[] _itemHeights = { 5.2, 4, 2, 1 };

        /// <summary>
        /// Gets or sets the size of the smallest tile and basis for all tiles being rendered.
        /// </summary>
        public double BaseTileHeight { get; set; } = 48;

        /// <summary>
        /// Gets or sets the direction the elements move.
        /// </summary>
        /// <remarks>
        /// Will be LeftToRight if flow direction is RightToLeft.
        /// </remarks>
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets the size of the margin between the items.
        /// </summary>
        public double ItemMargin { get; set; } = 4;

        /// <summary>
        /// The seed to use for randomizing the layout.
        /// </summary>
        public int Seed { get; set; } = 1;

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // TODO: Allow inverted direction
            // TODO: Remove tailing small tiles
            Random rand = new Random(/*Seed*/);

            // The offset of the max index lookup for a size this iteration.
            int maxSizeIndex = 2;

            // The min index lookup for a size this iteration.
            int minSizeIndex = 0;

            // Gets the height of the next item
            double NextHeight()
            {
                if (minSizeIndex >= _itemHeights.Length - maxSizeIndex)
                {
                    if (maxSizeIndex != 0)
                    {
                        maxSizeIndex = 0;
                    }
                    else
                    {
                        return double.PositiveInfinity;
                    }
                }

                int sizeIndex = rand.Next(minSizeIndex, _itemHeights.Length - maxSizeIndex);

                if (minSizeIndex == sizeIndex)
                {
                    // Only allow two of the same item sizes in a row (by column)
                    minSizeIndex++;
                }
                else
                {
                    // Larger items cannot go below smaller items
                    minSizeIndex = sizeIndex;
                }

                // Decrease the max offest each iteration by column till 0
                if (maxSizeIndex != 0)
                {
                    maxSizeIndex--;
                }

                return GetRealHeight(_itemHeights[sizeIndex]);
            }

            // Track the current top left most point and furthest right point
            Point point = new Point(0, 0);
            double maxX = 0;

            // Layout each child
            foreach (var child in Children)
            {
                // Find the child's ratio
                child.Measure(finalSize);
                Size desiredSize = child.DesiredSize;
                double widthRatio = 1;
                if (desiredSize.Height + desiredSize.Height != 0)
                {
                    widthRatio = desiredSize.Width / desiredSize.Height;
                }

            // This marks the beginning of an attempted tile making. If a tile does not fit in a column, it will adjust the status and return here.
            attemptLayout:

                // Set the child's height while respecting the ratio
                Size actualSize = Size.Empty;
                actualSize.Height = NextHeight();
                actualSize.Width = actualSize.Height * widthRatio;

                // Ensure the item fits in the column.
                if (actualSize.Height == double.PositiveInfinity || point.Y + actualSize.Height > finalSize.Height)
                {
                    // The item would not fit, iterate columns
                    maxX += ItemMargin;
                    point.X = maxX;
                    point.Y = 0;
                    minSizeIndex = 1;
                    maxSizeIndex = 1;

                    // With the new state, attempt to insert a tile
                    goto attemptLayout;
                }

                // Finally arrange child
                Rect rect = new Rect(point, actualSize);
                child.Arrange(rect);

                // Update position and maxX
                maxX = Math.Max(rect.Right, maxX);
                point.Y += actualSize.Height;
                point.Y += ItemMargin;
            }

            return new Size(point.X, point.Y);
        }

        /// <summary>
        /// Converts factors of heights to real heights, accounting for factor and margins.
        /// </summary>
        private double GetRealHeight(double factor)
        {
            return factor * BaseTileHeight + ((factor - 1) * ItemMargin);
        }
    }
}
