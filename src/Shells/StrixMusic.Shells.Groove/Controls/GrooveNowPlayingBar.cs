using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Shells.Groove.Helper;

namespace StrixMusic.Shells.Groove.Controls
{
    /// <summary>
    /// A <see cref="Control"/> to display the now playing bar.
    /// </summary>
    public partial class GrooveNowPlayingBar : Control
    {
        /// <summary>
        /// Backing dependency property for <see cref="ActiveDevice"/>.
        /// </summary>
        public static readonly DependencyProperty ActiveDeviceProperty =
            DependencyProperty.Register(nameof(ActiveDevice), typeof(DeviceViewModel), typeof(GrooveNowPlayingBar), new PropertyMetadata(null, (d, e) => d.Cast<GrooveNowPlayingBar>().OnActiveDeviceChanged((DeviceViewModel?)e.OldValue, (DeviceViewModel?)e.NewValue)));
        
        /// <summary>
        /// Backing dependency property for <see cref="BackgroundColor"/>.
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color?), typeof(GrooveNowPlayingBar), new PropertyMetadata(null));

        /// <summary>
        /// The backing dependency property for <see cref="Devices"/>.
        /// </summary>
        public static readonly DependencyProperty DevicesProperty =
            DependencyProperty.Register(nameof(Devices), typeof(ObservableCollection<IDevice>), typeof(NowPlayingBar), new PropertyMetadata(null, (s, e) => s.Cast<GrooveNowPlayingBar>().OnDevicesChanged()));


        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveNowPlayingBar"/> class.
        /// </summary>
        public GrooveNowPlayingBar()
        {
            DefaultStyleKey = typeof(GrooveNowPlayingBar);
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
        /// Holds active devices and track playback information.
        /// </summary>
        public DeviceViewModel? ActiveDevice
        {
            get => (DeviceViewModel?)GetValue(ActiveDeviceProperty);
            set => SetValue(ActiveDeviceProperty, value);
        }
        
        /// <summary>
        /// Gets or sets the color of the <see cref="Controls.GrooveNowPlayingBar"/> background.
        /// </summary>
        public Color? BackgroundColor
        {
            get => (Color?)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        private void AttachEvents(DeviceViewModel device)
        {
            device.NowPlayingChanged += ActiveDevice_NowPlayingChanged;
        }

        private void DetachEvents(DeviceViewModel device)
        {
            device.NowPlayingChanged -= ActiveDevice_NowPlayingChanged;
        }

        private async void ActiveDevice_NowPlayingChanged(object sender, PlaybackItem e)
        {
            // Load images if there aren't images loaded.
            // Uncommenting this will cause NowPlaying album art to break randomly while skipping tracks.
            // MAybe just ask the api for the first image directly, glhf.
            Guard.IsNotNull(e.Track, nameof(e.Track));

            if (e.Track.TotalImageCount != 0)
            {
                // If there are images, grab the color from the first image.
                var images = await e.Track.GetImagesAsync(1, 0).ToListAsync();

                foreach (var image in images)
                    BackgroundColor = await Task.Run(() => DynamicColorHelper.GetImageAccentColorAsync(image.Uri));
            }
        }

        private void OnActiveDeviceChanged(DeviceViewModel? oldValue, DeviceViewModel? newValue)
        {
            if (!(oldValue is null))
                DetachEvents(oldValue);

            if (!(newValue is null))
                AttachEvents(newValue);
        }

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
