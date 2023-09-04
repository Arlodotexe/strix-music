using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using OwlCore.Extensions;
using OwlCore.Storage;
using OwlCore.Storage.OneDrive;
using OwlCore.Storage.Uwp;
using StrixMusic.CoreModels;
using StrixMusic.Cores.Storage;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Settings;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Logger = OwlCore.Diagnostics.Logger;
using OwlCore.Kubo;
using Ipfs.Http;

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

#if __WASM__
        var folderToScan = (WindowsStorageFolder)AppRoot.KnownFolders.First(x => settings.ConfiguredFolderId == x.Id);
#else
        var storageFolderToScan = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(settings.FutureAccessToken);
        var folderToScan = new WindowsStorageFolder(storageFolderToScan);
#endif

        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/LocalStorage/Logo.svg"));
        var logo = new CoreFileImage(new WindowsStorageFile(logoFile));

        var core = new StorageCore
        (
            folderToScan,
            metadataCacheFolder: modifiableCoreDataFolder,
            "Local Storage",
            fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Music files found: {x.FilesProcessed}")))
        {
            ScannerWaitBehavior = ScannerWaitBehavior.NeverWait,
            InstanceDescriptor = folderToScan.Path,
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
    public static async Task<StorageCore> CreateOneDriveCoreAsync(OneDriveCoreSettings settings, HttpMessageHandler messageHandler)
    {
        var instanceId = settings.InstanceId.HashMD5Fast();
        var coreData = await settings.Folder.CreateFolderAsync(instanceId, overwrite: false);

        if (coreData is not IModifiableFolder modifiableCoreDataFolder)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        Guard.IsNotNullOrWhiteSpace(settings.ClientId);
        Guard.IsNotNullOrWhiteSpace(settings.FolderId);
        Guard.IsNotNullOrWhiteSpace(settings.UserId);

        // Setup to create a graph client
        var scopes = new[] { "Files.Read.All", "User.Read", "Files.ReadWrite" };
        var authorityUri = "https://login.microsoftonline.com/consumers";

        var clientAppBuilder = PublicClientApplicationBuilder.Create(settings.ClientId).WithAuthority(authorityUri, false);

        if (!string.IsNullOrWhiteSpace(settings.RedirectUri))
            clientAppBuilder = clientAppBuilder.WithRedirectUri(settings.RedirectUri);

        var clientApp = clientAppBuilder.Build();
        Logger.LogInformation($"Public client application created. Authenticating.");

        // Authenticate
        var account = await clientApp.GetAccountAsync(settings.UserId);
        var authResult = await clientApp.AcquireTokenSilent(scopes, account).ExecuteAsync();

        var authProvider = new BaseBearerTokenAuthenticationProvider(new OneDriveAccessTokenProvider(authResult.AccessToken));

        // Create graph client
        var handlers = GraphClientFactory.CreateDefaultHandlers();
        var httpClient = GraphClientFactory.Create(handlers, finalHandler: messageHandler);
        var graphClient = new GraphServiceClient(httpClient, authProvider);

        // Get selected OneDrive folder.
        var driveItem = await graphClient.Me.Drive.GetAsync();
        Guard.IsNotNull(driveItem);

        var targetDriveItem = await graphClient.Drives[driveItem.Id].Items[settings.FolderId].GetAsync();
        Guard.IsNotNull(targetDriveItem);

        // Create storage abstraction and core.
        var folderToScan = new OneDriveFolder(graphClient, targetDriveItem);
        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/OneDrive/Logo.svg"));

        var core = new StorageCore
        (
            folderToScan,
            metadataCacheFolder: modifiableCoreDataFolder,
            "OneDrive",
            fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Music files found: {x.FilesProcessed}")))
        {
            ScannerWaitBehavior = ScannerWaitBehavior.NeverWait,
            Logo = new CoreFileImage(new WindowsStorageFile(logoFile)),
            InstanceDescriptor = $"{authResult.Account.Username}: {settings.RelativeFolderPath}",
        };

        ((CoreFileImage)core.Logo).SourceCore = core;

        return core;
    }

    /// <summary>
    /// Creates a <see cref="StorageCore"/> from the provided <see cref="IpfsCoreSettings"/>.
    /// </summary>
    /// <param name="settings">The settings used to create the folder abstraction.</param>
    /// <param name="client">The configured ipfs client.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the new core instance.</returns>
    /// <exception cref="InvalidOperationException">A new folder was created in the data folder, but it's not modifiable.</exception>
    public static async Task<ICore> CreateIpfsCoreAsync(IpfsCoreSettings settings, IpfsClient client)
    {
        var instanceId = settings.InstanceId.HashMD5Fast();

        var coreDataFolder = await settings.Folder.GetFoldersAsync().FirstOrDefaultAsync(x => x.Name == instanceId) ??
                             await settings.Folder.CreateFolderAsync(instanceId);

        if (coreDataFolder is not IModifiableFolder modifiableCoreDataFolder)
            throw new InvalidOperationException($"A new folder was created in the data folder, but it's not modifiable.");

        var folderToScan = new IpfsFolder(settings.IpfsCidPath, client);

        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/Ipfs/logo.png"));
        var logo = new CoreFileImage(new WindowsStorageFile(logoFile));

        var core = new StorageCore
        (
            folderToScan,
            metadataCacheFolder: modifiableCoreDataFolder,
            "Ipfs Storage",
            fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Files Scanned: {x.FilesProcessed}")))
        {
            ScannerWaitBehavior = ScannerWaitBehavior.NeverWait,
            InstanceDescriptor = folderToScan.Id,
            Logo = logo,
        };

        logo.SourceCore = core;
        core.Logo = logo;
        return core;
    }
}
