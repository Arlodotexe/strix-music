using StrixMusic.CoreInterfaces.Interfaces;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.Shell.Default.Controls
{
    public sealed partial class HomeTabControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeTabControl"/> class.
        /// </summary>
        public HomeTabControl()
        {
            this.DefaultStyleKey = typeof(HomeTabControl);
        }

        private IPlayableCollectionBase? ViewModel => DataContext as IPlayableCollectionBase;
    }
}
