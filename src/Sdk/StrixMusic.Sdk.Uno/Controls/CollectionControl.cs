using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Uno.Extensions.Specialized;
using StrixMusic.Sdk.Uno.Helpers;
using System.Linq;

namespace StrixMusic.Sdk.Uno.Controls
{
    [TemplatePart(Name = nameof(PART_Selector), Type = typeof(Selector))]
    public partial class CollectionControl : Control
    {
        protected Selector? PART_Selector;

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
        }

        private ItemControl GetItemFromData(object data)
        {
            return VisualTreeHelpers.FindVisualChildren<ItemControl>(PART_Selector!.ContainerFromItem(data)).FirstOrDefault()!;
        }
    }
}
