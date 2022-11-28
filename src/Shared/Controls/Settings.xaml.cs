using StrixMusic.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace StrixMusic.Controls;

/// <summary>
/// Displays the settings for the provided <see cref="AppRoot"/>.
/// </summary>
public sealed partial class Settings : UserControl
{
    /// <summary>
    /// Create a new instance of <see cref="Settings"/>.
    /// </summary>
    public Settings()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The data root for this app instance.
    /// </summary>
    public AppRoot? AppRoot
    {
        get => (AppRoot?)GetValue(AppRootProperty);
        set => SetValue(AppRootProperty, value);
    }

    /// <summary>
    /// The backing dependency property for <see cref="AppRoot"/>.
    /// </summary>
    public static readonly DependencyProperty AppRootProperty =
        DependencyProperty.Register(nameof(AppRoot), typeof(AppRoot), typeof(Settings), new PropertyMetadata(null));

    /// <summary>
    /// Returns a value that indicates if the provided case is the active one.
    /// </summary>
    /// <returns>A value that indicates if the provided case is the active one.</returns>
    public bool AreEqual(object expected, object value) => expected == value;
}
