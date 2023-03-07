using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Extensions;
using OwlCore.Storage.Uwp;
using StrixMusic.Helpers;
using StrixMusic.Settings;
using StrixMusic.AppModels;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.LocalStorageCore;

/// <summary>
/// Displays UI to pick a folder, then navigate to the next "connect new" setup page for Local Storage once selected.
/// </summary>
[ObservableObject]
public sealed partial class LandingPage : Page
{
    private ConnectNewMusicSourceNavigationParams? _param;

    /// <summary>
    /// Creates a new instance of <see cref="LandingPage"/>.
    /// </summary>
    public LandingPage()
    {
        this.InitializeComponent();
    }

    private static Task<StorageFolder?> PickFolder()
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
        folderPicker.CommitButtonText = "Scan folder";

        return folderPicker.PickSingleFolderAsync().AsTask();
    }

    /// <inheritdoc />
    override protected void OnNavigatedTo(NavigationEventArgs e)
    {
        _param = (ConnectNewMusicSourceNavigationParams)e.Parameter;
        Guard.IsNotNull(_param.SelectedSourceToConnect);
    }

    private async void BrowseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Guard.IsNotNull(_param?.AppRoot?.MusicSourcesSettings);
        Guard.IsNotNull(_param.SelectedSourceToConnect);

        var pickedFolder = await PickFolder();
        if (pickedFolder == null)
            return;

        // Check if folder is already set up.
        if (_param.AppRoot.StrixDataRoot?.Sources.Any(x => x.InstanceId == pickedFolder.Path) ?? false)
        {
            var cd = new ContentDialog
            {
                Title = "Folder already connected",
                Content = new TextBlock { Text = "The selected folder has already been set up. Please choose another one." },
                PrimaryButtonText = "Ok",
            };

            await cd.ShowAsync(ShowType.Interrupt);
            return;
        }

        var folder = new WindowsStorageFolder(pickedFolder);
        var token = folder.Id.HashMD5Fast();
        var instanceId = folder.Id; // Same way a StorageCore gets the InstanceId.

#if __WASM__
        // FutureAccessList isn't implemented in Uno. The folder must be tracked manually.
        AppRoot.KnownFolders.Add(folder);
#else
        // Make it accessible across restarts.
        StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, pickedFolder);
#endif

        // Configure settings
        var defaultSettings = await _param.SelectedSourceToConnect.DefaultSettingsFactory(instanceId);
        var settings = (LocalStorageCoreSettings)defaultSettings;

        settings.InstanceId = instanceId;
        settings.FutureAccessToken = token;
        settings.ConfiguredFolderId = folder.Id;

        Frame.Navigate(typeof(ConfirmAndSave), (_param, settings));
    }

    [RelayCommand]
    private void Cancel()
    {
        Guard.IsNotNull(_param);
        _param.SetupCompleteTaskCompletionSource.SetCanceled();
    }
}
