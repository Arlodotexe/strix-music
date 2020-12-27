using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="AlbumCollection"/>.
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
            INavigationService<Control> navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();
            navigationService.NavigateTo(typeof(AlbumView), false, e.ClickedItem);
        }
    }
}
