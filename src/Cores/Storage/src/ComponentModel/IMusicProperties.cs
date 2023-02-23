using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Storage;

namespace StrixMusic.Cores.Storage.ComponentModel;

/// <summary>
/// Indicates that a stream of a music thumbnail can be opened.
/// </summary>
public interface IMusicProperties
{
    /// <summary>
    /// Returns a task that represents the asynchronous operation. Value is a stream of a thumbnail for an audio file.
    /// </summary>
    /// <returns>A stream of a thumbnail for an audio file.</returns>
    public Task<Stream> OpenMusicThumbnailStream();

    /// <summary>
    /// Gets the music properties for this file.
    /// </summary>
    /// <returns>A task that represents that asynchronous operation. Value is an storage property class containing data for music properties, with change support.</returns>
    public Task<IStorageProperty<MusicFileProperties>> GetMusicPropertiesAsync(CancellationToken cancellationToken = default);
}