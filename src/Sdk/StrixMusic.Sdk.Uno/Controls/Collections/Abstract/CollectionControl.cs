using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Uwp.UI.Controls;
using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.Uno.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Sdk.Uno.Controls.Collections.Abstract
{
    /// <summary>
    /// A Templated Control base for showing items with progressive loading.
    /// </summary>
    /// <typeparam name="TData">The data type.</typeparam>
    /// <typeparam name="TItem">The container type.</typeparam>
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

        private Control? PART_Selector { get; set; }

        private ScrollViewer? PART_Scroller { get; set; }

        private ContentPresenter? PART_EmptyContentPresenter { get; set; }

        /// <summary>
        /// Clears all selected items.
        /// </summary>
        public void ClearSelected()
        {
            if (PART_Selector == null)
                return;

            if (PART_Selector is Selector selector)
                selector.SelectedItem = null;
            else if (PART_Selector is DataGrid dataGrid)
                dataGrid.SelectedItem = null;
        }

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

            if (PART_Selector == null || PART_Scroller == null)
                return;

            if (PART_Selector is Selector selector)
                selector.SelectionChanged += SelectedItemChanged;
            else if (PART_Selector is DataGrid dataGrid)
                dataGrid.SelectionChanged += SelectedItemChanged;
            else return;

            PART_Scroller.ViewChanged += CollectionControl_ViewChanged;
        }

        private void DetachHandlers()
        {
            Loaded -= CollectionControl_Loaded;

            if (PART_Selector == null || PART_Scroller == null)
                return;

            if (PART_Selector is Selector selector)
                selector.SelectionChanged -= SelectedItemChanged;
            else if (PART_Selector is DataGrid dataGrid)
                dataGrid.SelectionChanged -= SelectedItemChanged;
            else return;

            PART_Scroller!.ViewChanged -= CollectionControl_ViewChanged;
        }

        private void CollectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Find and set Selector
            PART_Selector = VisualTreeHelpers.GetDataTemplateChild<Selector>(this, nameof(PART_Selector));
            if (PART_Selector == null)
            {
                PART_Selector = VisualTreeHelpers.GetDataTemplateChild<DataGrid>(this, nameof(PART_Selector));
            }

            // Find and set Scroller
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
            if (PART_Scroller == null)
                return;

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
                TItem? container = GetItemFromData(x);
                if (container != null)
                {
                    container.Selected = true;
                }
            });

            e.RemovedItems.ForEach(x =>
            {
                TItem? container = GetItemFromData(x);
                if (container != null)
                {
                    container.Selected = false;
                }
            });

            if (PART_Selector == null)
                return;

            // Get selected item
            // Invoke event
            TData data;
            if (PART_Selector is Selector selector)
                data = (TData)selector.SelectedItem;
            else if (PART_Selector is DataGrid dataGrid)
                data = (TData)dataGrid.SelectedItem;
            else
                return;

            Events.SelectionChangedEventArgs<TData> selectionChangedEventArgs = new Events.SelectionChangedEventArgs<TData>(data);
            SelectionChanged?.Invoke(this, selectionChangedEventArgs);
        }

        private TItem? GetItemFromData(object data)
        {
            if (PART_Selector is Selector selector)
                return VisualTreeHelpers.FindVisualChildren<TItem>(selector.ContainerFromItem(data)).FirstOrDefault();
            return null;
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
