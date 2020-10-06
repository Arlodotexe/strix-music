using System.Threading.Tasks;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Provides access to the properties of a file.
    /// </summary>
    public interface IFileDataProperties
    {
        /// <summary>
        /// Returns music properties.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Value is the requested <see cref="MusicFileProperties"/> if found. Otherwise null.</returns>
        Task<MusicFileProperties?> GetMusicPropertiesAsync();
    }
}