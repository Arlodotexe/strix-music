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
    public IAsyncEnumerable<IAddressableFile> ScanFolderAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _knownSubFolders.Clear();
        KnownFiles.Clear();

        return RecursiveScanForFilesAsync(RootFolder, cancellationToken);
    }

    private async IAsyncEnumerable<IAddressableFile> RecursiveScanForFilesAsync(IFolder folderToScan, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (folderToScan != RootFolder)
            _knownSubFolders[folderToScan.Id] = new SubFolderData((IAddressableFolder)folderToScan, children: new List<IAddressableStorable>(), folderWatcher: null);

        if (folderToScan is IMutableFolder mutableFolder)
            await EnableFolderWatcherAsync(mutableFolder, cancellationToken);

        await foreach (var item in folderToScan.GetItemsAsync(cancellationToken: cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
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
        var folderWatcher = await folder.GetFolderWatcherAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        _knownSubFolders[folder.Id].FolderWatcher = folderWatcher;

        folderWatcher.CollectionChanged += FolderWatcherOnCollectionChanged;
    }

    private async Task DisableFolderWatcherAsync(IMutableFolder folder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var folderWatcher = _knownSubFolders[folder.Id].FolderWatcher;
        Guard.IsNotNull(folderWatcher);

        folderWatcher.CollectionChanged -= FolderWatcherOnCollectionChanged;

        _knownSubFolders[folder.Id].FolderWatcher = null;
        await folderWatcher.DisposeAsync();
        cancellationToken.ThrowIfCancellationRequested();
    }

    private async void FolderWatcherOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var newItem in e.NewItems)
            {
                Guard.IsOfType<IAddressableStorable>(newItem);
                var parentFolder = ((IFolderWatcher)sender).Folder;

                // Parent folder must be in _knownSubFolders already.
                // Record this new item as a child of the parent folder.
                if (_knownSubFolders.First(x => x.Key == parentFolder.Id).Value is { } subFolderData)
                    subFolderData.Children.Add((IAddressableStorable)newItem);

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

                // Remove known folder and all children, recursively.
                if (_knownSubFolders.FirstOrDefault(x => x.Key == removedStorable.Id).Value is { } subFolderData)
                    await RemoveSubFolderRecursiveAsync(subFolderData);
            }
        }
    }

    private async Task RemoveSubFolderRecursiveAsync(SubFolderData subFolderToRemove)
    {
        _knownSubFolders.Remove(subFolderToRemove.Folder.Id);

        if (subFolderToRemove.Folder is IMutableFolder mutableFolder)
            await DisableFolderWatcherAsync(mutableFolder, CancellationToken.None);

        foreach (var childItem in subFolderToRemove.Children)
        {
            if (childItem is IFile childFile && KnownFiles.FirstOrDefault(x => x.Id == childFile.Id) is { } knownChildFile)
                KnownFiles.Remove(knownChildFile);

            if (childItem is IFolder childFolder && _knownSubFolders.FirstOrDefault(x => x.Key == childFolder.Id).Value is { } knownChildFolder)
            {
                await RemoveSubFolderRecursiveAsync(knownChildFolder);
            }
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
        foreach (var item in _knownSubFolders)
            item.Value.FolderWatcher?.Dispose();
    }
}
