using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using OwlCore.Storage.Uwp;
using StrixMusic.CoreModels;
using StrixMusic.Cores.Storage;
using StrixMusic.Settings;
using Windows.Storage;
using Windows.Storage.AccessCache;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.AppModels;

/// <summary>
/// Creates cores in the provided folder.
/// </summary>
public static class CoreFactory
{
    /// <summary>
    /// Creates a <see cref="StorageCore"/> from the provided <see cref="LocalStorageCoreSettings"/>.
    /// </summary>
    /// <param name="settings">The settings used to create the folder abstraction.</param>
    /// <param name="cacheFolder">The folder where scanned file metadata is stored for fast retrieval later.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException">A new folder was created in the data folder, but it's not modifiable.</exception>
    public static async Task<ICore> CreateLocalStorageCoreAsync(LocalStorageCoreSettings settings)
    {
        var instanceId = settings.InstanceId.HashMD5Fast();

        var coreDataFolder = await settings.Folder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == instanceId) ??
                       await settings.Folder.CreateFolderAsync(instanceId);

        if (coreDataFolder is not IModifiableFolder modifiableCoreDataFolder)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        Guard.IsNotNullOrWhiteSpace(settings.FutureAccessToken);
        var storageFolderToScan = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(settings.FutureAccessToken);
        var folderToScan = new WindowsStorageFolder(storageFolderToScan);

        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/LocalStorage/Logo.svg"));
        var logo = new CoreFileImage(new WindowsStorageFile(logoFile));

        var core = new StorageCore
        (
            folderToScan,
            metadataCacheFolder: modifiableCoreDataFolder,
            "Local Storage",
            fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Files Scanned: {x.FilesProcessed}")))
        {
            ScannerWaitBehavior = ScannerWaitBehavior.NeverWait,
            Logo = logo,
        };

        logo.SourceCore = core;
        core.Logo = logo;
        return core;
    }

    /// <summary>
    /// Creates a <see cref="StorageCore"/> from the provided <see cref="OneDriveCoreSettings"/>.
    /// </summary>
    /// <param name="settings">The settings used to create the folder abstraction.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException">A new folder was created in the data folder, but it's not modifiable.</exception>
    public static async Task<StorageCore> CreateOneDriveCoreAsync(OneDriveCoreSettings settings)
    {
        var instanceId = settings.InstanceId.HashMD5Fast();

        var coreData = await settings.Folder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == instanceId) ??
                       await settings.Folder.CreateFolderAsync(instanceId);

        if (coreData is not IModifiableFolder modifiableCoreData)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        throw new NotImplementedException("TODO");
        /*        return new StorageCore(, modifiableCoreData, "OneDrive");*/
    }
}
