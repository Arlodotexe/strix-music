using StrixMusic.Sdk.WinUI.Controls;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.WinUI.Styles.Views.Secondary
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="PlaylistView"/>.
    /// </summary>
    public sealed partial class PlaylistViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewStyle"/> class.
        /// </summary>
        public PlaylistViewStyle()
        {
            this.InitializeComponent();
        }

        //private void GoToOwner(object sender, RoutedEventArgs e)
        //{
        //    if ((sender as Control)?.DataContext is ArtistViewModel viewModel)
        //    {
        //        INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();

        //        navigationService.NavigateTo(typeof(ArtistView), false, viewModel);
        //    }
        //}
    }
}
