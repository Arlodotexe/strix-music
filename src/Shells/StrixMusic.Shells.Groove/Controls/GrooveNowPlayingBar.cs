using OwlCore.Extensions;
using StrixMusic.Sdk;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels;
using Windows.UI.Xaml;
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
            DefaultStyleKey = typeof(GrooveNowPlayingBar);
            DataContext = new GrooveNowPlayingBarViewModel();
        }

        /// <summary>
        /// Backing dependency property for <see cref="MainViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveDeviceProperty =
            DependencyProperty.Register(nameof(DeviceViewModel), typeof(DeviceViewModel), typeof(GrooveNowPlayingBar), new PropertyMetadata(null, (d, e) => d.Cast<GrooveNowPlayingBar>().OnActiveDeviceChanged()));

        /// <summary>
        /// The <see cref="GrooveNowPlayingBarViewModel"/> for the <see cref="GrooveNowPlayingBar"/> template.
        /// </summary>
        public GrooveNowPlayingBarViewModel ViewModel => (GrooveNowPlayingBarViewModel)DataContext;

        /// <summary>
        /// Holds active devices and track playback information.
        /// </summary>
        public DeviceViewModel? ActiveDevice
        {
            get => (DeviceViewModel?)GetValue(ActiveDeviceProperty);
            set => SetValue(ActiveDeviceProperty, value);
        }

        private void OnActiveDeviceChanged()
        {
            ViewModel.ActiveDevice = ActiveDevice;
        }
    }
}
