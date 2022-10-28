using System;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using OwlCore.Storage.Uwp;
using StrixMusic.Cores.Storage;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Diagnostics;
using Ipfs.Http;
using OwlCore.Kubo;

namespace StrixMusic;

/// <summary>
/// A control to display top-level app content.
/// </summary>
public sealed partial class AppFrame : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="AppFrame"/>.
    /// </summary>
    public AppFrame()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await InitAsync();
    }

    private async Task InitAsync()
    {
        // Temporary manual setup
        var folder = await PickFolderAsync();
        if (folder is null)
            return;
        /*
                var ipfsClient = new IpfsClient();
                var folder = new MfsFolder("/Music/", ipfsClient);*/

        var storageCore = await CreateStorageCoreAsync(folder);

        var cores = new[] { storageCore };

        Root = new MergedCore
        (
            id: ApplicationView.GetForCurrentView().Id.ToString(),
            cores: cores,
            new MergedCollectionConfig(MergedCollectionSorting.Ranked, cores.Select(x => x.InstanceId).ToList())
        );

        await Root.InitAsync();

        async Task<WindowsStorageFolder?> PickFolderAsync()
        {
            var picker = new FolderPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.MusicLibrary,
                FileTypeFilter = { "*" },
            };

            var pickedFolder = await picker.PickSingleFolderAsync();

            return pickedFolder is null ? null : new WindowsStorageFolder(pickedFolder);
        }

        async Task<StorageCore> CreateStorageCoreAsync(IFolder folderToScan)
        {
            var settingsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderToScan.Id.HashMD5Fast(), CreationCollisionOption.OpenIfExists);

            return new StorageCore
            (
                folderToScan,
                metadataCacheFolder: new WindowsStorageFolder(settingsFolder),
                displayName: folderToScan.GetType().Name,
                fileScanProgress: new Progress<FileScanState>(x => Logger.LogInformation($"Scan progress for {folderToScan.Id}: Stage {x.Stage}, Files Found: {x.FilesFound}: Files Scanned: {x.FilesProcessed}"))
            )
            {
                ScannerWaitBehavior = ScannerWaitBehavior.NeverWait
            };
        }
    }

    /// <summary>
    /// The root data source to use for library, search, etc.
    /// </summary>
    public IStrixDataRoot? Root
    {
        get => (IStrixDataRoot?)GetValue(RootProperty);
        set => SetValue(RootProperty, value);
    }

    /// <summary>
    /// The backing dependency property for <see cref="Root"/>.
    /// </summary>
    public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(nameof(Root), typeof(IStrixDataRoot), typeof(AppFrame), new PropertyMetadata(null));
}
