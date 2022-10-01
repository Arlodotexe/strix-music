using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Items
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
        public IArtist Artist
        {
            get { return (IArtist)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        /// <summary>
        /// The artist to display.
        /// </summary>
        public ArtistViewModel ArtistVm => (ArtistViewModel)GetValue(ArtistViewModelProperty);

        /// <summary>
        /// Dependency property for <see cref="Artist"/>.
        /// </summary>
        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register(nameof(Artist), typeof(IArtist), typeof(ArtistItem), new PropertyMetadata(null, (d, e) => ((ArtistItem)d).OnArtistChanged(e.OldValue as IArtist, e.NewValue as IArtist)));

        /// <summary>
        /// Dependency property for <see cref="ArtistVm"/>.
        /// </summary>
        public static readonly DependencyProperty ArtistViewModelProperty =
            DependencyProperty.Register(nameof(Artist), typeof(ArtistViewModel), typeof(ArtistItem), new PropertyMetadata(null));

        /// <summary>
        /// Fires when the <see cref="Artist"/> is changed.
        /// </summary>
        protected virtual void OnArtistChanged(IArtist? oldValue, IArtist? newValue)
        {
            if (newValue is not null)
                SetValue(ArtistViewModelProperty, Artist is ArtistViewModel artistVm ? artistVm : new ArtistViewModel(newValue, newValue.Root));
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
