using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// A <see cref="IFileScanner"/> that scans for all files in a folder and subfolders with a depth-first search.
    /// </summary>
    public class DepthFirstFileScanner : IFileScanner
    {
        private CancellationTokenSource? _scanningCancellationTokenSource;

        /// <summary>
        /// Creates a new instance of <see cref="DepthFirstFileScanner"/>.
        /// </summary>
        /// <param name="rootFolder">The root folder to operate in when scanning. Will be scanned recursively.</param>
        public DepthFirstFileScanner(IFolderData rootFolder)
        {
            RootFolder = rootFolder;

            AttachEvents();
        }

        private void AttachEvents()
        {
            // todo subscribe to file system changes.
        }

        private void DetachEvents()
        {
            // todo unsubscribe to file system changes.
        }

        /// <inheritdoc/>
        public event EventHandler? FileDiscoveryStarted;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<IFileData>>? FilesDiscovered;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<IFolderData>>? FoldersDiscovered;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<IFileData>>? FileDiscoveryCompleted;

        /// <inheritdoc/>
        public IFolderData RootFolder { get; }

        /// <inheritdoc/>
        public Task<IEnumerable<IFileData>> ScanFolder(IFolderData rootFolder)
        {
            return ScanFolder(rootFolder, new CancellationToken());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IFileData>> ScanFolder(IFolderData rootFolder, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var foldersToScan = new Stack<IFolderData>();
            foldersToScan.Push(rootFolder);

            try
            {
                FileDiscoveryStarted?.Invoke(this, EventArgs.Empty);

                var allDiscoveredFiles = await DFSFolderContentScan(foldersToScan, cancellationToken);

                FileDiscoveryCompleted?.Invoke(this, allDiscoveredFiles);

                return allDiscoveredFiles;
            }
            catch (OperationCanceledException)
            {
                return Enumerable.Empty<IFileData>();
            }
        }


        /// <summary>
        /// Performs a depth-first folder scan on the given <paramref name="foldersToCrawl"/>.
        /// </summary>
        /// <param name="foldersToCrawl">The folders to scan.</param>
        /// <returns>A <see cref="Queue{FileData}"/> with all discovered files.</returns>
        public virtual Task<Queue<IFileData>> DFSFolderContentScan(Stack<IFolderData> foldersToCrawl)
        {
            return DFSFolderContentScan(foldersToCrawl, new CancellationToken());
        }

        /// <summary>
        /// Performs a depth-first folder scan on the given <paramref name="foldersToCrawl"/>.
        /// </summary>
        /// <param name="foldersToCrawl">The folders to scan.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the ongoing task.</param>
        /// <returns>A <see cref="Queue{FileData}"/> with all discovered files.</returns>
        public virtual async Task<Queue<IFileData>> DFSFolderContentScan(Stack<IFolderData> foldersToCrawl, CancellationToken cancellationToken)
        {
            _scanningCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var filesToScan = new Queue<IFileData>();

            while (foldersToCrawl.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                var folderData = foldersToCrawl.Pop();

                var files = await folderData.GetFilesAsync();
                var filesList = files.ToList();

                foreach (var file in filesList)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    filesToScan.Enqueue(file);
                }

                if(filesList.Count > 0)
                    FilesDiscovered?.Invoke(this, filesList);

                var folders = await folderData.GetFoldersAsync();

                foreach (var folder in folders)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    foldersToCrawl.Push(folder);
                }

                FoldersDiscovered?.Invoke(this, folders);
            }

            return filesToScan;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachEvents();
        }
    }
}
