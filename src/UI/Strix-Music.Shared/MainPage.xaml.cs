using Strix_Music.Shared;
using Windows.UI.Xaml.Controls;

namespace Strix_Music
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.OverlayFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.OverlayFrame.Navigate(typeof(SuperShell));
        }
    }
}
