using System.Threading.Tasks;

namespace StrixMusic.Sdk.Services.StorageService
{
    /// <summary>
    /// The <see langword="interface"/> to handle storing and retreiving data.
    /// </summary>
    public interface ITextStorageService
    {
        /// <summary>
        /// Returns a stored setting, deserialized into a type.
        /// </summary>
        /// <param name="filename">The name of the file to get.</param>
        /// <returns>String representation of the stored value. Null if file isn't found.</returns>
        Task<string?> GetValueAsync(string filename);

        /// <summary>
        /// Stores data locally.
        /// </summary>
        /// <param name="filename">The name of the file (including the file extension).</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation..</returns>
        Task SetValueAsync(string filename, string value);

        /// <summary>
        /// Returns a stored setting, deserialized into a type.
        /// </summary>
        /// <param name="filename">The name of the file to get.</param>
        /// <param name="path">A relative path (separated by forward slashes), to save the file in a subfolder.</param>
        /// <returns>String representation of the stored value. Null if file isn't found.</returns>
        Task<string?> GetValueAsync(string filename, string path);

        /// <summary>
        /// Stores data locally.
        /// </summary>
        /// <param name="filename">The name of the file (including the file extension).</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="path">A relative path (separated by forward slashes), to save the file in a subfolder.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string filename, string value, string path);

        /// <summary>
        /// Removes all stored data associated with a <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The identifier to lookup.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveByPathAsync(string path);

        /// <summary>
        /// Removes all saved settings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveAll();

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <param name="filename">The name of the file (including the file extension).</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        Task<bool> FileExistsAsync(string filename);

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <param name="filename">The name of the file (including the file extension).</param>
        /// <param name="path">A relative path (separated by forward slashes), to save the file in a subfolder.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        Task<bool> FileExistsAsync(string filename, string path);
    }
}
