using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Controls.Settings.MusicSources.ConnectNew.OneDriveCore;
using StrixMusic.Settings;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.Ipfs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ObservableObject]
    public sealed partial class LandingPage : Page
    {
        private ConnectNewMusicSourceNavigationParams? _param;
        [ObservableProperty] private IpfsCoreSettings? _settings = null;

        /// <summary>
        /// Creates a new instance of <see cref="LandingPage"/>.
        /// </summary>
        public LandingPage()
        {
            this.InitializeComponent();
        }

        [RelayCommand(FlowExceptionsToTaskScheduler = true)]
        private async Task TryContinueAsync()
        {
            await Task.Yield();

            Guard.IsNotNull(Settings);
            Guard.IsNotNullOrWhiteSpace(Settings.IpfsCidPath);
            Frame.Navigate(typeof(FolderSelector), (_param, Settings));
        }

        [RelayCommand]
        private void Cancel()
        {
            Guard.IsNotNull(_param);
            _param.SetupCompleteTaskCompletionSource.SetCanceled();
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var param = (ConnectNewMusicSourceNavigationParams)e.Parameter;
            Guard.IsNotNull(param.SelectedSourceToConnect);

            // Save in a field to access from another method
            _param = param;

            // The instance ID here is a temporary placeholder.
            // We need to log in and get a folder ID before we can create the final instance ID.
            Settings = (IpfsCoreSettings)await _param.SelectedSourceToConnect.DefaultSettingsFactory(string.Empty);
        }

        private bool AllNotNullOrWhiteSpace(string value) => !string.IsNullOrWhiteSpace(value);

        private bool IsNull(object? obj) => obj is null;

        private bool IsNotNull(object? obj) => obj is not null;

        private bool InvertBool(bool val) => !val;
    }
}
