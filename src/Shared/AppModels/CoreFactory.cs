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
    /// <param name="storageCoreCacheContainingFolder">The folder where core metadata is saved.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<StorageCore> CreateLocalStorageCoreAsync(LocalStorageCoreSettings settings, IModifiableFolder storageCoreCacheContainingFolder)
    {
        var coreCache = await storageCoreCacheContainingFolder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == settings.InstanceId.HashMD5Fast())
                     ?? await storageCoreCacheContainingFolder.CreateFolderAsync(settings.InstanceId.HashMD5Fast());

        if (coreCache is not IModifiableFolder modifiableCoreCache)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        Guard.IsNotNullOrWhiteSpace(settings.FutureAccessToken);
        var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(settings.FutureAccessToken);
        var folder = new WindowsStorageFolder(storageFolder);

        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/LocalStorage/Logo.svg"));
        var logo = new CoreFileImage(new WindowsStorageFile(logoFile));

        var core = new StorageCore
        (
            folder,
            metadataCacheFolder: modifiableCoreCache,
            "Local Storage",
            fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folder.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Files Scanned: {x.FilesProcessed}")))
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
    /// <param name="storageCoreCacheContainingFolder"></param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<StorageCore> CreateOneDriveCoreAsync(OneDriveCoreSettings settings, IModifiableFolder storageCoreCacheContainingFolder)
    {
        var coreData = await storageCoreCacheContainingFolder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == settings.InstanceId.HashMD5Fast())
                    ?? await storageCoreCacheContainingFolder.CreateFolderAsync(settings.InstanceId.HashMD5Fast());

        if (coreData is not IModifiableFolder modifiableCoreData)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        throw new NotImplementedException("TODO");
        /*        return new StorageCore(, modifiableCoreData, "OneDrive");*/
    }
}
