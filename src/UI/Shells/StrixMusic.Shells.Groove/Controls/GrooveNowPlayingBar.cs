using StrixMusic.Shells.Groove.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls
{
    /// <summary>
    /// A <see cref="Control"/> to display the now playing bar.
    /// </summary>
    public partial class GrooveNowPlayingBar : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveNowPlayingBar"/> class.
        /// </summary>
        public GrooveNowPlayingBar()
        {
            this.DefaultStyleKey = typeof(GrooveNowPlayingBar);
        }

        /// <summary>
        /// The <see cref="GrooveNowPlayingBarViewModel"/> for the <see cref="GrooveNowPlayingBar"/> template.
        /// </summary>
        public GrooveNowPlayingBarViewModel ViewModel => (GrooveNowPlayingBarViewModel)DataContext;
    }
}
