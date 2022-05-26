using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.NowPlaying
{
    /// <summary>
    /// Media info for the currently playing track.
    /// </summary>
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
        public static readonly DependencyProperty DeviceProperty =
            DependencyProperty.Register(nameof(Device), typeof(DeviceViewModel), typeof(MediaInfo), new PropertyMetadata(null));
    }
}
