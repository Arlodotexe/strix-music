using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using OwlCore.Storage;
using OwlCore.Storage.OneDrive;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StrixMusic.AppModels;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.OneDriveCore;

/// <summary>
/// A page that can be used to perform device-code login.
/// </summary>
[ObservableObject]
public sealed partial class FolderSelector : Page
{
    private SynchronizationContext _synchronizationContext;
    private AuthenticationResult? _authResult;
    private ConnectNewMusicSourceNavigationParams? _param;
    [ObservableProperty] private OneDriveCoreSettings? _settings = null;
    [ObservableProperty] private OneDriveFolder? _rootFolder;

    /// <summary>
    /// Creates a new instance of <see cref="FolderSelector"/>.
    /// </summary>
    public FolderSelector()
    {
        _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        this.InitializeComponent();
    }

    /// <inheritdoc />
    override protected async void OnNavigatedTo(NavigationEventArgs e)
    {
        var param = ((ConnectNewMusicSourceNavigationParams NavParams, OneDriveCoreSettings Settings))e.Parameter;
        Guard.IsNotNull(param.NavParams);

        _param = param.NavParams;
        Settings = param.Settings;

        await AuthenticateCommand.ExecuteAsync(null);
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    private async Task AuthenticateAsync(CancellationToken cancellationToken)
    {
        Guard.IsNotNull(_param);
        Guard.IsNotNull(Settings);
        Guard.IsNotNullOrWhiteSpace(Settings.ClientId);
        Guard.IsNotNullOrWhiteSpace(Settings.UserId);

        // Setup to create a graph client
        var scopes = new[] { "Files.Read.All", "User.Read", "Files.ReadWrite" };
        var authorityUri = "https://login.microsoftonline.com/consumers";

        // Check for cached token
        var clientAppBuilder = PublicClientApplicationBuilder.Create(Settings.ClientId).WithAuthority(authorityUri, false);

        if (!string.IsNullOrWhiteSpace(Settings.RedirectUri))
            clientAppBuilder = clientAppBuilder.WithRedirectUri(Settings.RedirectUri);

        var clientApp = clientAppBuilder.Build();
        OwlCore.Diagnostics.Logger.LogInformation($"Public client application created. Authenticating.");

        // Authenticate
        var account = await clientApp.GetAccountAsync(Settings.UserId);
        _authResult = await clientApp.AcquireTokenSilent(scopes, account).ExecuteAsync(cancellationToken);

        await GetRootFolderCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task PickFolderAsync(IFolder? folder)
    {
        if (folder is null)
            return;

        Guard.IsNotNull(_param?.AppRoot?.MusicSourcesSettings);
        Guard.IsNotNull(Settings);
        Guard.IsNotNull(RootFolder);

        var relativePath = await RootFolder.GetRelativePathToAsync((IStorableChild)folder);
        Settings.FolderId = folder.Id;
        Settings.RelativeFolderPath = relativePath;

        _param.AppRoot.MusicSourcesSettings.ConfiguredOneDriveCores.Add(Settings);

        _param.SetupCompleteTaskCompletionSource.SetResult(null);
    }

    [RelayCommand]
    private async Task GetRootFolderAsync(CancellationToken cancellationToken)
    {
        Guard.IsNotNull(_param);
        Guard.IsNotNull(_authResult);

        var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new OneDriveAccessTokenProvider(_authResult.AccessToken));

        // Create graph client
        var httpClient = GraphClientFactory.Create(GraphClientFactory.CreateDefaultHandlers(), finalHandler: _param.HttpMessageHandler);
        var graphClient = new GraphServiceClient(httpClient, authenticationProvider);

        var drive = await graphClient.Me.Drive.GetAsync(cancellationToken: cancellationToken);
        var driveItem = await graphClient.Drives[drive.Id].Root.GetAsync(cancellationToken: cancellationToken);

        // Create storage abstraction and core.
        RootFolder = new OneDriveFolder(graphClient, driveItem);
    }

    [RelayCommand]
    private void Cancel()
    {
        Guard.IsNotNull(_param);
        _param.SetupCompleteTaskCompletionSource.SetCanceled();
    }

    private IFolder? AsFolder(object obj) => obj as IFolder;

    private bool IsFolder(object obj) => obj is IFolder;

    private bool And(bool val1, bool val2) => val1 && val2;

    private bool AndNot(bool val1, bool val2) => !(val1 && val2);

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
