﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using OwlCore.Extensions;
using OwlCore.Storage.Uwp;
using StrixMusic.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Settings;
using CommunityToolkit.Mvvm.Input;

namespace StrixMusic.Controls.MusicSources.ConnectNew.LocalStorageCore;

/// <summary>
/// Displays an instance of <see cref="LocalStorageCoreSettings"/> to be edited by the user.
/// </summary>
[ObservableObject]
public sealed partial class LocalStorageCoreSettingsEditor : Page
{
    private ConnectNewMusicSourceNavigationParams? _param;

    /// <summary>
    /// Creates a new instance of <see cref="LocalStorageCoreSettingsEditor"/>.
    /// </summary>
    public LocalStorageCoreSettingsEditor()
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
                Content = new TextBlock() { Text = "The selected folder has already been set up. Please choose another one." },
                PrimaryButtonText = "Ok",
            };

            await cd.ShowAsync();
            return;
        }

        var token = pickedFolder.Path.HashMD5Fast();
        var instanceId = new WindowsStorageFolder(pickedFolder).Id; // Same way a StorageCore gets the InstanceId.

        // Make it accessible across restarts.
        StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, pickedFolder);

        // Configure settings
        var defaultSettings = await _param.SelectedSourceToConnect.DefaultSettingsFactory(instanceId);
        var settings = (LocalStorageCoreSettings)defaultSettings;

        settings.InstanceId = instanceId;
        settings.FutureAccessToken = token;
        settings.Path = pickedFolder.Path;

        Frame.Navigate(typeof(ScanStartedNotice), (_param, settings));
    }

    [RelayCommand]
    private void Cancel()
    {
        Guard.IsNotNull(_param);
        _param.SetupCompleteTaskCompletionSource.SetCanceled();
    }
}
