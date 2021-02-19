using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views.Secondary;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Styles.Collections
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
            INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();
            navigationService.NavigateTo(typeof(PlaylistView), false, e.ClickedItem);
        }
    }
}
