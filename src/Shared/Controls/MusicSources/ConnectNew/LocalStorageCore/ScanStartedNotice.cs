using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using StrixMusic.Settings;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.AppModels;

namespace StrixMusic.Controls.MusicSources.ConnectNew.LocalStorageCore;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class ScanStartedNotice : Page
{
    private (ConnectNewMusicSourceNavigationParams, LocalStorageCoreSettings Settings) _param;
    [ObservableProperty] private ICore? _core;
    private AppRoot? _appRoot;

    /// <summary>
    /// Creates a new instance of <see cref="ScanStartedNotice"/>.
    /// </summary>
    public ScanStartedNotice()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The backing dependency property for <ses cref="Settings" />.
    /// </summary>
    public static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register(nameof(Settings), typeof(LocalStorageCoreSettings), typeof(ScanStartedNotice), new PropertyMetadata(null));

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
        _appRoot.MusicSourcesSettings.ConfiguredLocalStorageCores.Add(Settings);

        await Settings.SaveAsync();
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
