using Windows.UI.Xaml;
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
            DependencyProperty.Register(nameof(Main), typeof(MainViewModel), typeof(MediaTransports), new PropertyMetadata(0));
    }
}
