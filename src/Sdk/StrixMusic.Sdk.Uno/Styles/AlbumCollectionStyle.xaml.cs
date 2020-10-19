using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace StrixMusic.Sdk.Uno.Styles
{
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
            INavigationService<Control> navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
            navigationService.NavigateTo(typeof(AlbumView), false, e.ClickedItem);
        }
    }
}
