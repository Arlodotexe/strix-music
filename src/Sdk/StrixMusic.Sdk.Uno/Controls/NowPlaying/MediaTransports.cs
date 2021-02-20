using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.NowPlaying
{
    /// <summary>
    /// The Media Transparent controls
    /// </summary>
    public partial class MediaTransports : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaTransports"/> class.
        /// </summary>
        public MediaTransports()
        {
            this.DefaultStyleKey = typeof(MediaTransports);
        }

        /// <summary>
        /// The ViewModel for this control.
        /// </summary>
        public MainViewModel ViewModel => (MainViewModel)DataContext;
    }
}
