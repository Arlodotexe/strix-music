using Microsoft.Toolkit.Diagnostics;
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
        private MainViewModel? _mainViewModel;
        private Color? _backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveNowPlayingBarViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="MainViewModel"/> inside this ViewModel on display.</param>
        public GrooveNowPlayingBarViewModel(MainViewModel viewModel)
        {
            _backgroundColor = null;
            ViewModel = viewModel;
        }

        /// <summary>
        /// The <see cref="MainViewModel"/> inside this ViewModel on display.
        /// </summary>
        public MainViewModel? ViewModel
        {
            get => _mainViewModel;
            set
            {
                SetProperty(ref _mainViewModel, value);
                RegisterViewModelEvents();
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

        private void RegisterViewModelEvents()
        {
            Guard.IsNotNull(_mainViewModel, nameof(_mainViewModel));
            _mainViewModel.DevicesChanged += RegisterDeviceEvents;
        }

        private void RegisterDeviceEvents(object sender, System.Collections.Generic.IReadOnlyList<OwlCore.Events.CollectionChangedItem<Sdk.Data.IDevice>> addedItems, System.Collections.Generic.IReadOnlyList<OwlCore.Events.CollectionChangedItem<Sdk.Data.IDevice>> removedItems)
        {
            Guard.IsNotNull(_mainViewModel?.ActiveDevice, nameof(_mainViewModel.ActiveDevice));

            // TODO: Handle active device change
            _mainViewModel.ActiveDevice.NowPlayingChanged += ActiveDevice_NowPlayingChanged;
        }

        private async void ActiveDevice_NowPlayingChanged(object sender, Sdk.Data.ITrack e)
        {
            BackgroundColor = await Task.Run<Color?>(async () =>
            {
                TrackViewModel? nowPlaying = ViewModel?.ActiveDevice?.NowPlaying;
                if (nowPlaying != null)
                {
                    // Load images if there aren't images loaded.
                    if (nowPlaying.Images.Count == 0)
                    {
                        await nowPlaying.InitImageCollectionAsync();
                    }

                    // If there are now images, grab the color from the first image.
                    if (nowPlaying.Images.Count != 0)
                    {
                        return await DynamicColorHelper.GetImageAccentColorAsync(nowPlaying.Images[0]);
                    }
                }

                return null;
            });
        }
    }
}
