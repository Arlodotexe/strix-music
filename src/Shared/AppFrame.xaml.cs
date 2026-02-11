using System;
using System.ComponentModel;
using StrixMusic.AppModels;
using StrixMusic.Controls;
using StrixMusic.Sdk.WinUI.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace StrixMusic;

/// <summary>
/// A control to display top-level app content.
/// </summary>
public sealed partial class AppFrame : UserControl
{
    private Shell? _currentShell;

    /// <summary>
    /// Platform-specific action to minimize the window. Set from the platform host (e.g. Program.cs).
    /// </summary>
    internal static Action? MinimizeWindowAction { get; set; }

    /// <summary>
    /// Platform-specific action to maximize/restore the window. Set from the platform host (e.g. Program.cs).
    /// </summary>
    internal static Action? MaximizeWindowAction { get; set; }

    /// <summary>
    /// Platform-specific action to begin a window drag. Set from the platform host (e.g. Program.cs).
    /// </summary>
    internal static Action? BeginWindowDragAction { get; set; }

    private Windows.UI.Xaml.UIElement? _currentTitleBar;

    /// <summary>
    /// Creates a new instance of <see cref="AppFrame"/>.
    /// </summary>
    public AppFrame(AppRoot root)
    {
        InitializeComponent();

        AppRoot = root;

#if HAS_UNO
        CaptionMinimizeBtn.Visibility = Visibility.Visible;
        CaptionMaximizeBtn.Visibility = Visibility.Visible;
        CaptionCloseBtn.Visibility = Visibility.Visible;
#endif
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

        if (Window.Current is not null)
            Window.Current.SetTitleBar(hostOptions?.CustomTitleBar);

#if HAS_UNO
        // On Uno/Skia, SetTitleBar doesn't implement window drag.
        // Manually hook PointerPressed to initiate drag via platform action.
        if (_currentTitleBar != hostOptions?.CustomTitleBar)
        {
            if (_currentTitleBar is not null)
                _currentTitleBar.PointerPressed -= TitleBar_PointerPressed;

            _currentTitleBar = hostOptions?.CustomTitleBar;

            if (_currentTitleBar is not null)
                _currentTitleBar.PointerPressed += TitleBar_PointerPressed;
        }
#endif
    }

    private void ShellOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => SetupShellWindowHostOptions(CurrentApplicationView, CurrentCoreApplicationView, sender as ShellWindowHostOptions);

    private void TitleBar_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        BeginWindowDragAction?.Invoke();
    }

    private void CaptionMinimize_Click(object sender, RoutedEventArgs e)
        => MinimizeWindowAction?.Invoke();

    private void CaptionMaximize_Click(object sender, RoutedEventArgs e)
        => MaximizeWindowAction?.Invoke();

    private void CaptionClose_Click(object sender, RoutedEventArgs e)
    {
#if HAS_UNO
        Application.Current.Exit();
#endif
    }
}
