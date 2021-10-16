using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using Windows.Storage;
using Windows.Storage.Streams;

namespace StrixMusic.Sdk.Uno.Models
{
    /// <inheritdoc />
    public class FileDataProperties : IFileDataProperties
    {
        private readonly StorageFile _storageFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataProperties"/> class.
        /// </summary>
        /// <param name="storageFile">The storage file to get properties from.</param>
        public FileDataProperties(StorageFile storageFile)
        {
            _storageFile = storageFile;
        }

        /// <inheritdoc />
        public async Task<MusicFileProperties?> GetMusicPropertiesAsync()
        {
            var storageFileMusicProps = await _storageFile.Properties.GetMusicPropertiesAsync();

            var itemThumbnail = await _storageFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView,256);
            var imageStream = await Convert(itemThumbnail);

            var musicFileProps = new MusicFileProperties()
            {
                Album = storageFileMusicProps.Album,
                AlbumArtist = storageFileMusicProps.AlbumArtist,
                Artist = storageFileMusicProps.Artist,
                Bitrate = storageFileMusicProps.Bitrate,
                Composers = storageFileMusicProps.Composers.ToArray(),
                Conductors = storageFileMusicProps.Conductors.ToArray(),
                Duration = storageFileMusicProps.Duration,
                Genres = storageFileMusicProps.Genre.ToArray(),
                Producers = storageFileMusicProps.Producers.ToArray(),
                Publisher = storageFileMusicProps.Publisher,
                Rating = storageFileMusicProps.Rating,
                Subtitle = storageFileMusicProps.Subtitle,
                Title = storageFileMusicProps.Title,
                TrackNumber = storageFileMusicProps.TrackNumber,
                Writers = storageFileMusicProps.Writers.ToArray(),
                Year = storageFileMusicProps.Year,
                Image = imageStream
            };

            return musicFileProps;
        }

        private async Task<Stream> Convert(IRandomAccessStream straem)
        {
            using (straem)
            {
                var dr = new DataReader(straem.GetInputStreamAt(0));
                var bytes = new byte[straem.Size];
                await dr.LoadAsync((uint)straem.Size);
                dr.ReadBytes(bytes);

                var stream = new MemoryStream(bytes);

                return stream;
            }
        }
    }
}