using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Sdk.WinUI.Controls.Views.Secondary;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            if ((sender as Control)?.DataContext is ArtistViewModel viewModel)
            {
                var navigationService = Shell.Ioc.GetRequiredService<INavigationService<Control>>();

                navigationService.NavigateTo(typeof(ArtistView), false, viewModel);
            }
        }
    }
}
