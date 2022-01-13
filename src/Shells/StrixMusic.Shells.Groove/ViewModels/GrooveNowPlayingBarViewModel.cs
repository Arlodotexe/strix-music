﻿using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Helper;
using System.Threading.Tasks;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels
{
    /// <summary>
    /// The ViewModel for the <see cref="Controls.GrooveNowPlayingBar"/>.
    /// </summary>
    public class GrooveNowPlayingBarViewModel : ObservableObject
    {
        private DeviceViewModel? _activeDevice;
        private Color? _backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveNowPlayingBarViewModel"/> class.
        /// </summary>
        public GrooveNowPlayingBarViewModel()
        {
            _backgroundColor = null;
        }

        /// <summary>
        /// The <see cref="MainViewModel"/> inside this ViewModel on display.
        /// </summary>
        public DeviceViewModel? ActiveDevice
        {
            get => _activeDevice;
            set
            {
                if (!(_activeDevice is null))
                    DetachEvents(_activeDevice);

                if (!(value is null))
                    AttachEvents(value);

                SetProperty(ref _activeDevice, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the <see cref="Controls.GrooveNowPlayingBar"/> background.
        /// </summary>
        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        private void AttachEvents(DeviceViewModel device)
        {
            device.NowPlayingChanged += ActiveDevice_NowPlayingChanged;
        }

        private void DetachEvents(DeviceViewModel device)
        {
            device.NowPlayingChanged -= ActiveDevice_NowPlayingChanged;
        }

        private async void ActiveDevice_NowPlayingChanged(object sender, Sdk.Models.Core.ICoreTrack e)
        {
            // Load images if there aren't images loaded.
            // Uncommenting this will cause NowPlaying album art to break randomly while skipping tracks.
            // MAybe just ask the api for the first image directly, glhf.
            // await nowPlaying.InitImageCollectionAsync();

            // If there are now images, grab the color from the first image.
            if (e.TotalImageCount != 0)
            {
                await foreach (var image in e.GetImagesAsync(1, 0))
                {
                    BackgroundColor = await Task.Run(() => DynamicColorHelper.GetImageAccentColorAsync(image.Uri));
                }
            }
        }
    }
}
