using System;
using CommunityToolkit.Diagnostics;
using OwlCore;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.WinUI;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Shells.Groove;
using StrixMusic.Shells.ZuneDesktop;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic;

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
    }

    private async void ShellPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Only check when user has stopped resizing window for x amount of time
        // 41.666ms = 24fps
        if (await Flow.Debounce($"{sender.GetHashCode()}", TimeSpan.FromMilliseconds(41.666)))
        {
            if (_currentIsPreferred && ShouldUseFallbackShell())
                ApplyFallbackShell();

            if (!_currentIsPreferred && !ShouldUseFallbackShell())
                OnPreferredShellChanged();
        }
    }

    /// <summary>
    /// Identifies the <see cref="PreferredShell"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreferredShellProperty =
        DependencyProperty.Register(nameof(PreferredShell), typeof(StrixMusicShells), typeof(ShellPresenter), new PropertyMetadata(StrixMusicShells.ZuneDesktop, (d, e) => ((ShellPresenter)d).OnPreferredShellChanged()));

    /// <summary>
    /// Identifies the <see cref="FallbackShell"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FallbackShellProperty =
        DependencyProperty.Register(nameof(FallbackShell), typeof(AdaptiveShells), typeof(ShellPresenter), new PropertyMetadata(StrixMusicShells.Sandbox, (d, e) => ((ShellPresenter)d).OnFallbackShellChanged()));

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
        var currentShellData = ShellInfo.All[PreferredShell];
        var windowSize = CoreWindow.GetForCurrentThread().Bounds;

        var isWindowTooSmall = windowSize.Width < currentShellData.MinWindowSize.Width || currentShellData.MinWindowSize.Height < windowSize.Height;
        var isWindowTooBig = windowSize.Width > currentShellData.MaxWindowSize.Width || currentShellData.MaxWindowSize.Height > windowSize.Height;

        return isWindowTooBig || isWindowTooSmall;
    }

    private void OnRootChanged(IStrixDataRoot? oldValue, IStrixDataRoot? newValue)
    {
        if (oldValue == newValue)
            return;

        if (_currentShell is not null)
            _currentShell.Root = newValue;
    }

    private void OnPreferredShellChanged()
    {
        if (!ShouldUseFallbackShell())
            ApplyPreferredShell();
    }

    private void OnFallbackShellChanged()
    {
        if (ShouldUseFallbackShell())
            ApplyFallbackShell();
    }

    private void ApplyPreferredShell()
    {
        PART_ShellDisplay.Content = null;

        if (Root is null)
            return;

        PART_ShellDisplay.Content = _currentShell = CreatePreferredShell(PreferredShell, Root);
        _currentIsPreferred = true;

        Shell CreatePreferredShell(StrixMusicShells preferredShell, IStrixDataRoot root)
        {
            Shell shell = preferredShell switch
            {
                StrixMusicShells.Sandbox => new SandboxShell(),
                StrixMusicShells.GrooveMusic => new GrooveMusic(),
                StrixMusicShells.ZuneDesktop => new ZuneDesktop(),
                _ => throw new NotSupportedException("This shell has not been set up for display."),
            };

            shell.Root = root;

            return shell;
        }
    }

    private void ApplyFallbackShell()
    {
        PART_ShellDisplay.Content = null;

        if (Root is null)
            return;

        PART_ShellDisplay.Content = _currentShell = CreateFallbackShell(FallbackShell, Root);
        _currentIsPreferred = false;

        Shell CreateFallbackShell(AdaptiveShells adaptiveShells, IStrixDataRoot root)
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
