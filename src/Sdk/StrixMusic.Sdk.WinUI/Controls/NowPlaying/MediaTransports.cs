using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.NowPlaying
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
        /// The ViewModel that holds the active device.
        /// </summary>
        public DeviceViewModel Device
        {
            get { return (DeviceViewModel)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="DeviceViewModel"/>.
        /// </summary>
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceProperty =
            DependencyProperty.Register(nameof(Device), typeof(DeviceViewModel), typeof(MediaTransports), new PropertyMetadata(null));
    }
}
