using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Quickplay
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuickplayContent : UserControl
    {
        private bool _isSecondaryActive = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickplayContent"/> class.
        /// </summary>
        public QuickplayContent()
        {
            this.InitializeComponent();

            Loaded += QuickplayContent_Loaded;
        }

        /// <summary>
        /// The root <see cref="StrixDataRootViewModel" /> used by the shell.
        /// </summary>
        public StrixDataRootViewModel DataRoot
        {
            get => (StrixDataRootViewModel)GetValue(DataRootProperty);
            set => SetValue(DataRootProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="DataRoot"/>.
        /// </summary>
        public static readonly DependencyProperty DataRootProperty =
            DependencyProperty.Register(nameof(DataRoot), typeof(StrixDataRootViewModel), typeof(QuickplayContent), new PropertyMetadata(null));

        private void QuickplayContent_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataRoot is null)
                return;

            var library = (LibraryViewModel)DataRoot.Library;

            if (library.InitAlbumCollectionAsyncCommand.CanExecute(null))
                library.InitAlbumCollectionAsyncCommand.Execute(null);
        }

        /// <summary>
        /// Runs any animations for when the <see cref="QuickplayContent"/> enters view.
        /// </summary>
        public void RunEnterViewAnimation()
        {
            LoadInView.Begin();
        }

        private void Rectangle_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (_isSecondaryActive)
            {
                VisualStateManager.GoToState(RootControl, "Main", true);
                VisualStateManager.GoToState(RootControl, "SecondaryHover", true);
            }
            else
            {
                VisualStateManager.GoToState(RootControl, "Secondary", true);
                VisualStateManager.GoToState(RootControl, "MainHover", true);
            }

            _isSecondaryActive = !_isSecondaryActive;
        }

        private void Rectangle_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_isSecondaryActive)
            {
                VisualStateManager.GoToState(RootControl, "SecondaryHover", true);
            }
            else
            {
                VisualStateManager.GoToState(RootControl, "MainHover", true);
            }
        }

        private void Rectangle_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_isSecondaryActive)
            {
                VisualStateManager.GoToState(RootControl, "SecondaryNoHover", true);
            }
            else
            {
                VisualStateManager.GoToState(RootControl, "MainNoHover", true);
            }
        }

        private void MainWrapper_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            double center = MainScroller.ScrollableWidth / 2;
            MainScroller.ScrollToHorizontalOffset(center);
        }
    }
}
