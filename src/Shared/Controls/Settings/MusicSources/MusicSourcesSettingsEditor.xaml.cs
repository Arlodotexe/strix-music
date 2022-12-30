using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.AppModels;
using OwlCore.Storage;

namespace StrixMusic.Controls.Settings.MusicSources;

/// <summary>
/// A control to view available music source and add new music sources.
/// </summary>
[ObservableObject]
public sealed partial class MusicSourcesSettingsEditor : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="MusicSourcesSettingsEditor"/>.
    /// </summary>
    public MusicSourcesSettingsEditor()
    {
        InitializeComponent();
    }

    /// <summary>
    /// The backing dependency property for <see cref="AppRoot"/>.
    /// </summary>
    public static readonly DependencyProperty AppRootProperty =
        DependencyProperty.Register(nameof(AppRoot), typeof(AppRoot), typeof(MusicSourcesSettingsEditor), new PropertyMetadata(null));

    /// <summary>
    /// The configured app instance for displaying configured cores.
    /// </summary>
    public AppRoot? AppRoot
    {
        get => (AppRoot?)GetValue(AppRootProperty);
        set => SetValue(AppRootProperty, value);
    }

    [RelayCommand]
    private async Task AddNewMusicSourceAsync()
    {
        var param = new ConnectNew.ConnectNewMusicSourceNavigationParams()
        {
            AppRoot = AppRoot,
        };

        ConnectNewSourceFrame.Visibility = Visibility.Visible;

        ConnectNewSourceFrame.Navigate(typeof(ConnectNew.ConnectNewMusicSource), param);

        try
        {
            await param.SetupCompleteTaskCompletionSource.Task;
        }
        catch (OperationCanceledException _)
        {
            // Ignored
        }

        ConnectNewSourceFrame.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Converts the input to the object's Type.
    /// </summary>
    /// <returns>The Type of the given object.</returns>
    public Type ObjectToType(object value) => value.GetType();

    private Visibility IsZeroToVisibility(int arg) => arg == 0 ? Visibility.Visible : Visibility.Collapsed;

    private Visibility IsNotZeroToVisibility(int arg) => arg != 0 ? Visibility.Visible : Visibility.Collapsed;

    private IFolder? AsFolder(object obj) => obj as IFolder;

    private bool IsFolder(object obj) => obj is IFolder;

    private bool And(bool val1, bool val2) => val1 && val2;

    private bool AndNot(bool val1, bool val2) => !(val1 && val2);

    private bool AllNotNullOrWhiteSpace(string value, string value2) => !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2);

    private bool IsNull(object? obj) => obj is null;

    private bool IsNotNull(object? obj) => obj is not null;

    private Visibility IsNullToVisibility(object? obj) => obj is null ? Visibility.Visible : Visibility.Collapsed;

    private Visibility IsNotNullToVisibility(object? obj) => obj is not null ? Visibility.Visible : Visibility.Collapsed;

    private bool InvertBool(bool val) => !val;

    private Visibility BoolToVisibility(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

    private Visibility InvertBoolToVisibility(bool val) => !val ? Visibility.Visible : Visibility.Collapsed;

    private Uri StringToUri(string value) => new Uri(value);
}
