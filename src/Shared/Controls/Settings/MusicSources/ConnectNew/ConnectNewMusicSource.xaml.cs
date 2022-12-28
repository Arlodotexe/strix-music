using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.AppModels;
using StrixMusic.Settings;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew;

/// <summary>
/// A page should be hosted in a frame. Loading this page will initiate the setup process for connecting a new music source.
/// </summary>
[ObservableObject]
public sealed partial class ConnectNewMusicSource : Page
{
    private ConnectNewMusicSourceNavigationParams? _param;

    /// <summary>
    /// Creates a new instance of <see cref="ConnectNewMusicSource" />.
    /// </summary>
    public ConnectNewMusicSource()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// The backing dependency property for <see cref="AppRoot"/>.
    /// </summary>
    public static readonly DependencyProperty MusicSourcesSettingsProperty =
        DependencyProperty.Register(nameof(MusicSourcesSettings), typeof(MusicSourcesSettings), typeof(ConnectNewMusicSource), new PropertyMetadata(null));

    /// <summary>
    /// The instance of <see cref="MusicSourcesSettings"/> where the newly connected source will be saved.
    /// </summary>
    public MusicSourcesSettings? MusicSourcesSettings
    {
        get => (MusicSourcesSettings?)GetValue(MusicSourcesSettingsProperty);
        set => SetValue(MusicSourcesSettingsProperty, value);
    }

    /// <inheritdoc />
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Guard.IsNotNull(e.Parameter);
        _param = (ConnectNewMusicSourceNavigationParams)e.Parameter;

        MusicSourcesSettings = _param.AppRoot?.MusicSourcesSettings;
        base.OnNavigatedTo(e);
    }

    [RelayCommand]
    private void SelectMusicSource(AvailableMusicSource source)
    {
        Guard.IsNotNull(_param);

        _param.SelectedSourceToConnect = source;

        var pageType = source.Name switch
        {
            "Local Storage" => typeof(MusicSources.ConnectNew.LocalStorageCore.LandingPage),
            "OneDrive" => typeof(MusicSources.ConnectNew.OneDriveCore.LandingPage),
            _ => throw new ArgumentOutOfRangeException(),
        };

        Frame.Navigate(pageType, _param);
    }
}
