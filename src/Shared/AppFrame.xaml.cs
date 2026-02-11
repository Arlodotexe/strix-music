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

    /// <summary>
    /// Platform-specific action to begin a window edge resize. Parameter is the edge index (matching Gdk.WindowEdge).
    /// 0=NW, 1=N, 2=NE, 3=W, 4=E, 5=SW, 6=S, 7=SE.
    /// </summary>
    internal static Action<int>? BeginWindowResizeAction { get; set; }

    /// <summary>
    /// Platform-specific action to set the cursor. Parameter is the edge index, or -1 for default.
    /// </summary>
    internal static Action<int>? SetEdgeCursorAction { get; set; }

    private const double ResizeGripSize = 6;
    private int _currentEdge = -1;
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

        RootGrid.PointerMoved += RootGrid_PointerMoved;
        RootGrid.PointerPressed += RootGrid_PointerPressed;
        RootGrid.PointerExited += RootGrid_PointerExited;
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

    /// <summary>
    /// Determines the resize edge for a pointer position, or -1 if not near any edge.
    /// Edge values match Gdk.WindowEdge: 0=NW, 1=N, 2=NE, 3=W, 4=E, 5=SW, 6=S, 7=SE.
    /// </summary>
    private int GetResizeEdge(double x, double y, double width, double height)
    {
        bool left = x < ResizeGripSize;
        bool right = x >= width - ResizeGripSize;
        bool top = y < ResizeGripSize;
        bool bottom = y >= height - ResizeGripSize;

        if (top && left) return 0;       // NorthWest
        if (top && right) return 2;      // NorthEast
        if (bottom && left) return 5;    // SouthWest
        if (bottom && right) return 7;   // SouthEast
        if (top) return 1;               // North
        if (bottom) return 6;            // South
        if (left) return 3;              // West
        if (right) return 4;             // East

        return -1;
    }

    private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var pos = e.GetCurrentPoint(RootGrid).Position;
        var edge = GetResizeEdge(pos.X, pos.Y, RootGrid.ActualWidth, RootGrid.ActualHeight);

        if (edge != _currentEdge)
        {
            _currentEdge = edge;
            SetEdgeCursorAction?.Invoke(edge);
        }
    }

    private void RootGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_currentEdge >= 0)
        {
            BeginWindowResizeAction?.Invoke(_currentEdge);
            e.Handled = true;
        }
    }

    private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_currentEdge != -1)
        {
            _currentEdge = -1;
            SetEdgeCursorAction?.Invoke(-1);
        }
    }
}
