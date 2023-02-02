using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Storage;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

/// <summary>
/// A <see cref="IFolderScanner"/> that scans for all files in a folder and subfolders with a depth-first search.
/// </summary>
internal class DepthFirstFolderScanner : IFolderScanner
{
    private readonly Dictionary<string, SubFolderData> _knownSubFolders = new();
    private IFolderWatcher? _rootFolderWatcher;

    /// <summary>
    /// Creates a new instance of <see cref="DepthFirstFolderScanner"/>.
    /// </summary>
    /// <param name="rootFolder">The root folder to operate in when scanning. Will be scanned recursively.</param>
    public DepthFirstFolderScanner(IFolder rootFolder)
    {
        RootFolder = rootFolder;
    }

    /// <inheritdoc/>
    public IFolder RootFolder { get; }

    /// <inheritdoc/>
    public async IAsyncEnumerable<IAddressableFile> ScanFolderAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _knownSubFolders.Clear();
        KnownFiles.Clear();

        await foreach (var item in RecursiveScanForFilesAsync(RootFolder, cancellationToken))
            yield return item;
    }

    private async IAsyncEnumerable<IAddressableFile> RecursiveScanForFilesAsync(IFolder folderToScan, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        lock (_knownSubFolders)
        {
            _knownSubFolders[folderToScan.Id] = new SubFolderData((IAddressableFolder)folderToScan, children: new List<IAddressableStorable>(), folderWatcher: null);
        }

        if (folderToScan is IMutableFolder mutableFolder)
            await EnableFolderWatcherAsync(mutableFolder, cancellationToken);

        await foreach (var item in folderToScan.GetItemsAsync(cancellationToken: cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (RootFolder.Id != item.Id)
                _knownSubFolders[folderToScan.Id].Children.Add(item);

            if (item is IAddressableFile file)
            {
                KnownFiles.Add(file);
                yield return file;
            }

            if (item is IAddressableFolder folder)
            {
                await foreach (var subFile in RecursiveScanForFilesAsync(folder))
                {
                    KnownFiles.Add(subFile);
                    yield return subFile;
                }
            }
        }
    }

    /// <inheritdoc/>
    public ObservableCollection<IAddressableFile> KnownFiles { get; } = new();

    private async Task EnableFolderWatcherAsync(IMutableFolder folder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var folderWatcher = await folder.GetFolderWatcherAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            lock (_knownSubFolders)
            {
                if (folder.Id == RootFolder.Id)
                    _rootFolderWatcher = folderWatcher;
                else
                    _knownSubFolders[folder.Id].FolderWatcher = folderWatcher;
            }

            folderWatcher.CollectionChanged += FolderWatcherOnCollectionChanged;
        }
        catch (System.Exception ex)
        {
            OwlCore.Diagnostics.Logger.LogError("Couldn't get folder watcher", ex);
        }
    }

    private async Task DisableFolderWatcherAsync(IMutableFolder folder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var folderWatcher = folder.Id == RootFolder.Id ? _rootFolderWatcher : _knownSubFolders[folder.Id].FolderWatcher;
            Guard.IsNotNull(folderWatcher);

            folderWatcher.CollectionChanged -= FolderWatcherOnCollectionChanged;

            if (folder.Id == RootFolder.Id)
                _rootFolderWatcher = null;
            else
            {
                lock (_knownSubFolders)
                {
                    _knownSubFolders[folder.Id].FolderWatcher = null;
                }
            }

            await folderWatcher.DisposeAsync();
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (System.Exception ex)
        {
            OwlCore.Diagnostics.Logger.LogError("Couldn't get folder watcher", ex);
        }
    }

    private async void FolderWatcherOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var newItem in e.NewItems)
            {
                Guard.IsTrue(newItem is IAddressableStorable);
                var parentFolder = ((IFolderWatcher)sender).Folder;

                // Parent folder must be in _knownSubFolders already.
                // Record this new item as a child of the parent folder.
                lock (_knownSubFolders)
                {
                    if (_knownSubFolders.First(x => x.Key == parentFolder.Id).Value is { } subFolderData)
                        subFolderData.Children.Add((IAddressableStorable)newItem);
                }

                if (newItem is IAddressableFile newFile)
                    KnownFiles.Add(newFile);

                if (newItem is IAddressableFolder newFolder)
                {
                    await RecursiveScanForFilesAsync(newFolder).ToListAsync();
                }
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var oldItem in e.OldItems)
            {
                if (oldItem is not IStorable removedStorable)
                    continue;

                // Remove known file.
                if (KnownFiles.FirstOrDefault(x => x.Id == removedStorable.Id) is { } knownFile)
                    KnownFiles.Remove(knownFile);

                SubFolderData? targetSubFolderData;

                lock (_knownSubFolders)
                {
                    targetSubFolderData = _knownSubFolders.FirstOrDefault(x => x.Key == removedStorable.Id).Value;
                }

                // Remove known folder and all children, recursively.
                if (targetSubFolderData is not null)
                    await RemoveSubFolderRecursiveAsync(targetSubFolderData);
            }
        }
    }

    private async Task RemoveSubFolderRecursiveAsync(SubFolderData subFolderToRemove)
    {
        lock (_knownSubFolders)
        {
            _knownSubFolders.Remove(subFolderToRemove.Folder.Id);
        }

        if (subFolderToRemove.Folder is IMutableFolder mutableFolder)
            await DisableFolderWatcherAsync(mutableFolder, CancellationToken.None);

        foreach (var childItem in subFolderToRemove.Children)
        {
            if (childItem is IFile childFile && KnownFiles.FirstOrDefault(x => x.Id == childFile.Id) is { } knownChildFile)
                KnownFiles.Remove(knownChildFile);

            SubFolderData? targetSubFolderData;

            lock (_knownSubFolders)
            {
                if (childItem is not IFolder childFolder)
                    continue;

                targetSubFolderData = _knownSubFolders.FirstOrDefault(x => x.Key == childFolder.Id).Value;
            }

            if (targetSubFolderData is not null)
                await RemoveSubFolderRecursiveAsync(targetSubFolderData);

        }
    }

    private record SubFolderData(IAddressableFolder folder, List<IAddressableStorable> children, IFolderWatcher? folderWatcher)
    {
        public List<IAddressableStorable> Children { get; init; } = children;

        public IAddressableFolder Folder { get; init; } = folder;

        public IFolderWatcher? FolderWatcher { get; set; } = folderWatcher;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_knownSubFolders)
        {
            foreach (var item in _knownSubFolders)
            {
                if (item.Value.folder is IMutableFolder mutableFolder)
                    _ = DisableFolderWatcherAsync(mutableFolder, CancellationToken.None);
            }
        }

        if (RootFolder is IMutableFolder mutableRoot)
            _ = DisableFolderWatcherAsync(mutableRoot, CancellationToken.None);
    }
}
