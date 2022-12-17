using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using OwlCore;
using StrixMusic.AppModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.WinUI;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Shells.Groove;
using StrixMusic.Shells.ZuneDesktop;

namespace StrixMusic.Controls;

[ObservableObject]
public sealed partial class ShellPresenter : UserControl
{
    private Shell? _currentShell;
    private bool _currentIsPreferred;

    /// <summary>
    /// Creates a new instance of <see cref="ShellPresenter"/>.
    /// </summary>
    public ShellPresenter()
    {
        this.InitializeComponent();

        SizeChanged += ShellPresenter_SizeChanged;
        
        RegisterPropertyChangedCallback(WidthProperty, (d, e) => _ = ((ShellPresenter)d).OnSizeChangedAsync());
    }

    private void ShellPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _ = OnSizeChangedAsync();
    }

    private async Task OnSizeChangedAsync()
    {
        // Only check when the user stops resizing the control for a few frames at 24 FPS (41.666ms) instead of the default 1 frame at 60FPS
        // Also disables concurrency
        if (await Flow.Debounce($"{GetHashCode()}.{nameof(OnSizeChangedAsync)}", TimeSpan.FromMilliseconds(41.666 * 2)))
        {
            if (_currentIsPreferred && ShouldUseFallbackShell())
                ApplyFallbackShell();

            if (!_currentIsPreferred && !ShouldUseFallbackShell())
                ApplyPreferredShell();
        }
    }

    /// <summary>
    /// Identifies the <see cref="PreferredShell"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreferredShellProperty =
        DependencyProperty.Register(nameof(PreferredShell), typeof(StrixMusicShells), typeof(ShellPresenter), new PropertyMetadata(StrixMusicShells.ZuneDesktop, (d, e) => ((ShellPresenter)d).OnPreferredShellChanged((StrixMusicShells)e.OldValue, (StrixMusicShells)e.NewValue)));

    /// <summary>
    /// Identifies the <see cref="FallbackShell"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FallbackShellProperty =
        DependencyProperty.Register(nameof(FallbackShell), typeof(AdaptiveShells), typeof(ShellPresenter), new PropertyMetadata(StrixMusicShells.Sandbox, (d, e) => ((ShellPresenter)d).OnFallbackShellChanged((AdaptiveShells)e.OldValue, (AdaptiveShells)e.NewValue)));

    /// <summary>
    /// Identifies the <see cref="Root"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(nameof(Root), typeof(IStrixDataRoot), typeof(ShellPresenter), new PropertyMetadata(null, (d, e) => ((ShellPresenter)d).OnRootChanged(e.OldValue as IStrixDataRoot, e.NewValue as IStrixDataRoot)));

    /// <summary>
    /// The data root used to display content.
    /// </summary>
    public IStrixDataRoot? Root
    {
        get => (IStrixDataRoot?)GetValue(RootProperty);
        set => SetValue(RootProperty, value);
    }

    /// <summary>
    /// The preferred shell to use.
    /// </summary>
    public StrixMusicShells PreferredShell
    {
        get => (StrixMusicShells)GetValue(PreferredShellProperty);
        set => SetValue(PreferredShellProperty, value);
    }

    /// <summary>
    /// The adaptive shell to use when  the<see cref="PreferredShell"/> isn't able to fit the current screen size.
    /// </summary>
    public AdaptiveShells FallbackShell
    {
        get => (AdaptiveShells)GetValue(FallbackShellProperty);
        set => SetValue(FallbackShellProperty, value);
    }

    /// <summary>
    /// Gets a value indicating if the <see cref="FallbackShell"/> is currently active.
    /// </summary>
    public bool IsFallbackShellActive => ShouldUseFallbackShell();

    /// <summary>
    /// Gets a value indicating if the <see cref="PreferredShell"/> is currently active.
    /// </summary>
    public bool IsPreferredShellActive => !ShouldUseFallbackShell();

    private bool ShouldUseFallbackShell()
    {
        if (!IsLoaded)
            return false;

        var currentShellData = ShellInfo.All[PreferredShell];

        var isWindowTooSmall = ActualWidth < currentShellData.MinWindowSize.Width || currentShellData.MinWindowSize.Height > ActualHeight;
        var isWindowTooBig = ActualWidth > currentShellData.MaxWindowSize.Width || currentShellData.MaxWindowSize.Height < ActualHeight;

        return isWindowTooBig || isWindowTooSmall;
    }

    private void OnRootChanged(IStrixDataRoot? oldValue, IStrixDataRoot? newValue)
    {
        if (!IsLoaded)
            return;

        if (oldValue == newValue)
            return;

        if (_currentShell is not null)
            _currentShell.Root = newValue;

        if (_currentShell is null)
        {
            if (!ShouldUseFallbackShell())
                ApplyFallbackShell();
            else
                ApplyPreferredShell();
        }
    }

    private void OnPreferredShellChanged(StrixMusicShells oldValue, StrixMusicShells newValue)
    {
        if (!IsLoaded)
            return;

        if (oldValue == newValue)
            return;

        if (!ShouldUseFallbackShell())
            ApplyPreferredShell();
    }

    private void OnFallbackShellChanged(AdaptiveShells oldValue, AdaptiveShells newValue)
    {
        if (!IsLoaded)
            return;

        if (oldValue == newValue)
            return;

        if (ShouldUseFallbackShell())
            ApplyFallbackShell();
    }

    private void ApplyPreferredShell()
    {
        PART_ShellDisplay.Content = null;

        PART_ShellDisplay.Content = _currentShell = CreatePreferredShell(PreferredShell, Root);
        _currentIsPreferred = true;

        OnPropertyChanged(nameof(IsPreferredShellActive));
        OnPropertyChanged(nameof(IsFallbackShellActive));

        Shell CreatePreferredShell(StrixMusicShells preferredShell, IStrixDataRoot? root)
        {
            var shell = preferredShell switch
            {
                StrixMusicShells.Sandbox => new SandboxShell(),
                StrixMusicShells.GrooveMusic => new GrooveMusic(),
                StrixMusicShells.ZuneDesktop => new ZuneDesktop(),
                _ => ThrowHelper.ThrowNotSupportedException<Shell>("This shell has not been set up for display."),
            };

            shell.Root = root;

            return shell;
        }
    }

    private void ApplyFallbackShell()
    {
        PART_ShellDisplay.Content = null;

        PART_ShellDisplay.Content = _currentShell = CreateFallbackShell(FallbackShell, Root);
        _currentIsPreferred = false;

        OnPropertyChanged(nameof(IsPreferredShellActive));
        OnPropertyChanged(nameof(IsFallbackShellActive));

        Shell CreateFallbackShell(AdaptiveShells adaptiveShells, IStrixDataRoot? root)
        {
            var shell = adaptiveShells switch
            {
                AdaptiveShells.Sandbox => new SandboxShell(),
                AdaptiveShells.GrooveMusic => new GrooveMusic(),
                _ => ThrowHelper.ThrowNotSupportedException<Shell>("This shell has not been set up for display."),
            };

            shell.Root = root;

            return shell;
        }
    }
}
