using System;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.AppModels;

namespace StrixMusic.Controls;

public sealed partial class ShellSettingsEditor : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="ShellSettingsEditor"/>
    /// </summary>
    public ShellSettingsEditor()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The backing dependency property for <see cref="ShellSettings"/>.
    /// </summary>
    public static readonly DependencyProperty ShellSettingsProperty =
        DependencyProperty.Register(nameof(ShellSettings), typeof(int), typeof(ShellSettingsEditor), new PropertyMetadata(new ShellSettings { InstanceId = $"{Guid.NewGuid()}" }));

    /// <summary>
    /// The AppRoot to provide to displayed shells. Optional.
    /// </summary>
    public IStrixDataRoot? Root
    {
        get => (IStrixDataRoot?)GetValue(RootProperty);
        set => SetValue(RootProperty, value);
    }

    /// <summary>
    /// The backing dependency property for <see cref="Root"/>.
    /// </summary>
    public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(nameof(Root), typeof(IStrixDataRoot), typeof(ShellSettingsEditor), new PropertyMetadata(null));

    /// <summary>
    /// The settings instance to edit.
    /// </summary>
    public ShellSettings ShellSettings
    {
        get => (ShellSettings)GetValue(ShellSettingsProperty);
        set => SetValue(ShellSettingsProperty, value);
    }

    /// <summary>
    /// Inverts a boolean then converts it to Visibility.
    /// </summary>
    /// <returns>An inverted visibility representation of the bool value.</returns>
    public Visibility InverseVisibility(bool val) => BoolToVisibility(!val);

    /// <summary>
    /// Converts a boolean to Visibility.
    /// </summary>
    /// <returns>A visibility representation of the bool value.</returns>
    public Visibility BoolToVisibility(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Given a <paramref name="shell"/>, returns a boolean indicating whether the shell is fully adaptive.
    /// </summary>
    /// <returns>A boolean indicating whether the shell is fully adaptive.</returns>
    public bool IsAdaptiveShell(StrixMusicShells shell) => shell is StrixMusicShells.GrooveMusic or StrixMusicShells.Sandbox;

    /// <summary>
    /// Given a <paramref name="shell"/>, returns a boolean indicating whether the shell is fully adaptive.
    /// </summary>
    /// <returns>A boolean indicating whether the shell is fully adaptive.</returns>
    public Visibility IsAdaptiveShellToInvertedVisibility(StrixMusicShells shell) => BoolToVisibility(!IsAdaptiveShell(shell));
}
