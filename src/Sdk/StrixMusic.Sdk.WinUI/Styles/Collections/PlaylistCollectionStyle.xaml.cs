using CommunityToolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.WinUI.Controls.Views.Secondary;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.WinUI.Controls.Collections;

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
    }
}
