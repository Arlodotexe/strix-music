// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;

namespace StrixMusic.Sdk.Extensions
{
    /// <summary>
    /// <see cref="IFolderData"/> extension methods.
    /// </summary>
    public static class FolderDataExtensions
    {
        /// <summary>
        /// Searches all files in a folder and including sub folders if any.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the folders in this directory.</returns>
        public static async Task<IList<IFileData>> RecursiveDepthFileSearchAsync(this IFolderData folder)
        {
            // TODO https://dev.azure.com/arloappx/Strix-Music/_backlogs/backlog/Strix-Music%20Team/Epics/?workitem=459
            // Move out of an extension method
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

                await RecursiveDepthFileSearchAsync(folder, fileDatas);
            }

            return fileDatas;
        }
    }
}
