using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Sdk.WinUI.Controls.Views.Secondary;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Styles.Collections
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="PlaylistCollection"/>.
    /// </summary>
    public sealed partial class PlaylistCollectionStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistCollectionStyle"/> class.
        /// </summary>
        public PlaylistCollectionStyle()
        {
            this.InitializeComponent();
        }

        private void OpenPlaylist(object sender, ItemClickEventArgs e)
        {
            var navigationService = Ioc.Default.GetRequiredService<INavigationService<Control>>();

            navigationService.NavigateTo(typeof(PlaylistView), false, e.ClickedItem);
        }
    }
}
