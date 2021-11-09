using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Quickplay
{
    /// <summary>
    /// A panel that arranges objects in a very **very** visualy striking way.
    /// </summary>
    public partial class QuickplayPanel
        : Panel
    {
        private struct ArrangeConfig
        {
            /// <summary>
            /// An array of potential item sizes.
            /// </summary>
            private static readonly double[] _itemHeights = { 1, 2, 4, 5.2 };

            private Random _rand;
            private int _minSizeIndex;
            private int _maxSizeIndex;

            private int _newColumnMinSizeIndex;
            private int _newColumnMaxSizeIndex;

            public ArrangeConfig(
                int seed,
                int initialMaxSizeIndex = 1,
                int initialMinSizeIndex = 1,
                int newColumnMaxSizeIndex = 1,
                int newColumnMinSizeIndex = 1)
            {
                if (seed != -1)
                    _rand = new Random(seed);
                else
                    _rand = new Random();

                _maxSizeIndex = initialMaxSizeIndex;
                _minSizeIndex = initialMinSizeIndex;

                _newColumnMinSizeIndex = newColumnMinSizeIndex;
                _newColumnMaxSizeIndex = newColumnMaxSizeIndex;
            }

            public double GetNextRelativeHeight()
            {
                /*if (_itemHeights.Length - _maxSizeIndex <= _minSizeIndex)
                {
                    if (_minSizeIndex != 0)
                    {
                        _minSizeIndex = 0;
                    }
                    else
                    {
                        return double.PositiveInfinity;
                    }
                }*/

                int sizeIndex = _rand.Next(_minSizeIndex, _itemHeights.Length - _maxSizeIndex);

/*                if (_maxSizeIndex == sizeIndex)
                {
                    // Only allow two of the same item sizes in a row (by column)
                    _maxSizeIndex++;
                }
                else
                {
                    // Larger items cannot go below smaller items
                    _maxSizeIndex = _itemHeights.Length - sizeIndex;
                }

                // Decrease the max offest each iteration by column till 0
                if (_minSizeIndex > 0)
                {
                    _minSizeIndex--;
                }*/

                return _itemHeights[sizeIndex];
            }

            public void IncerementColumn()
            {
                _minSizeIndex = _newColumnMinSizeIndex;
                _maxSizeIndex = _newColumnMaxSizeIndex;
            }
        }

        private struct ArrangeState
        {
            private Point _point;
            private double _maxX;
            private double _maxY;
            private double _itemMargin;
            private double _finalHeight;

            public ArrangeState(double finalHeight, double itemMargin)
            {
                _finalHeight = finalHeight;
                _itemMargin = itemMargin;
                _point = new Point(0, 0);
                _maxX = 0;
                _maxY = 0;
            }

            public bool TryAddChild(UIElement child, Size childSize, double relativeSize)
            {
                if (_point.Y + childSize.Height > _finalHeight)
                {
                    // Item won't fit
                    return false;
                }

                // Arrange child
                Rect rect = new Rect(_point, childSize);

                if (_maxY < rect.Bottom && relativeSize < 2)
                {
                    // Removes tailing small tiles, because they are un-appealing
                    return false;
                }

                child.Arrange(rect);

                // Update position and maxX
                _maxX = Math.Max(rect.Right, _maxX);
                _maxY = Math.Max(rect.Bottom, _maxY);
                _point.Y += childSize.Height;
                _point.Y += _itemMargin;

                return true;
            }

            public void IncerementColumn()
            {
                _maxX += _itemMargin;
                _point.X = _maxX;
                _point.Y = 0;
            }

            public Size FinalSize => new Size(_point.X, _point.Y);
        }

        /// <summary>
        /// Gets or sets the base size of a tile.
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
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }

            return ArrangeTiles(availableSize);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeTiles(finalSize);
        }

        private Size ArrangeTiles(Size finalSize)
        {
            // TODO: Allow inverted direction
            ArrangeConfig arrangeConfig = new ArrangeConfig(Seed);
            ArrangeState arrangeState = new ArrangeState(finalSize.Height, ItemMargin);

            // Layout each child
            foreach (var child in Children)
            {
                // Find the child's ratio
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
                double relativeSize = arrangeConfig.GetNextRelativeHeight();
                actualSize.Height = GetRealHeight(relativeSize);
                actualSize.Width = actualSize.Height * widthRatio;

                if (double.IsNegativeInfinity(actualSize.Height))
                {
                    // No more sizes were valid for this column.
                    arrangeState.IncerementColumn();
                    arrangeConfig.IncerementColumn();
                    goto attemptLayout;
                }

                if (!arrangeState.TryAddChild(child, actualSize, relativeSize))
                {
                    // The item would not fit in this column, iterate columns
                    arrangeState.IncerementColumn();
                    arrangeConfig.IncerementColumn();
                    goto attemptLayout;
                }
            }

            return arrangeState.FinalSize;
        }

        /// <summary>
        /// Converts factors of heights to real heights, accounting for factor and margins.
        /// </summary>
        private double GetRealHeight(double factor)
        {
            return (factor * BaseTileHeight) + ((factor - 1) * ItemMargin);
        }
    }
}
