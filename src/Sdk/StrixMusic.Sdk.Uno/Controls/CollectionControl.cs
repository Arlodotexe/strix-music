using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Uno.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Sdk.Uno.Controls
{
    [TemplatePart(Name = nameof(PART_Selector), Type = typeof(Selector))]
    public abstract partial class CollectionControl<TData, TItem> : Control
        where TData : class
        where TItem : ItemControl
    {
        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="EmptyContent"/> property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentProperty =
            DependencyProperty.Register(
                nameof(EmptyContent),
                typeof(FrameworkElement),
                typeof(CollectionControl<TData, TItem>),
                new PropertyMetadata(null, (d, e) => ((CollectionControl<TData, TItem>)d).SetNoContentTemplate((FrameworkElement)e.NewValue)));

        /// <summary>
        /// Fired when the selected item changes
        /// </summary>
        public event EventHandler<Events.SelectionChangedEventArgs<TData>>? SelectionChanged;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += CollectionControl_Loaded;
        }

        /// <summary>
        /// The content to show when this <see cref="TData"/> is empty.
        /// </summary>
        public FrameworkElement EmptyContent
        {
            get => (FrameworkElement)GetValue(EmptyContentProperty);
            set => SetValue(EmptyContentProperty, value);
        }

        private Selector? PART_Selector { get; set; }

        private ScrollViewer? PART_Scroller { get; set; }

        private ContentPresenter? PART_EmptyContentPresenter { get; set; }

        /// <summary>
        /// Perform incremental loading.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task LoadMore();

        /// <summary>
        /// Checks if the collection has no content.
        /// </summary>
        protected abstract void CheckAndToggleEmpty();

        /// <summary>
        /// Sets the visibility of the empty content.
        /// </summary>
        protected void SetEmptyVisibility(Visibility visibility)
        {
            Guard.IsNotNull(PART_EmptyContentPresenter, nameof(PART_EmptyContentPresenter));

            PART_EmptyContentPresenter.Visibility = visibility;
        }

        private void AttachHandlers()
        {
            Unloaded += CollectionControl_Unloaded;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(PART_Scroller, nameof(PART_Scroller));
            PART_Selector.SelectionChanged += SelectedItemChanged;
            PART_Scroller.ViewChanged += CollectionControl_ViewChanged;
        }

        private void DetachHandlers()
        {
            Loaded -= CollectionControl_Loaded;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(PART_Selector, nameof(PART_Scroller));
            PART_Selector!.SelectionChanged -= SelectedItemChanged;
            PART_Scroller!.ViewChanged -= CollectionControl_ViewChanged;
        }

        private void CollectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Find and set Selector
            PART_Selector = VisualTreeHelpers.GetDataTemplateChild<Selector>(this, nameof(PART_Selector));
            PART_Scroller = VisualTreeHelpers.GetDataTemplateChild<ScrollViewer>(this);

            AttachHandlers();

            CheckScrollPosition();
        }

        private void CollectionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void CollectionControl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            CheckScrollPosition();
        }

        private void CheckScrollPosition()
        {
            Guard.IsNotNull(PART_Scroller, nameof(PART_Scroller));

            double fromBottom = PART_Scroller.ScrollableHeight - PART_Scroller.VerticalOffset;

            // If approaching the bottom of the list
            if (fromBottom < 100)
            {
                _ = LoadMore();
            }
        }

        private void SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            e.AddedItems.ForEach(x =>
            {
                TItem container = GetItemFromData(x);
                if (container != null)
                {
                    container.Selected = true;
                }
            });

            e.RemovedItems.ForEach(x =>
            {
                TItem container = GetItemFromData(x);
                if (container != null)
                {
                    container.Selected = false;
                }
            });

            // Get selected item
            // Invoke event
            Events.SelectionChangedEventArgs<TData> selectionChangedEventArgs = new Events.SelectionChangedEventArgs<TData>((PART_Selector!.SelectedItem as TData)!);
            SelectionChanged?.Invoke(this, selectionChangedEventArgs);
        }

        private TItem GetItemFromData(object data)
        {
            return VisualTreeHelpers.FindVisualChildren<TItem>(PART_Selector!.ContainerFromItem(data)).FirstOrDefault()!;
        }

        private void SetNoContentTemplate(FrameworkElement frameworkElement)
        {
            if (PART_EmptyContentPresenter != null)
            {
                PART_EmptyContentPresenter.Content = frameworkElement;
            }
        }
    }
}