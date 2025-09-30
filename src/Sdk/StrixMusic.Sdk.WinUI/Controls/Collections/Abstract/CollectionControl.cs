using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Collections.Abstract
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
                new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="ItemClickCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register(
                nameof(ItemClickCommand),
                typeof(FrameworkElement),
                typeof(CollectionControl<TData, TItem>),
                new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsItemClickEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsItemClickEnabledProperty =
            DependencyProperty.Register(
                nameof(IsItemClickEnabled),
                typeof(bool),
                typeof(CollectionControl<TData, TItem>),
                new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="EmptyContentVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty EmptyContentVisibilityProperty =
            DependencyProperty.Register(
                nameof(EmptyContentVisibility),
                typeof(Visibility),
                typeof(CollectionControl<TData, TItem>),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Fired when the selected item changes
        /// </summary>
        public event EventHandler<Events.SelectionChangedEventArgs<TData>>? SelectionChanged;

        /// <summary>
        /// The content to show when this <see cref="TData"/> is empty.
        /// </summary>
        public FrameworkElement EmptyContent
        {
            get => (FrameworkElement)GetValue(EmptyContentProperty);
            set => SetValue(EmptyContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the ItemClick command to invoke when an item is clicked.
        /// </summary>
        public ICommand ItemClickCommand
        {
            get => (ICommand)GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating if ItemClick should be enabled.
        /// </summary>
        public bool IsItemClickEnabled
        {
            get => (bool)GetValue(IsItemClickEnabledProperty);
            set => SetValue(IsItemClickEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="EmptyContent"/> should be shown.
        /// </summary>
        public Visibility EmptyContentVisibility
        {
            get => (Visibility)GetValue(EmptyContentVisibilityProperty);
            set => SetValue(EmptyContentVisibilityProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += CollectionControl_Loaded;
        }

        private Control? PART_Selector { get; set; }

        private ScrollViewer? PART_Scroller { get; set; }

        /// <summary>
        /// Perform incremental loading.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task LoadMore();

        /// <summary>
        /// Checks if the collection has no content.
        /// </summary>
        protected abstract void CheckAndToggleEmpty();

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

            switch (PART_Selector)
            {
                case Selector selector:
                    selector.SelectionChanged -= SelectedItemChanged;
                    break;
                case DataGrid dataGrid:
                    dataGrid.SelectionChanged -= SelectedItemChanged;
                    break;
                default:
                    return;
            }

            PART_Scroller.ViewChanged -= CollectionControl_ViewChanged;
        }

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

            _ = LoadMore();
        }

        private void CollectionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void CollectionControl_ViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
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
            foreach (var item in e.AddedItems)
            {
                TItem? container = GetItemFromData(item);
                if (container != null)
                {
                    container.Selected = true;
                }
            }

            foreach (var item in e.RemovedItems)
            {
                TItem? container = GetItemFromData(item);
                if (container != null)
                {
                    container.Selected = false;
                }
            }

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
    }
}
