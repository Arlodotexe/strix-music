using OwlCore.AbstractStorage;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    public partial class PlaylistMetadataScanner
    {
        private static string? TryGetHashFromExistingTracks(Uri path, IEnumerable<FileMetadata?> files)
        {
            return files.FirstOrDefault(c => c?.TrackMetadata?.Url == path.AbsoluteUri)?.Id;
        }

        /// <summary>
        /// Determines whether a given path is full or relative.
        /// </summary>
        private static bool IsFullPath(string path)
        {
            // FIXME: http:// paths are not recognized as absolute paths
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || !Path.IsPathRooted(path))
                return false;

            var pathRoot = Path.GetPathRoot(path);
            if (pathRoot.Length <= 2 && pathRoot != "/") // Accepts X:\ and \\UNC\PATH, rejects empty string, \ and X:, but accepts / to support Linux
                return false;

            if (pathRoot[0] != '\\' || pathRoot[1] != '\\')
                return true; // Rooted and not a UNC path

            return pathRoot.Trim('\\').IndexOf('\\') != -1; // A UNC server name without a share name (e.g "\\NAME" or "\\NAME\") is invalid
        }

        /// <summary>
        /// Resolves a potentially relative file path to an absolute path.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <param name="currentPath">The path to append to <paramref name="path"/>
        /// if it's relative.</param>
        /// <remarks>
        /// This method is safe to use on absolute paths as well.
        /// Does not work for Unix paths.
        /// </remarks>
        /// <returns>An absolute path.</returns>
        public static string ResolveFilePath(string path, string currentPath)
        {
            // Check if the path is absolute
            if (IsFullPath(path))
            {
                // Path is absolute
                return path;
            }
            else
            {
                // Path is relative
                string fullPath;
                if (path.StartsWith("~", StringComparison.InvariantCulture))
                {
                    // Unix relative file path
                    fullPath = Path.GetFullPath(Path.GetDirectoryName(currentPath) + path.Substring(1));
                }
                else
                {
                    fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(currentPath) ?? string.Empty, path));
                }

                return fullPath;
            }
        }

        /// <summary>
        /// Resolves a potentially relative file path to an absolute path.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <param name="fileData">The file to resolve paths relative to.</param>
        /// <remarks>This method is safe to use on absolute paths as well.</remarks>
        /// <returns>An absolute path.</returns>
        public static string ResolveFilePath(string path, IFileData fileData)
        {
            return ResolveFilePath(path, Path.GetDirectoryName(fileData.Path) ?? string.Empty);
        }
    }
}
