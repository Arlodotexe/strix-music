using System.Collections.ObjectModel;
using System.Linq;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.WinUI.Controls.Shells
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
        /// Holds active devices and track playback information.
        /// </summary>
        private DeviceViewModel? ActiveDevice
        {
            get => (DeviceViewModel?)GetValue(ActiveDeviceProperty);
            set => SetValue(ActiveDeviceProperty, value);
        }

        /// <summary>
        /// A list of devices that can be selected from for displaying playback status.
        /// </summary>
        public ObservableCollection<IDevice> Devices
        {
            get => (ObservableCollection<IDevice>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="Devices"/>.
        /// </summary>
        public static readonly DependencyProperty DevicesProperty =
            DependencyProperty.Register(nameof(Devices), typeof(ObservableCollection<IDevice>), typeof(NowPlayingBar), new PropertyMetadata(null, (s, e) => s.Cast<NowPlayingBar>().OnDevicesChanged()));

        /// <summary>
        /// Backing dependency property for <see cref="ActiveDevice"/>.
        /// </summary>
        private static readonly DependencyProperty ActiveDeviceProperty =
            DependencyProperty.Register(nameof(ActiveDevice), typeof(DeviceViewModel), typeof(NowPlayingBar), new PropertyMetadata(null));

        private void OnDevicesChanged()
        {
            var targetDevice = Devices.FirstOrDefault(x => x.IsActive);
            if (targetDevice is null)
                return;

            if (targetDevice is not DeviceViewModel dvm)
                dvm = new DeviceViewModel(targetDevice);

            ActiveDevice = dvm;
        }
    }
}
