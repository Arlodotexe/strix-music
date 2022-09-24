﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Cores.Storage.FileMetadata;
using StrixMusic.Cores.Storage.FileMetadata.Scanners;
using StrixMusic.Cores.Storage.Models;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Cores.Storage;

/// <summary>
/// A common base class for all cores that handle scanning any kind of file system for audio files.
/// </summary>
public class StorageCore : ICore
{
    private readonly IModifiableFolder _metadataCacheFolder;
    private readonly SemaphoreSlim _initMutex = new(1, 1);
    private CoreState _coreState;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageCore"/> class.
    /// </summary>
    /// <param name="folder">The folder being scanned for music.</param>
    /// <param name="metadataCacheFolder">A folder where metadata can be stored</param>
    /// <param name="displayName">A user-friendly display name to use for this storage core.</param>
    /// <param name="fileScanProgress">Monitor the progress of a file scan.</param>
    protected StorageCore(IFolder folder, IModifiableFolder metadataCacheFolder, string displayName, Progress<FileScanState>? fileScanProgress = null)
    {
        _metadataCacheFolder = metadataCacheFolder;
        FileScanProgress = fileScanProgress;
        FolderScanner = new DepthFirstFolderScanner(folder);
        Folder = folder;
        DisplayName = displayName;
        InstanceId = $"{nameof(StorageCore)}.{folder.Id}";
        Devices = new List<ICoreDevice>();
        Library = new StorageCoreLibrary(this);
    }

    /// <inheritdoc />
    public string Id => nameof(StorageCore);

    /// <summary>
    /// The folder being scanned for music.
    /// </summary>
    public IFolder Folder { get; }

    /// <summary>
    /// The scanner used to discover files.
    /// </summary>
    public IFolderScanner FolderScanner { get; }

    /// <inheritdoc />
    public virtual string DisplayName { get; }

    /// <inheritdoc />
    public virtual ICoreImage? Logo { get; set; }

    /// <inheritdoc/>
    public string InstanceId { get; }

    /// <inheritdoc />
    public virtual string InstanceDescriptor { get; set; } = string.Empty;

    /// <inheritdoc />
    public virtual MediaPlayerType PlaybackType => MediaPlayerType.Standard;

    /// <inheritdoc />
    public ICore SourceCore => this;

    /// <summary>
    /// Manages scanning and caching of all music metadata from files in a folder.
    /// </summary>
    internal FileMetadataManager? FileMetadataManager { get; set; }

    /// <summary>
    /// Reports progress for any file scans that take place.
    /// </summary>
    public Progress<FileScanState>? FileScanProgress { get; }

    /// <summary>
    /// The wait behavior of the metadata scanner when InitAsync is called in a file-based <see cref="ICore"/>.
    /// </summary>
    public ScannerWaitBehavior ScannerWaitBehavior { get; set; } = ScannerWaitBehavior.AlwaysWait;

    /// <inheritdoc/>
    public CoreState CoreState
    {
        get => _coreState;
        set
        {
            _coreState = value;
            CoreStateChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public ICoreUser? User => null;

    /// <inheritdoc/>
    public IReadOnlyList<ICoreDevice> Devices { get; }

    /// <inheritdoc/>
    public ICoreLibrary Library { get; }

    /// <inheritdoc />
    public ICoreSearch? Search => null;

    /// <inheritdoc/>
    public ICoreRecentlyPlayed? RecentlyPlayed => null;

    /// <inheritdoc/>
    public ICoreDiscoverables? Discoverables => null;

    /// <inheritdoc/>
    public ICorePlayableCollectionGroup? Pins => null;

    /// <inheritdoc/>
    public event EventHandler<CoreState>? CoreStateChanged;

    /// <inheritdoc />
    public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

    /// <inheritdoc />
    public event EventHandler<string>? InstanceDescriptorChanged;

    /// <inheritdoc />
    public event EventHandler<ICoreImage?>? LogoChanged;

    /// <inheritdoc />
    public event EventHandler<string>? DisplayNameChanged;

    /// <inheritdoc/>
    public virtual async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return;

        using (_initMutex.DisposableWaitAsync(cancellationToken: cancellationToken))
        {
            if (IsInitialized)
                return;

            CoreState = CoreState.Loading;

            FileMetadataManager = new FileMetadataManager(FolderScanner, _metadataCacheFolder, FileScanProgress);

            if (ScannerWaitBehavior == ScannerWaitBehavior.AlwaysWait)
                await FileMetadataManager.ScanAsync(cancellationToken);

            if (ScannerWaitBehavior == ScannerWaitBehavior.NeverWait)
                _ = FileMetadataManager.ScanAsync(cancellationToken);

            if (ScannerWaitBehavior == ScannerWaitBehavior.WaitIfNoData)
            {
                var existingMetadata = await FileMetadataManager.TryGetFileMetadataCacheAsync(cancellationToken);
                var scanningTask = FileMetadataManager.ScanAsync(cancellationToken);

                if (existingMetadata is null)
                    await scanningTask;
            }
        }

        CoreState = CoreState.Loaded;
    }

    /// <inheritdoc/>
    public bool IsInitialized { get; set; }

    /// <inheritdoc/>
    public virtual ValueTask DisposeAsync()
    {
        if (FileMetadataManager is not null)
            return FileMetadataManager.DisposeAsync();

        return default;
    }

    /// <inheritdoc/>
    public async Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        Guard.IsTrue(IsInitialized);
        Guard.IsNotNull(FileMetadataManager, nameof(FileMetadataManager));

        var artist = await FileMetadataManager.AlbumArtists.GetByIdAsync(id);
        if (artist != null)
            return new StorageCoreArtist(SourceCore, artist);

        var album = await FileMetadataManager.Albums.GetByIdAsync(id);
        if (album != null)
            return new StorageCoreAlbum(SourceCore, album);

        var track = await FileMetadataManager.Tracks.GetByIdAsync(id);
        if (track != null)
            return new StorageCoreTrack(SourceCore, track);

        return null;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// You may override this and return a different MediaSourceConfig if needed, such as a Stream instead of a file path.
    /// </remarks>
    public virtual async Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default)
    {
        Guard.IsTrue(IsInitialized);

        var file = FolderScanner.KnownFiles.FirstOrDefault(x => x.Id == track.Id);
        if (file is null)
            return null;

        if (!Path.GetExtension(file.Name).TryGetMimeType(out var mimeType))
        {
            Logger.LogWarning($"Unable to get mime type from file name {file.Name} for track {track.Id}");
            return null;
        }

        var stream = await file.OpenStreamAsync(FileAccess.Read, cancellationToken);
            
        return new MediaSourceConfig(track, track.Id, stream, mimeType);
    }
}