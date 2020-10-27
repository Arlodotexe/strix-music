using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Strix.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="AlbumCollection"/> in the Strix Shell.
    /// </summary>
    public sealed partial class AlbumCollectionStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumCollectionStyle"/> class.
        /// </summary>
        public AlbumCollectionStyle()
        {
            this.InitializeComponent();
        }

        private void OpenAlbum(object sender, ItemClickEventArgs e)
        {
            INavigationService<Control> navigationService = StrixShellIoc.Ioc.GetService<INavigationService<Control>>();
            navigationService.NavigateTo(typeof(AlbumView), false, e.ClickedItem);
        }
    }
}
