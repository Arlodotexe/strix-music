using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using OwlCore.Storage.Uwp;
using StrixMusic.CoreModels;
using StrixMusic.Cores.Storage;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Settings;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Controls;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using OwlCore.Storage.OneDrive;
using Logger = OwlCore.Diagnostics.Logger;

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
    public static async Task<StorageCore> CreateOneDriveCoreAsync(OneDriveCoreSettings settings, HttpMessageHandler messageHandler)
    {
        Logger.LogInformation($"Creating new {nameof(StorageCore)} using a {nameof(OneDriveFolder)}.");

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

        // Check for cached token
        var clientAppBuilder = PublicClientApplicationBuilder.Create(settings.ClientId).WithAuthority(authorityUri, false);

        if (!string.IsNullOrWhiteSpace(settings.RedirectUri))
            clientAppBuilder = clientAppBuilder.WithRedirectUri(settings.RedirectUri);

        var clientApp = clientAppBuilder.Build();
        Logger.LogInformation($"Public client application created. Authenticating.");

        // Authenticate
        var account = await clientApp.GetAccountAsync(settings.UserId);
        var authResult = await clientApp.AcquireTokenSilent(scopes, account).ExecuteAsync();

        var authProvider = new DelegateAuthenticationProvider(requestMessage =>
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", authResult.AccessToken);
            return Task.CompletedTask;
        });

        // Create graph client
        var handlers = GraphClientFactory.CreateDefaultHandlers(authProvider);
        var httpClient = GraphClientFactory.Create(handlers, finalHandler: messageHandler);
        var graphClient = new GraphServiceClient(httpClient);

        // Get selected OneDrive folder.
        var driveItem = await graphClient.Drive.Items[settings.FolderId].Request().GetAsync();

        // Create storage abstraction and core.
        var folderToScan = new OneDriveFolder(graphClient, driveItem);
        var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Cores/OneDrive/Logo.svg"));

        var core = new StorageCore
        (
            folderToScan,
            metadataCacheFolder: modifiableCoreDataFolder,
            "Local Storage",
            fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Files Scanned: {x.FilesProcessed}")))
        {
            ScannerWaitBehavior = ScannerWaitBehavior.NeverWait,
            Logo = new CoreFileImage(new WindowsStorageFile(logoFile)),
            InstanceDescriptor = authResult.Account.Username,
        };

        ((CoreFileImage)core.Logo).SourceCore = core;

        return core;
    }
}
