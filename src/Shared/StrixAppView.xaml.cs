using System;
using StrixMusic.Sdk.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic;

public sealed partial class StrixAppView : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="StrixAppView"/>.
    /// </summary>
    public StrixAppView()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Identifies the <see cref="PreferredShell"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreferredShellProperty =
        DependencyProperty.Register(nameof(PreferredShell), typeof(AllShells), typeof(StrixAppView), new PropertyMetadata(AllShells.Sandbox));

    /// <summary>
    /// Identifies the <see cref="FallbackShell"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FallbackShellProperty =
        DependencyProperty.Register(nameof(FallbackShell), typeof(AdaptiveShells), typeof(StrixAppView), new PropertyMetadata(AllShells.ZuneDesktop));

    /// <summary>
    /// Identifies the <see cref="Root"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(nameof(Root), typeof(IStrixDataRoot), typeof(StrixAppView), new PropertyMetadata(null, (d, e) => ((StrixAppView)d).OnRootChanged(e.OldValue as IStrixDataRoot, e.NewValue as IStrixDataRoot)));

    private void OnRootChanged(IStrixDataRoot? oldValue, IStrixDataRoot? newValue)
    {
        
    }

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
    public AllShells PreferredShell
    {
        get => (AllShells)GetValue(PreferredShellProperty);
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
}
