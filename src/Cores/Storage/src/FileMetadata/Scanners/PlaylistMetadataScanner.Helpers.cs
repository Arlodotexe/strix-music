using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Storage;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

internal partial class PlaylistMetadataScanner
{
    /// <summary>
    /// Traverses a relative path from the provided <see cref="IStorable"/> and returns the item at that path.
    /// </summary>
    /// <param name="from">The item to start with when traversing.</param>
    /// <param name="relativePath">The path of the storable item to return, relative to the provided item.</param>
    /// <param name="cancellationToken">A token to cancel the ongoing operation.</param>
    /// <returns>The <see cref="IStorable"/> item found at the relative path.</returns>
    /// <exception cref="ArgumentException">
    /// A parent directory was specified, but the provided <see cref="IStorable"/> is not addressable.
    /// Or, the provided relative path named a folder, but the item was a file. 
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">A parent folder was requested, but the storable item did not return a parent.</exception>
    /// <exception cref="FileNotFoundException">A named item was specified in a folder, but the item wasn't found.</exception>
    internal static async Task<IStorable> TraverseRelativePathAsync(this IStorable from, string relativePath, CancellationToken cancellationToken = default)
    {
        var directorySeparatorChar = Path.DirectorySeparatorChar;

        // Traverse only one level at a time
        // But recursively, until the target has been reached.
        var pathParts = relativePath.Split(directorySeparatorChar).Where(x => !string.IsNullOrWhiteSpace(x) && x != ".").ToArray();

        // Current directory was specified.
        if (pathParts.Length == 0)
            return from;

        var nextDirectoryName = pathParts[0];
        Guard.IsNotNullOrWhiteSpace(nextDirectoryName);

        // Get parent directory.
        if (nextDirectoryName == "..")
        {
            if (from is not IAddressableStorable addressableStorable)
                throw new ArgumentException($"A parent folder was requested, but the storable item named {from.Name} is not addressable.", nameof(relativePath));

            var parent = await addressableStorable.GetParentAsync(cancellationToken);

            // If this item was the last one needed.
            if (parent is not null && pathParts.Length == 1)
                return parent;

            if (parent is null)
                throw new ArgumentOutOfRangeException(nameof(relativePath), "A parent folder was requested, but the storable item did not return a parent.");

            return await TraverseRelativePathAsync(parent, string.Join(directorySeparatorChar.ToString(), pathParts.Skip(1)));
        }

        // Get child item by name.
        if (from is not IFolder folder)
            throw new ArgumentException($"An item named {nextDirectoryName} was requested from the folder named {from.Name}, but {from.Name} is not a folder.");

        var item = await folder.GetItemsAsync(cancellationToken: cancellationToken).FirstOrDefaultAsync(x => x.Name == nextDirectoryName, cancellationToken: cancellationToken);

        if (item is null)
            throw new FileNotFoundException($"An item named {nextDirectoryName} was requested from the folder named {from.Name}, but {nextDirectoryName} wasn't found in the folder.");

        return await TraverseRelativePathAsync(item, string.Join(directorySeparatorChar.ToString(), pathParts.Skip(1)));
    }

    /*string GetParentPath(string relativePath)
    {
        // Path.GetDirectoryName() treats strings that end with a directory separator as a directory. If there's no trailing slash, it's treated as a file.
        // Run it twice for folders. The first time only shaves off the trailing directory separator.
        var parentDirectoryName = relativePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? Path.GetDirectoryName(Path.GetDirectoryName(relativePath)) : Path.GetDirectoryName(relativePath);

        // It also doesn't return a string that has a path separator at the end.
        return parentDirectoryName + directorySeparatorChar;
    }

    string GetParentDirectoryName(string relativePath)
    {
        var parentPath = GetParentPath(relativePath);
        var parentParentPath = GetParentPath(parentPath);

        return parentPath.Replace(parentParentPath, "").TrimEnd(directorySeparatorChar);
    }*/
}
