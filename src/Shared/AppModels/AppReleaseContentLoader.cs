using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ipfs.Http;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Kubo;
using OwlCore.Storage;
using StrixMusic.Settings;

namespace StrixMusic.AppModels;

/// <summary>
/// Manages loading content from the original release source.
/// </summary>
public partial class AppReleaseContentLoader : ObservableObject, IAsyncInit
{
    private readonly IpfsSettings _settings;
    private readonly IpfsClient _client;
    private IFolder _releaseSourceFolder;

    /// <summary>
    /// The folder containing release content, if available.
    /// </summary>
    public IFolder ReleaseSourceFolder
    {
        get => _releaseSourceFolder;
        set => SetProperty(ref _releaseSourceFolder, value);
    }

    /// <summary>
    /// The configured release bundle handlers created from the data in <see cref="ReleaseSourceFolder"/>.
    /// </summary>
    public ObservableCollection<AppReleaseContentBundlePreloadHandler> Bundles { get; } = new();

    /// <summary>
    /// Creates a new instance of <see cref="AppReleaseContentLoader"/>
    /// </summary>
    public AppReleaseContentLoader(IpfsSettings settings, IpfsClient client, IpnsFolder releaseSourceFolder)
    {
        _settings = settings;
        _client = client;
        _releaseSourceFolder = releaseSourceFolder;
    }

    /// <inheritdoc />
    [RelayCommand(IncludeCancelCommand = true)]
    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return;

        // Get bundles
        var availableContentBundles = await GetAppReleaseContentBundlesAsync(cancellationToken);
        Logger.LogInformation($"Retrieved {availableContentBundles.Count} content bundles from publisher.");

        // Check for bundles that were removed
        var removedBundles = _settings.ReleaseContentBundles.Except(availableContentBundles).ToList();
        foreach (var removedBundle in removedBundles)
        {
            if (removedBundle.LocallyPinnedCids is null)
                continue;

            await removedBundle.LocallyPinnedCids.InParallel(path => path.Value.InParallel(cid => _client.Pin.RemoveAsync(cid, recursive: true, cancellationToken)));
        }

        // Setup available content bundles.
        await availableContentBundles.InParallel(async bundle =>
        {
            // Restore saved bundle, if present. The client stores custom properties on these objects that the publisher doesn't provide.
            var savedBundle = _settings.ReleaseContentBundles.FirstOrDefault(x => x.Id == bundle.Id);

            // Update only the vanity details and content paths from publisher.
            if (savedBundle is not null)
            {
                // Check for changed paths
                if (bundle.RootRelativePaths is not null && savedBundle.RootRelativePaths is not null &&
                    !savedBundle.RootRelativePaths.SequenceEqual(bundle.RootRelativePaths))
                {
                    // Pinned content that is no longer included should be cleaned up.
                    var orphanedPaths = savedBundle.RootRelativePaths.Except(bundle.RootRelativePaths).ToList();
                    Logger.LogInformation($"Found {orphanedPaths.Count} paths that are no longer needed for the release bundle ID {bundle.Id}. Cleaning up.");

                    // When we pin the content, we store the CIDs that were pinned for each path.
                    // Find the CIDs that were pinned for the removed paths, then unpin them.
                    foreach (var path in orphanedPaths)
                    {
                        Guard.IsNotNull(savedBundle.LocallyPinnedCids);
                        var pinnedContent = savedBundle.LocallyPinnedCids[path];
                        await pinnedContent.InParallel(cid => _client.Pin.RemoveAsync(cid, recursive: true, cancellationToken));
                    }
                }

                savedBundle.DisplayName = bundle.DisplayName;
                savedBundle.Description = bundle.Description;
                savedBundle.RootRelativePaths = bundle.RootRelativePaths;
            }

            var handler = new AppReleaseContentBundlePreloadHandler(savedBundle ?? bundle, _client, ReleaseSourceFolder);
            handler.PropertyChanged += HandlerOnPropertyChanged;

            if (_settings.ReleaseContentBundles.All(x => x.Id != handler.ReleaseContentBundle.Id))
                _settings.ReleaseContentBundles.Add(handler.ReleaseContentBundle);

            Bundles.Add(handler);

            if (handler.IsPinned)
            {
                Logger.LogInformation($"{handler.ReleaseContentBundle.Id} was previously pinned by the user.");
                await handler.PinCommand.ExecuteAsync(cancellationToken);
                await _settings.SaveCommand.ExecuteAsync(cancellationToken);
            }
        });

        await _settings.SaveAsync(cancellationToken);

        IsInitialized = true;
        Logger.LogInformation("Initialized");
    }

    private async void HandlerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppReleaseContentBundlePreloadHandler.IsPinned))
        {
            var handler = (AppReleaseContentBundlePreloadHandler)sender;

            var cancellationToken = CancellationToken.None;

            if (handler.IsPinned)
                await handler.PinCommand.ExecuteAsync(cancellationToken);
            else
                await handler.UnpinCommand.ExecuteAsync(cancellationToken);

            var existingValue = _settings.ReleaseContentBundles.First(x => x.Id == handler.ReleaseContentBundle.Id);
            existingValue.LocallyPinnedCids = handler.ReleaseContentBundle.LocallyPinnedCids;
            existingValue.IsPinned = handler.ReleaseContentBundle.IsPinned;

            await _settings.SaveCommand.ExecuteAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Gets release content bundle data from the release source folder.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The release content bundles configured by the publisher.</returns>
    public async Task<List<AppReleaseContentBundle>> GetAppReleaseContentBundlesAsync(CancellationToken cancellationToken)
    {
        var content = await _releaseSourceFolder.GetFirstByNameAsync("release-content-bundles.json", cancellationToken);
        var file = (IFile)content;

        using var stream = await file.OpenStreamAsync(cancellationToken: cancellationToken);
        return await AppSettingsSerializer.Singleton.DeserializeAsync<List<AppReleaseContentBundle>>(stream, cancellationToken);
    }

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }
}
