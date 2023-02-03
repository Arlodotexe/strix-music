using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs;
using Ipfs.Http;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Storage;
using OwlCore.Storage.SystemIO;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static System.Environment;

namespace StrixMusic.AppModels;

/// <summary>
/// Enables managed access to IPFS.
/// </summary>
public partial class IpfsAccess : ObservableObject, IAsyncInit
{
    private readonly SemaphoreSlim _initMutex = new(1, 1);

    private IpfsSettings _settings;
    private bool _isInitialized;
    private bool _isRunningEmbeddedNode;
    private string _initStatus = "IPFS is not loaded";
    private KuboBootstrapper? _kuboBootstrapper;
    private PeerRoom? _everyone;
    private IpfsClient? _client;
    private Ipfs.Peer? _thisPeer;

    /// <summary>
    /// Creates a new instance of <see cref="IpfsAccess"/>.
    /// </summary>
    /// <param name="settings"></param>
    public IpfsAccess(IpfsSettings settings)
    {
        _settings = settings;

        _settings.PropertyChanged += SettingsOnPropertyChanged;
    }

    /// <summary>
    /// A container for all settings related to Ipfs.
    /// </summary>
    public IpfsSettings Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    /// <summary>
    /// Gets a boolean that indicates if IPFS can be accessed.
    /// </summary>
    public bool IsInitialized
    {
        get => _isInitialized;
        set => SetProperty(ref _isInitialized, value);
    }

    /// <summary>
    /// Gets a boolean that indicates if a node was downloaded and boostrapped by the application.
    /// </summary>
    public bool IsRunningEmbeddedNode
    {
        get => _isRunningEmbeddedNode;
        set => SetProperty(ref _isRunningEmbeddedNode, value);
    }

    /// <summary>
    /// Detailed status info for <see cref="InitAsync"/>. 
    /// </summary>
    public string InitStatus
    {
        get => _initStatus;
        set => SetProperty(ref _initStatus, value);
    }

    /// <summary>
    /// The bootstrapper that was used to start an embedded node.
    /// </summary>
    public KuboBootstrapper? KuboBootstrapper
    {
        get => _kuboBootstrapper;
        set => SetProperty(ref _kuboBootstrapper, value);
    }

    /// <summary>
    /// A room where all users of the application are present.
    /// </summary>
    public PeerRoom? Everyone
    {
        get => _everyone;
        set => SetProperty(ref _everyone, value);
    }

    /// <summary>
    /// The configured IPFS client, if available.
    /// </summary>
    public IpfsClient? Client
    {
        get => _client;
        set => SetProperty(ref _client, value);
    }

    /// <summary>
    /// The peer information for this user, if the daemon is running and accessible.
    /// </summary>
    public Ipfs.Peer? ThisPeer
    {
        get => _thisPeer;
        set => SetProperty(ref _thisPeer, value);
    }

    private async void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_settings.Enabled))
        {
            // Shut down the node if IPFS is turned off.
            if (IsInitialized && !_settings.Enabled)
                await StopCommand.ExecuteAsync(null);
        }
    }

    /// <summary>
    /// The message handler to use when accessing IPFS over any HTTP API.
    /// </summary>
    public HttpMessageHandler MessageHandler { get; set; } = new HttpClientHandler();

    /// <summary>
    /// Stops any running embedded IPFS nodes, removes the current peer, and disables access to IPFS.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Everyone?.Dispose();
        Everyone = null;

        if (KuboBootstrapper is not null)
        {
            KuboBootstrapper.Stop();
            KuboBootstrapper.Dispose();
        }

        Client = null;
        ThisPeer = null;

        IsInitialized = false;

        if (IsRunningEmbeddedNode)
        {
            InitStatus = "The embedded IPFS node has been stopped.";
            IsRunningEmbeddedNode = false;
        }
        else
        {
            InitStatus = "Access to the local node has been disabled.";
        }
    }

    /// <inheritdoc />
    [RelayCommand(FlowExceptionsToTaskScheduler = true, IncludeCancelCommand = true)]
    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        await Settings.LoadCommand.ExecuteAsync(null);

        if (!Settings.Enabled || IsInitialized)
            return;

        using (await _initMutex.DisposableWaitAsync(cancellationToken))
        {
            if (IsInitialized)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            InitStatus = "Checking if IPFS is already running";
            Client = await ScanLocalhostForRunningKuboCompliantRpcApi(cancellationToken);

            if (Client is null)
            {
#if __WASM__
                InitStatus = "No local Kubo daemon was found. IPFS cannot be initialized.";
                return;
#else
InitStatus = "No local Kubo daemon was found. Downloading Kubo for bootstrapping...";
#endif
                var kuboBin = await GetDownloadedKuboBinaryAsync(cancellationToken);
                if (kuboBin is null)
                {
                    InitStatus = "Downloading and extracting latest version of Kubo";
                    var downloader = new KuboDownloader { HttpMessageHandler = MessageHandler };
                    var downloadedBinary = await downloader.DownloadLatestBinaryAsync(cancellationToken);

                    kuboBin = await StoreDownloadedKuboBinaryAsync(downloadedBinary, cancellationToken);
                }

                // Starting a Kubo binary for the first time will prompt Windows show a firewall exception dialog to the user.
                // Warn the user of this and guide them through it.
                if (!Settings.FirewallWarningDisplayed)
                {
                    InitStatus = "Showing Kubo firewall warning";
                    Settings.FirewallWarningDisplayed = true;

                    await new ContentDialog
                    {
                        Title = "Add IPFS to Windows Firewall",
                        Content = new StackPanel
                        {
                            Spacing = 20,
                            Children =
                            {
                                new StackPanel
                                {
                                    Spacing = 7,
                                    Children =
                                    {
                                        new TextBlock { Text = "To enable IPFS functionality, we'll need to bootstrap Kubo.", TextWrapping = TextWrapping.WrapWholeWords },
                                        new TextBlock { Text = "Windows may prompt you to add Kubo as a firewall exception.", TextWrapping = TextWrapping.WrapWholeWords },
                                    }
                                },
                                new StackPanel
                                {
                                    Spacing = 7,
                                    Children =
                                    {
                                        new TextBlock { Text = "For the best experience it's recommended to allow Kubo on both public and private networks.", TextWrapping = TextWrapping.WrapWholeWords },
                                        new TextBlock { Text = "This will not effect your existing communication preferences.", TextWrapping = TextWrapping.WrapWholeWords },
                                    }
                                },
                            },
                        },
                        CloseButtonText = "Continue"
                    }.ShowAsync(ShowType.Interrupt);

                    await Settings.SaveAsync(cancellationToken);
                }

                InitStatus = "Starting Kubo";
                Guard.IsNotNull(kuboBin);

                var kuboBinParentFolder = await kuboBin.GetParentAsync(cancellationToken);
                Guard.IsNotNull(kuboBinParentFolder);

                var ipfsFolder = await ((IModifiableFolder)kuboBinParentFolder).CreateFolderAsync(".ipfs", overwrite: false, cancellationToken);

                KuboBootstrapper = new KuboBootstrapper(kuboBin, ipfsFolder.Path)
                {
                    ApiUri = new Uri($"http://127.0.0.1:{Settings.NodeApiPort}"),
                };

                await KuboBootstrapper.StartAsync(cancellationToken);

                Client = new IpfsClient($"{KuboBootstrapper.ApiUri}");
                ThisPeer = await Client.IdAsync(cancel: cancellationToken);
                IsRunningEmbeddedNode = true;
            }

            Guard.IsNotNull(Client);
            Guard.IsNotNull(ThisPeer);

            Everyone = new PeerRoom(ThisPeer, Client.PubSub, "StrixMusicAppEveryoneRoom");

            InitStatus = "Kubo is running and ready to use";
            IsInitialized = true;
        }
    }

    private void OnInitStatusChanged(string? value)
    {
        if (value is not null)
            OwlCore.Diagnostics.Logger.LogInformation(value);
    }

    /// <summary>
    /// Scans localhost for a running Kubo RPC API.
    /// </summary>
    public async Task<IpfsClient?> ScanLocalhostForRunningKuboCompliantRpcApi(CancellationToken cancellationToken)
    {
        var innerCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var mutex = new SemaphoreSlim(20, 20);
        var checkedPorts = new HashSet<int>();

        // Scan default embedded node value
        await ScanPortAsync(Settings.NodeApiPort);

        // Scan default Kubo value
        await ScanPortAsync(5001);

        async Task ScanPortAsync(int port)
        {
            if (ThisPeer is not null || Client is not null || innerCancellationTokenSource.IsCancellationRequested)
                return;

            using (await mutex.DisposableWaitAsync(cancellationToken: innerCancellationTokenSource.Token))
            {
                if (ThisPeer is not null || Client is not null || innerCancellationTokenSource.IsCancellationRequested)
                    return;

                // Never scan the same port twice.
                if (!checkedPorts.Add(port))
                    return;

                var url = $"http://127.0.0.1:{port}";
                Logger.LogInformation($"Scanning {url} for a Kubo Compliant RPC API");

                var client = new IpfsClient(url);

                try
                {
                    ThisPeer = await client.IdAsync(cancel: cancellationToken);
                    Client = client;

                    innerCancellationTokenSource.Cancel();
                }
                catch
                {
                    // ignored
                }
            }
        }

        return Client;
    }

    /// <summary>
    /// Stores the provided Kubo <paramref name="binaryFile"/> in a place that can be quickly retrieved later.
    /// </summary>
    /// <param name="binaryFile">The Kubo binary to store.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    public async Task<IAddressableFile> StoreDownloadedKuboBinaryAsync(IFile binaryFile, CancellationToken cancellationToken)
    {
        var appDataFolder = new SystemFolder(Environment.GetFolderPath(SpecialFolder.LocalApplicationData));
        var binariesFolder = (SystemFolder)await appDataFolder.CreateFolderAsync("bin", overwrite: false, cancellationToken);

        Settings.DownloadKuboBinaryFileId = binaryFile.Id;
        return await binariesFolder.CreateCopyOfAsync(binaryFile, overwrite: true, cancellationToken);
    }

    /// <summary>
    /// Retrieves the latest kubo binary from the place where <see cref="StoreDownloadedKuboBinaryAsync"/> has stored it.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A Task that represents the asynchronous operation. The value is the downloaded file, if found.</returns>
    public async Task<IAddressableFile?> GetDownloadedKuboBinaryAsync(CancellationToken cancellationToken)
    {
        var appDataFolder = new SystemFolder(Environment.GetFolderPath(SpecialFolder.LocalApplicationData));
        var binariesFolder = (SystemFolder)await appDataFolder.CreateFolderAsync("bin", overwrite: false, cancellationToken);

        if (!string.IsNullOrWhiteSpace(Settings.DownloadKuboBinaryFileId))
        {
            var item = await binariesFolder.GetItemAsync(Settings.DownloadKuboBinaryFileId, cancellationToken);
            return item as IAddressableFile;
        }

        return null;
    }
}
