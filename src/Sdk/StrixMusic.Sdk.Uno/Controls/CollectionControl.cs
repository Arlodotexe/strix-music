using StrixMusic.Sdk.Uno.Helpers;
using System;
using System.Linq;
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
        protected Selector? PART_Selector;

        /// <summary>
        /// Fired when the
        /// </summary>
        public event EventHandler<Events.SelectionChangedEventArgs<TData>>? SelectionChanged;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachHandlers();
        }

        private void AttachHandlers()
        {
            Unloaded += CollectionControl_Unloaded;
            Loaded += CollectionControl_Loaded;

            if (PART_Selector != null)
            {
                PART_Selector.SelectionChanged += SelectedItemChanged;
            }
        }

        private void CollectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Find and set Selector
            PART_Selector = VisualTreeHelpers.GetDataTemplateChild<Selector>(this, nameof(PART_Selector));

            AttachHandlers();
        }

        private void DetachHandlers()
        {
            Loaded -= CollectionControl_Loaded;

            if (PART_Selector != null)
            {
                PART_Selector.SelectionChanged -= SelectedItemChanged;
            }
        }

        private void CollectionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            e.AddedItems.ForEach(x => GetItemFromData(x).Selected = true);
            e.RemovedItems.ForEach(x => GetItemFromData(x).Selected = false);

            /// Get selected item
            // Invoke event
            Events.SelectionChangedEventArgs<TData> selectionChangedEventArgs = new Events.SelectionChangedEventArgs<TData>((PART_Selector!.SelectedItem as TData)!);
            SelectionChanged?.Invoke(this, selectionChangedEventArgs);
        }

        private TItem GetItemFromData(object data)
        {
            return VisualTreeHelpers.FindVisualChildren<TItem>(PART_Selector!.ContainerFromItem(data)).FirstOrDefault()!;
        }
    }
}
