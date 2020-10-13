using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Observables;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Styles
{
    public sealed partial class ArtistCollectionStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistCollectionStyle "/> class.
        /// </summary>
        public ArtistCollectionStyle()
        {
            this.InitializeComponent();
        }

        private void OpenAlbum(object sender, ItemClickEventArgs e)
        {
            INavigationService<Control> navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
            navigationService.NavigateTo(typeof(ArtistView), false, e.ClickedItem);
        }
    }
}
