using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs.Http;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Storage;

namespace StrixMusic.AppModels;

/// <summary>
/// Provided a <see cref="AppReleaseContentBundle"/> and an <see cref="IpfsClient"/>, handle the caching of bundle content on the local node.
/// </summary>
public partial class AppReleaseContentBundlePreloadHandler : ObservableObject
{
    private readonly IpfsClient _client;
    private readonly IpnsFolder _releaseSourceFolder;
    [ObservableProperty] private bool _isPreloaded;
    [ObservableProperty] private bool _isPinned;

    /// <summary>
    /// Creates a new instance of <see cref="AppReleaseContentBundlePreloadHandler"/>.
    /// </summary>
    public AppReleaseContentBundlePreloadHandler(AppReleaseContentBundle releaseContentBundle, IpfsClient client, IFolder releaseSourceFolder)
    {
        _client = client;
        ReleaseContentBundle = releaseContentBundle;

        // https://github.com/Arlodotexe/OwlCore.Storage/issues/18
        // Since TraverseRelativePath is broken and we're waiting on OwlCore.Storage updates
        // We won't use the interfaces to navigate to the folder.
        // Instead, we'll manually feed the IPNS address + path into Kubo.
        _releaseSourceFolder = (IpnsFolder)releaseSourceFolder;
    }

    /// <summary>
    /// The content bundle data pulled from the release source.
    /// </summary>
    public AppReleaseContentBundle ReleaseContentBundle { get; }

    /// <summary>
    /// Preloads all content for this bundle.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true, FlowExceptionsToTaskScheduler = true)]
    public async Task PreloadAsync(CancellationToken cancellationToken)
    {
        Guard.IsNotNull(ReleaseContentBundle.RootRelativePaths);
        Logger.LogInformation($"Started preload for app release content bundle {ReleaseContentBundle.Id} ({ReleaseContentBundle.DisplayName})");

        await ReleaseContentBundle.RootRelativePaths.InParallel(async x =>
        {
            var targetItem = await _releaseSourceFolder.GetItemByRelativePathAsync(x, cancellationToken);

            await _client.DoCommandAsync("refs", cancellationToken, targetItem.Id, "recursive=true", "unique=true");
        });

        IsPreloaded = true;
        Logger.LogInformation($"Preloaded app release content bundle {ReleaseContentBundle.Id} ({ReleaseContentBundle.DisplayName})");
    }
}
