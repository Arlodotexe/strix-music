using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs;
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
    private readonly IFolder _releaseSourceFolder;
    [ObservableProperty] private bool _isPinned;

    /// <summary>
    /// Creates a new instance of <see cref="AppReleaseContentBundlePreloadHandler"/>.
    /// </summary>
    public AppReleaseContentBundlePreloadHandler(AppReleaseContentBundle releaseContentBundle, IpfsClient client, IFolder releaseSourceFolder)
    {
        _client = client;
        _releaseSourceFolder = releaseSourceFolder;
        ReleaseContentBundle = releaseContentBundle;
        _isPinned = releaseContentBundle.IsPinned;
    }

    /// <summary>
    /// The content bundle data pulled from the release source.
    /// </summary>
    public AppReleaseContentBundle ReleaseContentBundle { get; }

    /// <summary>
    /// Retrieves or removes the release content based on the value of <see cref="IsPinned"/>.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true, FlowExceptionsToTaskScheduler = true)]
    public async Task PinAsync(CancellationToken cancellationToken)
    {
        ReleaseContentBundle.LocallyPinnedCids ??= new Dictionary<string, List<string>>();
        Guard.IsNotNull(ReleaseContentBundle.RootRelativePaths);
        Logger.LogInformation($"Pinning app release content bundle {ReleaseContentBundle.Id} ({ReleaseContentBundle.DisplayName})");

        await ReleaseContentBundle.RootRelativePaths.InParallel(async x =>
        {
            var targetItem = await _releaseSourceFolder.GetItemByRelativePathAsync(x, cancellationToken);

            if (targetItem is IpnsFile or IpnsFolder or IpfsFile or IpfsFolder)
                await _client.DoCommandAsync("refs", cancellationToken, targetItem.Id, "recursive=true", "unique=true");
            else
                throw new System.NotSupportedException($"{targetItem.GetType()} isn't supported for release content preloading.");

            var pinnedCids = await _client.Pin.AddAsync(targetItem.Id, recursive: true, cancellationToken);

            lock (ReleaseContentBundle.LocallyPinnedCids)
            {
                if (ReleaseContentBundle.LocallyPinnedCids.TryGetValue(x, out var value))
                {
                    foreach (var item in pinnedCids)
                    {
                        if (!value.Contains(item))
                            value.Add(item);
                    }
                }
                else
                {
                    ReleaseContentBundle.LocallyPinnedCids.Add(x, new List<string>(pinnedCids.Select(cid => cid.ToString())));
                }
            }
        });

        ReleaseContentBundle.IsPinned = true;
        Logger.LogInformation($"Pinned app release content bundle {ReleaseContentBundle.Id} ({ReleaseContentBundle.DisplayName})");
    }

    /// <summary>
    /// Retrieves or removes the release content based on the value of <see cref="IsPinned"/>.
    /// </summary>
    [RelayCommand(IncludeCancelCommand = true, FlowExceptionsToTaskScheduler = true)]
    public async Task UnpinAsync(CancellationToken cancellationToken)
    {
        ReleaseContentBundle.LocallyPinnedCids ??= new Dictionary<string, List<string>>();
        Guard.IsNotNull(ReleaseContentBundle.RootRelativePaths);
        Logger.LogInformation($"Unpinning app release content bundle {ReleaseContentBundle.Id} ({ReleaseContentBundle.DisplayName})");

        await ReleaseContentBundle.RootRelativePaths.InParallel(async x =>
        {
            foreach (var item in ReleaseContentBundle.LocallyPinnedCids)
            {
                var removedCids = await item.Value.InParallel(cid => _client.Pin.RemoveAsync(cid, recursive: true, cancellationToken));

                if (!removedCids.Select(cid => cid.ToString()).SequenceEqual(item.Value))
                {
                    Logger.LogWarning("The list of unpinned CIDs didn't match the list of known ID pins. This may indicate manual clean up of data (not performed by this application), a mismatch in the data format agreed upon between the publisher and client, or a generic application bug. Inspect the logs to see if this is undesirable behavior.");
                }

                ReleaseContentBundle.LocallyPinnedCids.Remove(x);
            }
        });

        ReleaseContentBundle.IsPinned = false;
        Logger.LogInformation($"Unpinned app release content bundle {ReleaseContentBundle.Id} ({ReleaseContentBundle.DisplayName})");
    }
}
