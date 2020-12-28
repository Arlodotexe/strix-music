using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="ArtistCollection"/>.
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
            INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();
            navigationService.NavigateTo(typeof(ArtistView), false, e.ClickedItem);
        }
    }
}
