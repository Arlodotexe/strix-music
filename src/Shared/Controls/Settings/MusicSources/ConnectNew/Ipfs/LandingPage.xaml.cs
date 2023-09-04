using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs;
using OwlCore.Kubo;
using OwlCore.Storage;
using StrixMusic.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.Ipfs;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class LandingPage : Page
{
    [ObservableProperty] private ConnectNewMusicSourceNavigationParams? _param;
    [ObservableProperty] private IpfsCoreSettings? _settings = null;
    private TimeSpan _ipfsMaxResponseTime = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Creates a new instance of <see cref="LandingPage"/>.
    /// </summary>
    public LandingPage()
    {
        this.InitializeComponent();
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true, AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
    private async Task TryContinueAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        Guard.IsNotNull(Settings);
        Guard.IsNotNull(Param?.AppRoot?.MusicSourcesSettings);
        Guard.IsNotNull(Settings);
        Guard.IsNotNull(Param?.AppRoot?.Ipfs?.Client);

        // Create new token we can cancel on a timer
        using var cancellationTokenSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationTokenSrc.CancelAfter(_ipfsMaxResponseTime);

        cancellationToken = cancellationTokenSrc.Token;

        try
        {
            if (!string.IsNullOrWhiteSpace(Settings.IpfsCidPath))
            {
                var cid = Settings.IpfsCidPath
                    .Replace("/ipfs/", string.Empty)
                    .Split('/')[0]
                    .Replace("ipfs://", string.Empty)
                    .Split('/')[0];

                var path = Settings.IpfsCidPath.Replace(cid, string.Empty);
                var folder = await CreateIpfsFolder(cid, path, cancellationToken);

                CompleteCoreSetup(folder);
            }
            else if (!string.IsNullOrWhiteSpace(Settings.IpnsAddress))
            {
                var folder = new IpnsFolder(Settings.IpnsAddress, Param.AppRoot.Ipfs.Client);

                var firstItem = await folder.GetItemsAsync(StorableType.All, cancellationToken).FirstOrDefaultAsync(cancellationToken);
                if (firstItem is null)
                    throw new InvalidOperationException("The folder is empty.");

                CompleteCoreSetup(folder);
            }
        }
        catch (OperationCanceledException)
        {
            throw new InvalidOperationException("Request timed out. Make sure the address is valid");
        }
    }

    private async Task<IpfsFolder> CreateIpfsFolder(Cid cid, string path, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(Param?.AppRoot?.Ipfs?.Client);
        var rootFolder = new IpfsFolder(cid, Param.AppRoot.Ipfs.Client);

        if (!string.IsNullOrWhiteSpace(path))
        {
            var targetFolder = (IpfsFolder?)await rootFolder.GetItemByRelativePathAsync(path);
            if (targetFolder is null || string.IsNullOrWhiteSpace(targetFolder.Id))
                throw new InvalidOperationException("Cannot retreive the contents from the path provided.");

            var firstItem = await targetFolder.GetItemsAsync(StorableType.All, cancellationToken).FirstOrDefaultAsync(cancellationToken);
            if (firstItem is null)
                throw new InvalidOperationException("The folder is empty.");

            return targetFolder;
        }
        else
        {
            var firstItem = await rootFolder.GetItemsAsync(StorableType.All, cancellationToken).FirstOrDefaultAsync(cancellationToken);
            if (firstItem is null)
                throw new InvalidOperationException("The folder is empty.");

            return rootFolder;
        }
    }

    private void CompleteCoreSetup(IFolder folder)
    {
        Guard.IsNotNull(Param?.AppRoot?.MusicSourcesSettings?.ConfiguredIpfsCores);
        Guard.IsNotNull(Settings);
        Settings.IpfsCidPath = folder.Id;
        Param.AppRoot.MusicSourcesSettings.ConfiguredIpfsCores.Add(Settings);
        Param.SetupCompleteTaskCompletionSource.SetResult(null);
    }

    [RelayCommand]
    private void CancelCoreSetup()
    {
        Guard.IsNotNull(Param);
        Param.SetupCompleteTaskCompletionSource.SetCanceled();
    }

    /// <inheritdoc />
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        var param = (ConnectNewMusicSourceNavigationParams)e.Parameter;
        Guard.IsNotNull(param.SelectedSourceToConnect);

        // Save in a field to access from another method
        Param = param;

        // The instance ID here is a temporary placeholder.
        // We need to log in and get a folder ID before we can create the final instance ID.
        Settings = (IpfsCoreSettings)await Param.SelectedSourceToConnect.DefaultSettingsFactory(string.Empty);
        Settings.InstanceId = Settings.Folder.Id;
    }

    private bool IsAnyValidAddress(string value, string value2) => !string.IsNullOrWhiteSpace(value)
                                                                        || !string.IsNullOrWhiteSpace(value2);

    private bool IsNull(object? obj) => obj is null;

    private bool IsNotNull(object? obj) => obj is not null;

    private bool InvertBool(bool val) => !val;

    private Visibility BoolToVisibility(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

    private Visibility InverseIsNullOrWhiteSpaceToVisibility(string val) => BoolToVisibility(!string.IsNullOrWhiteSpace(val));

    private Visibility NullToVisibility(object? val) => val == null ? Visibility.Collapsed : Visibility.Visible;
    private Visibility NullToInveerseVisibility(object? val) => val == null ? Visibility.Visible : Visibility.Collapsed;
}
