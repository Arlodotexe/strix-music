using System;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Settings;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.SoundCloudCore;

/// <summary>
/// Begins the process of gathering enough data to create a SoundCloud folder. Login, folder selection from root, etc.
/// </summary>
[ObservableObject]
public sealed partial class LandingPage : Page
{
    private ConnectNewMusicSourceNavigationParams? _param;
    [ObservableProperty] private SoundCloudCoreSettings? _settings = null;


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
        //Guard.IsNotNullOrWhiteSpace(Settings.ClientId);
        Guard.IsNotNullOrWhiteSpace(Settings.Token);

        // Configure settings
        Frame.Navigate(typeof(ConfirmAndSave), (_param, Settings));
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
        Settings = (SoundCloudCoreSettings)await _param.SelectedSourceToConnect.DefaultSettingsFactory(string.Empty);

        // To show that the user canceled setup:
        // param.Item1.SetupCompleteTaskCompletionSource.SetCanceled();

        // To show that the user completed setup:
        // param.Item1.SetupCompleteTaskCompletionSource.SetResult(null);

        // To navigate to a different page.
        // Remember to pass the original ConnectNewMusicSourceNavigationParams as well, so you can complete/cancel setup from another page.
        // Frame.Navigate(typeof(NextPageType), nextPageParam));
    }

    private bool AllNotNullOrWhiteSpace(string value, string value2) => IsNotNullOrWhiteSpace(value) && IsNotNullOrWhiteSpace(value2);

    private bool IsNull(object? obj) => obj is null;

    private bool IsNotNull(object? obj) => obj is not null;

    private bool IsNotNullOrWhiteSpace(string value) => !string.IsNullOrWhiteSpace(value);

    private bool InvertBool(bool val) => !val;
}
