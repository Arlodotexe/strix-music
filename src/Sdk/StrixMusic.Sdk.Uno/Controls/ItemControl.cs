using StrixMusic.Sdk.Uno.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace StrixMusic.Sdk.Uno.Controls
{
    public partial class ItemControl : Control
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Selected"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register(
                nameof(Selected),
                typeof(bool),
                typeof(ArtistItem),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets whether or not the Artist item is selected.
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
            } else
            {
                VisualStateManager.GoToState(this, "Unselected", true);
            }
        }
    }
}
