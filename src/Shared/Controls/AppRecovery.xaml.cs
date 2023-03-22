using System;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Extensions;
using StrixMusic.AppModels;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls;

/// <summary>
/// Displayed to the user to recover from an unhealthy app shutdown.
/// </summary>
public sealed partial class AppRecovery : UserControl
{
    private readonly Func<AppRoot> _faultedAppRootFactory;

    /// <summary>
    /// Creates a new instance of <see cref="AppRecovery"/>.
    /// </summary>
    /// <param name="healthManager">The health manager used to catch and manage the <see cref="FaultedAppRoot"/>.</param>
    /// <param name="faultedAppRootFactory">A factory to create the <see cref="AppRoot"/> that was in use when the app faulted.</param>
    public AppRecovery(ApplicationHealthManager healthManager, Func<AppRoot> faultedAppRootFactory)
    {
        Guard.IsTrue(healthManager.State.UnhealthyShutdown);
        _faultedAppRootFactory = faultedAppRootFactory;

        HealthManager = healthManager;

        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
    }

    private static void ResetToHealthyState(ApplicationHealthManager healthManager)
    {
        try
        {
            healthManager.State = new ApplicationHealthState()
            {
                ReasonWhitelist = healthManager.State.ReasonWhitelist,
            };

            healthManager.FlushState();
        }
        catch (Exception ex)
        {
            _ = new ContentDialog()
            {
                Title = "Unable to reset app to healthy state",
                Content = new TextBlock { Text = "A critical error has occurred and the app health could not be reset. Normal startup is no longer possible. Please reset or reinstall the app." },
                PrimaryButtonText = "Close app"
            }
                .ShowAsync(ShowType.Interrupt)
                .ContinueWith(_ =>
                {
                    healthManager.App.Exit();
                });
        }
    }

    /// <summary>
    /// The health manager used to catch and manage the faulted app.
    /// </summary>
    public ApplicationHealthManager HealthManager { get; }

    private void OnSuppressErrorClick(object sender, RoutedEventArgs e)
    {
        Guard.IsNotNullOrWhiteSpace(HealthManager.State.UnhealthyShutdownReason);

        if (HealthManager.State.UnhealthyShutdownReason.Contains(nameof(IntentionalCrashException)))
        {
            _ = new ContentDialog()
                {
                    Title = "Not allowed",
                    Content = new TextBlock { Text = "This error is used for debug purposes and cannot be suppressed." },
                    PrimaryButtonText = "Ok"
                }
                .ShowAsync(ShowType.Interrupt);
        }

        if (!HealthManager.State.ReasonWhitelist.Contains(HealthManager.State.UnhealthyShutdownReason))
        {
            HealthManager.State.ReasonWhitelist.Add(HealthManager.State.UnhealthyShutdownReason);
            HealthManager.FlushState();
        }
    }

    private void ContinueToAppOnClick(object sender, RoutedEventArgs e)
    {
        Guard.IsNotNull(HealthManager.App.MainWindow);

        ResetToHealthyState(HealthManager);
        HealthManager.App.MainWindow.Content = HealthManager.App.CreateMainWindowContent();
    }

    [RelayCommand]
    private async Task OpenLogFolderAsync()
    {
        var logsLocation = ApplicationData.Current.LocalCacheFolder.Path + @"\logs";
        var folder = await StorageFolder.GetFolderFromPathAsync(logsLocation);

        await Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private async Task ResetAllSettingsAsync()
    {
        var appRoot = _faultedAppRootFactory();
        var appDiagnosticDataFolder = await appRoot.GetOrCreateSettingsFolderAsync(nameof(AppDiagnostics));

        var diag = new AppDiagnostics(appDiagnosticDataFolder);
        await diag.DeleteUserDataCommand.ExecuteAsync(null);
    }
}
