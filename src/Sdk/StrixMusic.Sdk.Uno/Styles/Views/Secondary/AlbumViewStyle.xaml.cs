using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views.Secondary;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Styles.Views.Secondary
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="AlbumView"/>.
    /// </summary>
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
            if ((sender as Control)?.DataContext is ArtistViewModel viewModel)
            {
                INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();

                navigationService.NavigateTo(typeof(ArtistView), false, viewModel);
            }
        }
    }
}
