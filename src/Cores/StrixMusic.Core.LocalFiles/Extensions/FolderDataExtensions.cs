using OwlCore.AbstractStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Extensions
{
    /// <summary>
    /// <see cref="IFolderData"/> extentions methods for <see cref="LocalFileCore"/>.
    /// </summary>
    public static class FolderDataExtensions
    {
        /// <summary>
        /// Searches all files in a folder and including sub folders if any.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the folders in this directory.</returns>
        public static async Task<IList<IFileData>> RecursiveDepthFileSearchAsync(this IFolderData folder)
        {
            var files = await folder.GetFilesAsync();

            return await RecursiveDepthFileSearchAsync(folder, files.ToList());
        }

        private static async Task<IList<IFileData>> RecursiveDepthFileSearchAsync(IFolderData folderData, IList<IFileData> fileDatas)
        {
            var folders = await folderData.GetFoldersAsync();
            foreach (var folder in folders)
            {
                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    fileDatas.Add(file);
                }

                return await RecursiveDepthFileSearchAsync(folder, fileDatas);
            }

            return fileDatas;
        }
    }
}
