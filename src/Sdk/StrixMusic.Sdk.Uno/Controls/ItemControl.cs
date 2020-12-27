using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// Represents a container item in the <see cref="CollectionControl{TData, TItem}"/>.
    /// </summary>
    public partial class ItemControl : Control
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Selected"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register(
                nameof(Selected),
                typeof(bool),
                typeof(ItemControl),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets whether or not the Item is selected.
        /// </summary>
        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set
            {
                SetValue(SelectedProperty, value);
                UpdateVisualState(value);
            }
        }

        private void UpdateVisualState(bool selected)
        {
            if (selected)
            {
                VisualStateManager.GoToState(this, "Selected", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselected", true);
            }
        }
    }
}
