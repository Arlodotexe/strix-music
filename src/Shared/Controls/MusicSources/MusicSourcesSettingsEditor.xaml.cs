using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls.MusicSources;

/// <summary>
/// A control to view available music source and add new music sources.
/// </summary>
[ObservableObject]
public sealed partial class MusicSourcesSettingsEditor : UserControl
{
    [ObservableProperty] private MusicSourceItem? _currentSettings;

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

    /// <summary>
    /// Begins the setup process for the provided music source.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    [RelayCommand]
    public async Task StartCoreSetupAsync(AvailableMusicSource availableMusicSource)
    {
        var settings = await availableMusicSource.DefaultSettingsFactory();
            
        CurrentSettings = new MusicSourceItem(settings, availableMusicSource)
        {
            EditingFinishedCommand = CompleteCoreSetupCommand,
        };
    }

    /// <summary>
    /// Called when the setup process 
    /// </summary>
    /// <param name="item">Editor data about the music source settings which has completed setup.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    [RelayCommand]
    public async Task CompleteCoreSetupAsync(MusicSourceItem item)
    {
        // TODO
    }
}
