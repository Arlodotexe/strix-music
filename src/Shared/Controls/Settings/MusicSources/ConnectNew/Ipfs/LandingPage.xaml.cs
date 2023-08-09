using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph.Models;
using OwlCore.Kubo;
using OwlCore.Storage;
using StrixMusic.Controls.Settings.MusicSources.ConnectNew.OneDriveCore;
using StrixMusic.Helpers;
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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.Ipfs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ObservableObject]
    public sealed partial class LandingPage : Page
    {
        [ObservableProperty] private ConnectNewMusicSourceNavigationParams? _param;
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
            Guard.IsNotNull(_param?.AppRoot?.MusicSourcesSettings);
            Guard.IsNotNull(Settings);
            Guard.IsNotNull(_param?.AppRoot?.Ipfs?.Client);
            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.IpfsCidPath))
                {
                    var cid = Settings.IpfsCidPath.Replace("/ipfs/", string.Empty).Split('/')[0];
                    cid = cid.Replace("ipfs://", string.Empty).Split('/')[0];
                    var path = Settings.IpfsCidPath.Replace(cid, string.Empty);
                    var rootFolder = new IpfsFolder(cid, _param.AppRoot.Ipfs.Client);

                    try
                    {
                        Guard.IsNotNull(rootFolder);
                        ConfigureCore(rootFolder.Id);
                    }
                    catch
                    {
                        // Ignore
                    }
                }
                else if (!string.IsNullOrWhiteSpace(Settings.IpnsAddress))
                {
                    var folder = new IpnsFolder(Settings.IpnsAddress, _param.AppRoot.Ipfs.Client);

                    try
                    {
                        // validating the folder
                        var root = await folder.GetRootAsync();
                        ConfigureCore(folder.Id);
                    }
                    catch
                    {
                        // Ignore
                    }
                }
            }
            catch
            {
                // Ignore
            }

            void ConfigureCore(string id)
            {
                Guard.IsNotNull(Settings);
                Settings.IpfsCidPath = id;
                _param.AppRoot.MusicSourcesSettings.ConfiguredIpfsCores.Add(Settings);
                _param.SetupCompleteTaskCompletionSource.SetResult(null);
            }
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
            Settings.InstanceId = Settings.Folder.Id;
        }

        private bool IsAnyValidAddress(string value, string value2) => !string.IsNullOrWhiteSpace(value)
                                                                            || !string.IsNullOrWhiteSpace(value2);

        private bool IsNull(object? obj) => obj is null;

        private bool IsNotNull(object? obj) => obj is not null;

        private bool InvertBool(bool val) => !val;

        private Visibility NullToVisibility(object? val) => val == null ? Visibility.Collapsed : Visibility.Visible;
        private Visibility NullToInveerseVisibility(object? val) => val == null ? Visibility.Visible : Visibility.Collapsed;
    }
}
