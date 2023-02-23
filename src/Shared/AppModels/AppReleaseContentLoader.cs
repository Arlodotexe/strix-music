using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    private IpnsFolder _releaseSourceFolder;

    /// <summary>
    /// The folder containing release content, if available.
    /// </summary>
    public IpnsFolder ReleaseSourceFolder
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

        await availableContentBundles.InParallel(async bundle =>
        {
            var handler = new AppReleaseContentBundlePreloadHandler(bundle, _client, ReleaseSourceFolder);
            Bundles.Add(handler);

            // Skip if the user has not configured to load this bundle.
            if (!_settings.PreloadedReleaseContentBundles.Contains(bundle))
                return;

            // If the user has configured this bundle, preload it to the local node. If already preloaded, it'll finish fairly quickly and act as an integrity check.
            await handler.PreloadCommand.ExecuteAsync(cancellationToken);
        });

        IsInitialized = true;
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
