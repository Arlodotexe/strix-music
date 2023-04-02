using System.ComponentModel;
using Windows.ApplicationModel.Core;
using OwlCore.Storage.Uwp;
using StrixMusic.AppModels;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using StrixMusic.Controls;
using StrixMusic.Sdk.WinUI.Controls;

namespace StrixMusic;

/// <summary>
/// A control to display top-level app content.
/// </summary>
public sealed partial class AppFrame : UserControl
{
    private Shell? _currentShell;

    /// <summary>
    /// Creates a new instance of <see cref="AppFrame"/>.
    /// </summary>
    public AppFrame(AppRoot root)
    {
        InitializeComponent();

        AppRoot = root;
    }

    /// <summary>
    /// Represents the active application view and its associative behaviors.
    /// </summary>
    public ApplicationView CurrentApplicationView { get; } = ApplicationView.GetForCurrentView();

    /// <summary>
    /// Represents an app window and its thread.
    /// </summary>
    public CoreApplicationView CurrentCoreApplicationView { get; } = CoreApplication.GetCurrentView();

    /// <summary>
    /// The root for all data required by the Strix Music App to function.
    /// </summary>
    public AppRoot AppRoot { get; }

    private void ShellPresenter_CurrentShellChanged(object? sender, Shell? e)
    {
        var shellPresenter = sender as ShellPresenter;

        if (_currentShell is not null)
            _currentShell.WindowHostOptions.PropertyChanged -= ShellOnPropertyChanged;

        _currentShell = shellPresenter?.CurrentShell;

        if (_currentShell?.WindowHostOptions is not null)
        {
            SetupShellWindowHostOptions(CurrentApplicationView, CurrentCoreApplicationView, _currentShell.WindowHostOptions);
            _currentShell.WindowHostOptions.PropertyChanged += ShellOnPropertyChanged;
        }
    }

    private void SetupShellWindowHostOptions(ApplicationView applicationView, CoreApplicationView coreApplicationView, ShellWindowHostOptions? hostOptions)
    {
        applicationView.TitleBar.BackgroundColor = hostOptions?.BackgroundColor;
        applicationView.TitleBar.ButtonBackgroundColor = hostOptions?.ButtonBackgroundColor;
        applicationView.TitleBar.ButtonForegroundColor = hostOptions?.ButtonForegroundColor;
        applicationView.TitleBar.ButtonHoverBackgroundColor = hostOptions?.ButtonHoverBackgroundColor;
        applicationView.TitleBar.ButtonHoverForegroundColor = hostOptions?.ButtonHoverForegroundColor;
        applicationView.TitleBar.ButtonInactiveBackgroundColor = hostOptions?.ButtonInactiveBackgroundColor;
        applicationView.TitleBar.ButtonInactiveForegroundColor = hostOptions?.ButtonInactiveForegroundColor;
        applicationView.TitleBar.ButtonInactiveForegroundColor = hostOptions?.ButtonInactiveForegroundColor;
        applicationView.TitleBar.ButtonPressedForegroundColor = hostOptions?.ButtonPressedForegroundColor;
        applicationView.TitleBar.ForegroundColor = hostOptions?.ForegroundColor;
        applicationView.TitleBar.InactiveBackgroundColor = hostOptions?.InactiveBackgroundColor;
        applicationView.TitleBar.InactiveForegroundColor = hostOptions?.InactiveForegroundColor;

        coreApplicationView.TitleBar.ExtendViewIntoTitleBar = hostOptions?.ExtendViewIntoTitleBar ?? true;
        Window.Current.SetTitleBar(hostOptions?.CustomTitleBar);
    }

    private void ShellOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => SetupShellWindowHostOptions(CurrentApplicationView, CurrentCoreApplicationView, sender as ShellWindowHostOptions);
}
