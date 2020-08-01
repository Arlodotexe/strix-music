using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace Strix_Music.DefaultShell.Controls
{
    [TemplatePart(Name = nameof(rootGrid), Type = typeof(Grid))]
    public sealed partial class AlbumControl : Control
    {
        private Grid rootGrid;

        public AlbumControl()
        {
            this.DefaultStyleKey = typeof(AlbumControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Find and set RootGrid
            rootGrid = GetTemplateChild(nameof(rootGrid)) as Grid;

            rootGrid.PointerEntered += RootGrid_PointerEntered;
            rootGrid.PointerExited += RootGrid_PointerExited;
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

#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning restore CS8601 // Possible null reference assignment.