using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs;
using Ipfs.Http;
using Newtonsoft.Json.Linq;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Storage;
using Uno.Extensions.Specialized;

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
    public AppReleaseContentBundlePreloadHandler(AppReleaseContentBundle releaseContentBundle, IpfsClient client, IpnsFolder releaseSourceFolder)
    {
        _client = client;
        _releaseSourceFolder = releaseSourceFolder;
        ReleaseContentBundle = releaseContentBundle;
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
        Guard.IsNotNull(ReleaseContentBundle.RelativePathsToRoot);

        await ReleaseContentBundle.RelativePathsToRoot.InParallel(async x =>
        {
            var targetItem = await _releaseSourceFolder.GetItemByRelativePathAsync(x, cancellationToken);

            await _client.DoCommandAsync("refs", cancellationToken, targetItem.Id, "recursive=true", "unique=true");
        });

        IsPreloaded = true;
    }
}
