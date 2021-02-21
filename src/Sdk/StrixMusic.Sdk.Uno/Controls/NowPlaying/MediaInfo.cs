using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.NowPlaying
{
    /// <summary>
    /// Media info for the currently playing track.
    /// </summary>
    /// <remarks>
    /// TODO: Allow hiding AlbumImage with property
    /// </remarks>
    public partial class MediaInfo : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaInfo"/> class.
        /// </summary>
        public MediaInfo()
        {
            this.DefaultStyleKey = typeof(MediaInfo);
        }

        /// <summary>
        /// The ViewModel for this control.
        /// </summary>
        public MainViewModel ViewModel => (MainViewModel)DataContext;
    }
}
