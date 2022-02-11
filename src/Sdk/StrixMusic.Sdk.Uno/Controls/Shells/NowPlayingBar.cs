using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Shells
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the NowPlaying bar in a Shell.
    /// </summary>
    public sealed partial class NowPlayingBar : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingBar"/> class.
        /// </summary>
        public NowPlayingBar()
        {
            this.DefaultStyleKey = typeof(NowPlayingBar);
        }

        /// <summary>
        /// Backing dependency property for <see cref="MainViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveDeviceProperty = DependencyProperty.Register(
            nameof(DeviceViewModel),
            typeof(DeviceViewModel),
            typeof(NowPlayingBar),
            new PropertyMetadata(null));

        /// <summary>
        /// Holds active devices and track playback information.
        /// </summary>
        public DeviceViewModel? ActiveDevice
        {
            get => (DeviceViewModel?)GetValue(ActiveDeviceProperty);
            set => SetValue(ActiveDeviceProperty, value);
        }
    }
}
