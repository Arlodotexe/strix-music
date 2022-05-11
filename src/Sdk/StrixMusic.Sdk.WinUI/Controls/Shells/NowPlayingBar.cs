using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Shells
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the NowPlaying bar in a Shell.
    /// </summary>
    public partial class NowPlayingBar : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingBar"/> class.
        /// </summary>
        public NowPlayingBar()
        {
            this.DefaultStyleKey = typeof(NowPlayingBar);
            AttachEvents();
        }

        private void AttachEvents()
        {
            Unloaded += NowPlayingBar_Unloaded;

            foreach (var device in Devices)
                AttachEvents(device);
        }

        private void DetachEvents()
        {
            Unloaded -= NowPlayingBar_Unloaded;

            foreach (var device in Devices)
                DetachEvents(device);
        }

        /// <summary>
        /// Attaches events to the provided devices.
        /// </summary>
        protected virtual void AttachEvents(IDevice device)
        {
            device.IsActiveChanged += Device_IsActiveChanged;
        }

        /// <summary>
        /// Detaches events from the provided devices.
        /// </summary>
        protected virtual void DetachEvents(IDevice device)
        {
            device.IsActiveChanged -= Device_IsActiveChanged;
        }

        private void Device_IsActiveChanged(object sender, bool e)
        {
            if (e)
            {
                var device = (IDevice)sender;

                if (device is not DeviceViewModel dvm)
                    dvm = new DeviceViewModel(device);

                ActiveDevice = dvm;
            }
        }

        private void NowPlayingBar_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
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
        public IReadOnlyList<IDevice> Devices
        {
            get => (IReadOnlyList<IDevice>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="Devices"/>.
        /// </summary>
        public static readonly DependencyProperty DevicesProperty =
            DependencyProperty.Register(nameof(Devices), typeof(IReadOnlyList<IDevice>), typeof(NowPlayingBar), new PropertyMetadata(new List<IDevice>(), (s, e) => s.Cast<NowPlayingBar>().OnDevicesChanged(e.OldValue.Cast<IReadOnlyList<IDevice>>(), e.NewValue.Cast<IReadOnlyList<IDevice>>())));

        /// <summary>
        /// Backing dependency property for <see cref="ActiveDevice"/>.
        /// </summary>
        private static readonly DependencyProperty ActiveDeviceProperty =
            DependencyProperty.Register(nameof(ActiveDevice), typeof(DeviceViewModel), typeof(NowPlayingBar), new PropertyMetadata(null, (s, e) => s.Cast<NowPlayingBar>().OnActiveDeviceChanged(e.OldValue.Cast<DeviceViewModel>(), e.NewValue.Cast<DeviceViewModel>())));

        /// <summary>
        /// Callback for when the <see cref="ActiveDevice"/> property is changed.
        /// </summary>
        protected virtual void OnActiveDeviceChanged(DeviceViewModel? oldValue, DeviceViewModel? newValue)
        {
        }

        /// <summary>
        /// Callback for when the <see cref="Devices"/> property is changed.
        /// </summary>
        protected virtual void OnDevicesChanged(IReadOnlyList<IDevice> oldValue, IReadOnlyList<IDevice> newValue)
        {
            foreach (var item in oldValue)
                DetachEvents(item);

            foreach (var item in newValue)
                AttachEvents(item);

            var targetDevice = newValue.FirstOrDefault(x => x.IsActive);
            if (targetDevice is null)
                return;

            if (targetDevice is not DeviceViewModel dvm)
                dvm = new DeviceViewModel(targetDevice);

            ActiveDevice = dvm;
        }
    }
}
