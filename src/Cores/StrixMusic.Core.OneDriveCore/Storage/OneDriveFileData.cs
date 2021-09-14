using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Graph;
using OwlCore.AbstractStorage;
using OwlCore.Validation.Mime;

namespace StrixMusic.Cores.OneDrive.Storage
{
    /// <summary>
    /// Wraps around an instance of  <see cref="DriveItem"/> to implement a file for AbstractStorage.
    /// </summary>
    public class OneDriveFileData : IFileData
    {
        private readonly GraphServiceClient _graphClient;
        private readonly DriveItem _driveItem;

        /// <summary>
        /// Creates a new instance of <see cref="OneDriveFolderData"/>.
        /// </summary>
        /// <param name="graphClient">The service that handles API requests to Microsoft Graph.</param>
        /// <param name="driveItem">An instance of <see cref="DriveItem"/> that represents the underlying OneDrive folder.</param>
        public OneDriveFileData(GraphServiceClient graphClient, DriveItem driveItem)
        {
            _graphClient = graphClient;
            _driveItem = driveItem;
        }

        /// <inheritdoc />
        public string Path => _driveItem.WebUrl;

        /// <inheritdoc />
        public string Name => _driveItem.Name;

        /// <inheritdoc />
        public string DisplayName => _driveItem.Name;

        /// <inheritdoc />
        public string FileExtension => MimeTypeMap.GetExtension(_driveItem.File.MimeType);

        /// <inheritdoc />
        public IFileDataProperties Properties { get; set; } = new OneDriveFileDataProperties();

        /// <inheritdoc />
        public async Task<IFolderData> GetParentAsync()
        {
            var parent = await _graphClient.Drive.Items[_driveItem.ParentReference.DriveId].Request().GetAsync();

            return new OneDriveFolderData(_graphClient, parent);
        }

        /// <inheritdoc />
        public Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read)
        {
            return Task.FromResult(_driveItem.Content);
        }

        /// <inheritdoc />
        public Task Delete()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes)
        {
            throw new NotSupportedException();
        }
    }
}
