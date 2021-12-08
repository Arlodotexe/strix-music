using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Items
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ArtistViewModel"/> in a list.
    /// </summary>
    [TemplatePart(Name = nameof(_rootGrid), Type = typeof(Grid))]
    public partial class ArtistItem : ItemControl
    {
        private Grid? _rootGrid;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistItem"/> class.
        /// </summary>
        public ArtistItem()
        {
            this.DefaultStyleKey = typeof(ArtistItem);
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// ViewModel holding the data for <see cref="ArtistItem" />
        /// </summary>
        public ArtistViewModel Artist
        {
            get { return (ArtistViewModel)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="ArtistViewModel" />.
        /// </summary>
        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register(nameof(Artist), typeof(ArtistViewModel), typeof(ArtistItem), new PropertyMetadata(0));

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