using System.Threading.Tasks;

namespace StrixMusic.Services.StorageService
{
    /// <summary>
    /// The <see langword="interface"/> to handle storing and retreiving data.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Returns a stored setting, deserialized into a type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="filename">The name of the file to get.</param>
        /// <returns>String representation of the stored value. Null if file isn't found.</returns>
        Task<string> GetValueAsync(string filename);

        /// <summary>
        /// Stores data locally.
        /// </summary>
        /// <param name="filename">The name of the file (including the file extension).</param>
        /// <param name="value">The value to be stored.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder>.</returns>
        Task SetValueAsync(string filename, string value);

        /// <summary>
        /// Returns a stored setting, deserialized into a type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="filename">The name of the file to get.</param>
        /// <param name="path">A relative path (separated by forward slashes), to save the file in a subfolder.</param>
        /// <returns>String representation of the stored value. Null if file isn't found.</returns>
        Task<string> GetValueAsync(string filename, string path);

        /// <summary>
        /// Stores data locally.
        /// </summary>
        /// <param name="filename">The name of the file (including the file extension).</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="path">A relative path (separated by forward slashes), to save the file in a subfolder.</param>
        /// <returns>The <see cref="Task"/> representing the asyncronous operation.</returns>
        Task SetValueAsync(string filename, string value, string path);

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
