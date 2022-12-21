using StrixMusic.Sdk.WinUI.Controls.Views.Secondary;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.Strix.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="AlbumView"/> in the Strix Shell.
    /// </summary>
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
        }
    }
}
