using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Strix.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="Sdk.Uno.Controls.ArtistCollection"/> in the Strix Shell.
    /// </summary>
    public sealed partial class ArtistCollectionStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistCollectionStyle"/> class.
        /// </summary>
        public ArtistCollectionStyle()
        {
            this.InitializeComponent();
        }

        private void OpenArtist(object sender, ItemClickEventArgs e)
        {
        }
    }
}
