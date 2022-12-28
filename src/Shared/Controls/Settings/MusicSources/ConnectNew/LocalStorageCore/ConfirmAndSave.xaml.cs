using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Settings;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.LocalStorageCore;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class ConfirmAndSave : Page
{
    private (ConnectNewMusicSourceNavigationParams, LocalStorageCoreSettings Settings) _param;
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
        DependencyProperty.Register(nameof(Settings), typeof(LocalStorageCoreSettings), typeof(ConfirmAndSave), new PropertyMetadata(null));

    /// <summary>
    /// Collection holding the data for <see cref="Settings" />
    /// </summary>
    public LocalStorageCoreSettings? Settings
    {
        get => (LocalStorageCoreSettings?)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    /// <inheritdoc />
    override protected async void OnNavigatedTo(NavigationEventArgs e)
    {
        var param = ((ConnectNewMusicSourceNavigationParams NavParams, LocalStorageCoreSettings Settings))e.Parameter;
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
        
        _appRoot.MusicSourcesSettings.ConfiguredLocalStorageCores.Add(Settings);
        await _appRoot.MusicSourcesSettings.SaveAsync();
        
        _param.Item1.SetupCompleteTaskCompletionSource.SetResult(null);
    }

    [RelayCommand]
    private void Cancel()
    {
        Guard.IsNotNull(_param);
        _param.Item1.SetupCompleteTaskCompletionSource.SetCanceled();
    }
}
