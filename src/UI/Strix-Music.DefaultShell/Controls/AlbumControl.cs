using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.DefaultShell.Controls
{
    [TemplatePart(Name = nameof(_rootGrid), Type = typeof(Grid))]
    public sealed partial class AlbumControl : Control
    {
        private Grid? _rootGrid;

        public AlbumControl()
        {
            this.DefaultStyleKey = typeof(AlbumControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Find and set RootGrid
            _rootGrid = GetTemplateChild(nameof(_rootGrid)) as Grid;

            if (_rootGrid != null)
            {
                _rootGrid.PointerEntered += RootGrid_PointerEntered;
                _rootGrid.PointerExited += RootGrid_PointerExited;
            }
        }

        private void RootGrid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void RootGrid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }
    }
}