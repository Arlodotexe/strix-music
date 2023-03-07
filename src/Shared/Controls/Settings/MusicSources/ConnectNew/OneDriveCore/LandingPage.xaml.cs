using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graph;
using StrixMusic.Settings;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.OneDriveCore;

/// <summary>
/// Begins the process of gathering enough data to create a OneDrive folder. Login, folder selection from root, etc.
/// </summary>
[ObservableObject]
public sealed partial class LandingPage : Page
{
    private ConnectNewMusicSourceNavigationParams? _param;
    [ObservableProperty] private OneDriveCoreSettings? _settings = null;

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
        Guard.IsNotNullOrWhiteSpace(Settings.ClientId);
        Guard.IsNotNullOrWhiteSpace(Settings.TenantId);

        Frame.Navigate(typeof(DeviceCodeLogin), (_param, Settings));
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
        Settings = (OneDriveCoreSettings)await _param.SelectedSourceToConnect.DefaultSettingsFactory(string.Empty);

        if (!string.IsNullOrWhiteSpace(Settings.ClientId) && !string.IsNullOrWhiteSpace(Settings.TenantId) && !string.IsNullOrWhiteSpace(Settings.RedirectUri))
            Frame.Navigate(typeof(DeviceCodeLogin), (_param, Settings));

        // To show that the user canceled setup:
        // param.Item1.SetupCompleteTaskCompletionSource.SetCanceled();

        // To show that the user completed setup:
        // param.Item1.SetupCompleteTaskCompletionSource.SetResult(null);

        // To navigate to a different page.
        // Remember to pass the original ConnectNewMusicSourceNavigationParams as well, so you can complete/cancel setup from another page.
        // Frame.Navigate(typeof(NextPageType), nextPageParam));
    }

    private bool AllNotNullOrWhiteSpace(string value, string value2) => !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2);

    private bool IsNull(object? obj) => obj is null;

    private bool IsNotNull(object? obj) => obj is not null;

    private bool InvertBool(bool val) => !val;
}
