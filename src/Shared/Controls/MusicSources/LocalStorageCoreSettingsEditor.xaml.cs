using System;
using Windows.Storage.Pickers;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CommunityToolkit.Mvvm.Input;

namespace StrixMusic.Controls.MusicSources;

/// <summary>
/// Displays an instance of <see cref="LocalStorageCoreSettings"/> to be edited by the user.
/// </summary>
public sealed partial class LocalStorageCoreSettingsEditor : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="LocalStorageCoreSettingsEditor"/>.
    /// </summary>
    public LocalStorageCoreSettingsEditor()
    {
        this.InitializeComponent();

        Loaded += LocalStorageCoreSettingsEditor_Loaded;
    }

    private void LocalStorageCoreSettingsEditor_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= LocalStorageCoreSettingsEditor_Loaded;
        Unloaded += LocalStorageCoreSettingsEditor_Unloaded;
    }

    private void LocalStorageCoreSettingsEditor_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= LocalStorageCoreSettingsEditor_Unloaded;
    }

    /// <summary>
    /// The backing dependency property for <ses cref="Settings" />.
    /// </summary>
    public static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register(nameof(Settings), typeof(LocalStorageCoreSettings), typeof(LocalStorageCoreSettingsEditor), new PropertyMetadata(null));

    /// <summary>
    /// The backing dependency property for <ses cref="EditingFinishedCommand" />.
    /// </summary>
    public static readonly DependencyProperty EditingFinishedCommandProperty =
        DependencyProperty.Register(nameof(EditingFinishedCommand), typeof(IRelayCommand<MusicSourceItem>), typeof(LocalStorageCoreSettingsEditor), new PropertyMetadata(null));

    /// <summary>
    /// Collection holding the data for <see cref="Settings" />
    /// </summary>
    public LocalStorageCoreSettings? Settings
    {
        get => (LocalStorageCoreSettings?)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    /// <summary>
    /// The command to use when editing has completed.
    /// </summary>
    public IRelayCommand<MusicSourceItem>? EditingFinishedCommand
    {
        get => (IRelayCommand<MusicSourceItem>?)GetValue(EditingFinishedCommandProperty);
        set => SetValue(EditingFinishedCommandProperty, value);
    }

    private async void BrowseButton_OnClick(object sender, RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
        folderPicker.CommitButtonText = "Scan folder";

        var pickedFolder = await folderPicker.PickSingleFolderAsync();


    }
}
