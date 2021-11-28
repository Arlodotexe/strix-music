using Windows.UI.Xaml;
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
        /// The viewmodel that holds application's main data.
        /// </summary>
        public MainViewModel Main
        {
            get { return (MainViewModel)GetValue(MainProperty); }
            set { SetValue(MainProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MainViewModel"/>.
        /// </summary>
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainProperty =
            DependencyProperty.Register("MainProperty", typeof(MainViewModel), typeof(MediaInfo), new PropertyMetadata(0));
    }
}
