
namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Event args for the file scanning related events.
    /// </summary>
    public class FileScanStateEventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="FileScanStateEventArgs"/>.
        /// </summary>
        /// <param name="folderData">The containing folder.</param>
        /// <param name="fileData">The file being updated.</param>
        public FileScanStateEventArgs(IFolderData folderData, IFileData fileData)
        {
            FolderData = folderData;
            FileData = fileData;
        }

        /// <inheritdoc cref="IFileData"/>
        public IFileData FileData { get; set; }

        /// <inheritdoc cref="IFolderData"/>
        public IFolderData FolderData { get; set; }
    }
}
