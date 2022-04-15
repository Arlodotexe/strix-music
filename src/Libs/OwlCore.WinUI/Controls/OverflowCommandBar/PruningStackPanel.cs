using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OwlCore.WinUI.Controls
{
#if NETFX_CORE
    public partial class PruningStackPanel : Panel
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> for the <see cref="CheckedContent"/> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(PruningStackPanel),
                new PropertyMetadata(Orientation.Horizontal, LayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the <see cref="Orientation"/> of the <see cref="PruningStackPanel"/>.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public int HiddenItemCount { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            double usedSpace = 0;
            double maxSpace = 0;

            foreach (var child in Children)
            {
                child.Measure(availableSize);
                Size childDesiredSize = child.DesiredSize;
                double newUsedSpace = usedSpace + (Orientation == Orientation.Horizontal ? childDesiredSize.Width : childDesiredSize.Height);
                double newMaxSpace = (Orientation == Orientation.Horizontal ? childDesiredSize.Height : childDesiredSize.Width);
                if (newMaxSpace > maxSpace)
                {
                    maxSpace = Math.Min(newMaxSpace, (Orientation == Orientation.Horizontal ? availableSize.Height : availableSize.Width));
                }

                if (newUsedSpace > (Orientation == Orientation.Horizontal ? availableSize.Width : availableSize.Height))
                {
                    break;
                }
            }

            return new Size()
            {
                Height = Orientation == Orientation.Horizontal ? maxSpace : usedSpace,
                Width = Orientation == Orientation.Horizontal ? usedSpace : maxSpace,
            };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
            {
                return new Size(0, 0);
            }

            // The used space in which-ever dimension the orientation is.
            double usedSpace = 0;
            for (var i = 0; i < Children.Count; i++)
            {
                // Determine room for new item
                double remainingHeight = finalSize.Height;
                double remainingWidth = finalSize.Width;
                if (Orientation == Orientation.Horizontal)
                {
                    remainingWidth -= usedSpace;
                } else
                {
                    remainingHeight -= usedSpace;
                }

                Size lastUsedSize = ArrangeChild(Children[i], new Size(remainingWidth, remainingHeight), usedSpace);
                
                // If the last item used none of the allocated space.
                if ((Orientation == Orientation.Horizontal ? lastUsedSize.Width : lastUsedSize.Height) == 0)
                {
                    HiddenItemCount = Children.Count - i + 1;
                }

                usedSpace += Orientation == Orientation.Horizontal ? lastUsedSize.Width : lastUsedSize.Height;
            }

            return finalSize;
        }

        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PruningStackPanel psp)
            {
                psp.InvalidateMeasure();
                psp.InvalidateArrange();
            }
        }

        private Size ArrangeChild(UIElement child, Size remainingSize, double constrainedOffset)
        {
            // The desired space of the child over the constrained orientation.
            double desiredSpace = Orientation == Orientation.Horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;

            if (desiredSpace > (Orientation == Orientation.Horizontal ? remainingSize.Width : remainingSize.Height))
            {
                // The child wants more space than available.
                return new Size(0, 0);
            } else
            {
                // The child can be fit in this space.
                Size childSize = new Size()
                {
                    Height = Orientation == Orientation.Horizontal ? remainingSize.Height : desiredSpace,
                    Width = Orientation == Orientation.Horizontal ? desiredSpace : remainingSize.Width,
                };
                Rect renderRect = new Rect()
                {
                    X = Orientation == Orientation.Horizontal ? constrainedOffset : 0,
                    Y = Orientation == Orientation.Horizontal ? 0 : constrainedOffset,
                    Height = childSize.Height,
                    Width = childSize.Width,
                };
                child.Arrange(renderRect);
                return childSize;
            }
        }
    }
#endif
}
