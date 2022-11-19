using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Diagnostics;
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
public class CoreFactory
{
    private readonly IModifiableFolder _dataFolder;

    /// <summary>
    /// Creates a new instance of <see cref="CoreFactory"/>.
    /// </summary>
    /// <param name="dataFolder">The folder where core data is stored.</param>
    public CoreFactory(IModifiableFolder dataFolder)
    {
        _dataFolder = dataFolder;
    }

    /// <summary>
    /// Creates a <see cref="StorageCore"/> from the provided <see cref="LocalStorageCoreSettings"/>.
    /// </summary>
    /// <param name="settings">The settings used to create the folder abstraction.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<StorageCore> CreateLocalStorageCoreAsync(LocalStorageCoreSettings settings)
    {
        var coreData = await _dataFolder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == settings.InstanceId) ??
                       await _dataFolder.CreateFolderAsync(settings.InstanceId);

        if (coreData is not IModifiableFolder modifiableCoreData)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        Guard.IsNotNullOrWhiteSpace(settings.FutureAccessToken);
        var storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(settings.FutureAccessToken);
        var folder = new WindowsStorageFolder(storageFolder);

        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/LocalFiles/Logo.svg"));
        var logo = new CoreFileImage(new WindowsStorageFile(logoFile));

        var core = new StorageCore
        (
            folder,
            modifiableCoreData,
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
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<StorageCore> CreateOneDriveCoreAsync(OneDriveCoreSettings settings)
    {
        var coreData = await _dataFolder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == settings.InstanceId) ??
                       await _dataFolder.CreateFolderAsync(settings.InstanceId);

        if (coreData is not IModifiableFolder modifiableCoreData)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        throw new NotImplementedException("TODO");
        /*        return new StorageCore(, modifiableCoreData, "OneDrive");*/
    }
}
