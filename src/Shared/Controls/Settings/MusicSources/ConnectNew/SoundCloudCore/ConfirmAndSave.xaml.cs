using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.SoundCloudCore;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class ConfirmAndSave : Page
{
    private (ConnectNewMusicSourceNavigationParams, SoundCloudCoreSettings Settings) _param;
    [ObservableProperty] private ICore? _core;
    private AppRoot? _appRoot;

    /// <summary>
    /// Creates a new instance of <see cref="ConfirmAndSave"/>.
    /// </summary>
    public ConfirmAndSave()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The backing dependency property for <ses cref="Settings" />.
    /// </summary>
    public static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register(nameof(Settings), typeof(SoundCloudCoreSettings), typeof(ConfirmAndSave), new PropertyMetadata(null));

    /// <summary>
    /// Collection holding the data for <see cref="Settings" />
    /// </summary>
    public SoundCloudCoreSettings? Settings
    {
        get => (SoundCloudCoreSettings?)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    /// <inheritdoc />
    override protected async void OnNavigatedTo(NavigationEventArgs e)
    {
        var param = ((ConnectNewMusicSourceNavigationParams NavParams, SoundCloudCoreSettings Settings))e.Parameter;
        _param = param;
        _appRoot = _param.Item1.AppRoot;

        Settings = param.Settings;
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        Guard.IsNotNull(_appRoot?.MusicSourcesSettings);
        Guard.IsNotNull(Settings);
        
        await Settings.SaveAsync();
        
        _appRoot.MusicSourcesSettings.ConfiguredSoundCloudCores.Add(Settings);
        
        _param.Item1.SetupCompleteTaskCompletionSource.SetResult(null);
    }

    [RelayCommand]
    private void Cancel()
    {
        Guard.IsNotNull(_param);
        _param.Item1.SetupCompleteTaskCompletionSource.SetCanceled();
    }
}
