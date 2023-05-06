using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Identity.Client;
using OwlCore.Extensions;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.OneDriveCore;

/// <summary>
/// A page that can be used to perform device-code login.
/// </summary>
[ObservableObject]
public sealed partial class DeviceCodeLogin : Page
{
    private SynchronizationContext _synchronizationContext;
    private ConnectNewMusicSourceNavigationParams? _param;
    [ObservableProperty] private OneDriveCoreSettings? _settings = null;
    [ObservableProperty] private DeviceCodeResult? _deviceCodeResult;
    [ObservableProperty] private string _status = "Waiting";

    /// <summary>
    /// Creates a new instance of <see cref="DeviceCodeLogin"/>.
    /// </summary>
    public DeviceCodeLogin()
    {
        _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        this.InitializeComponent();
    }

    /// <inheritdoc />
    override protected void OnNavigatedTo(NavigationEventArgs e)
    {
        var param = ((ConnectNewMusicSourceNavigationParams NavParams, OneDriveCoreSettings Settings))e.Parameter;
        Guard.IsNotNull(param.NavParams);

        _param = param.NavParams;
        Settings = param.Settings;
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task StartDeviceCodeAuthAsync()
    {
        Guard.IsNotNull(Settings);
        Guard.IsNotNullOrWhiteSpace(Settings.ClientId);
        Guard.IsNotNullOrWhiteSpace(Settings.TenantId);

        // Setup to create a graph client
        var scopes = new[] { "Files.Read.All", "User.Read", "Files.ReadWrite" };
        var authorityUri = "https://login.microsoftonline.com/consumers";

        Status = "Building OneDrive Client";
        var clientAppBuilder = PublicClientApplicationBuilder.Create(Settings.ClientId).WithAuthority(authorityUri, false);

        if (!string.IsNullOrWhiteSpace(Settings.RedirectUri))
            clientAppBuilder = clientAppBuilder.WithRedirectUri(Settings.RedirectUri);

        var clientApp = clientAppBuilder.Build();
        OwlCore.Diagnostics.Logger.LogInformation($"Public client application created. Authenticating.");

        Status = "Waiting for authentication";
        var authResult = await clientApp.AcquireTokenWithDeviceCode(scopes, x =>
        {
            return _synchronizationContext.PostAsync(() =>
            {
                DeviceCodeResult = x;
                return Task.CompletedTask;
            });
        }).ExecuteAsync();

        Settings.UserId = authResult.Account.HomeAccountId.Identifier;
        Settings.UserDisplayName = authResult.Account.Username;

        Status = "Authenticated, proceeding to folder picker";

        Frame.Navigate(typeof(FolderSelector), (_param, Settings));
    }

    [RelayCommand]
    private void Cancel()
    {
        Guard.IsNotNull(_param);
        _param.SetupCompleteTaskCompletionSource.SetCanceled();
    }

    private bool AllNotNullOrWhiteSpace(string value, string value2) => !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2);

    private bool IsNull(object? obj) => obj is null;

    private bool IsNotNull(object? obj) => obj is not null;

    private Visibility IsNullToVisibility(object? obj) => obj is null ? Visibility.Visible : Visibility.Collapsed;

    private Visibility IsNotNullToVisibility(object? obj) => obj is not null ? Visibility.Visible : Visibility.Collapsed;

    private bool InvertBool(bool val) => !val;

    private Visibility BoolToVisibility(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

    private Visibility InvertBoolToVisibility(bool val) => !val ? Visibility.Visible : Visibility.Collapsed;

    private Uri StringToUri(string value) => new Uri(value);
}
