using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.Storage;
using StrixMusic.CoreInterfaces.Models.Storage;
using Windows.Storage;

namespace StrixMusic.UWP.Models
{
    /// <inheritdoc cref="IFileData"/>
    public class FileData : IFileData
    {
        /// <summary>
        /// The underlying <see cref="StorageFile"/> instance in use.
        /// </summary>
        internal StorageFile StorageFile { get; }

        /// <summary>
        /// Creates a new instance of <see cref="FileData" />.
        /// </summary>
        /// <param name="storageFile">The <see cref="StorageFile"/> to wrap.</param>
        public FileData(StorageFile storageFile)
        {
            StorageFile = storageFile;
        }

        /// <inheritdoc/>
        public string Path => StorageFile.Path;

        /// <inheritdoc/>
        public string Name => StorageFile.Name;

        /// <inheritdoc/>
        public string DisplayName => StorageFile.DisplayName;

        /// <inheritdoc/>
        public string FileExtension => StorageFile.FileType;

        /// <inheritdoc/>
        public IMusicFileProperties MusicProperties { get; set; } = new MusicFileProperties();

        /// <summary>
        /// Populates the <see cref="MusicProperties"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartMediaScan()
        {
            var storageFileMusicProps = await StorageFile.Properties.GetMusicPropertiesAsync();

            var musicFileProps = new MusicFileProperties()
            {
                Album = storageFileMusicProps.Album,
                AlbumArtist = storageFileMusicProps.AlbumArtist,
                Artist = storageFileMusicProps.Artist,
                Bitrate = storageFileMusicProps.Bitrate,
                Composers = storageFileMusicProps.Composers.ToArray(),
                Conductors = storageFileMusicProps.Conductors.ToArray(),
                Duration = storageFileMusicProps.Duration,
                Genre = storageFileMusicProps.Genre.ToArray(),
                Producers = storageFileMusicProps.Producers.ToArray(),
                Publisher = storageFileMusicProps.Publisher,
                Rating = storageFileMusicProps.Rating,
                Subtitle = storageFileMusicProps.Subtitle,
                Title = storageFileMusicProps.Title,
                TrackNumber = storageFileMusicProps.TrackNumber,
                Writers = storageFileMusicProps.Writers.ToArray(),
                Year = storageFileMusicProps.Year,
            };

            MusicProperties = musicFileProps;
        }

        /// <inheritdoc />
        public Task<Stream> GetStream()
        {
            var stream = StorageFile.OpenStreamForReadAsync();

            return stream;
        }
    }
}
