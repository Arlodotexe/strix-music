using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;

namespace StrixMusic.Cores.OneDrive.Storage
{
    /// <inheritdoc />
    public class OneDriveFileDataProperties : IFileDataProperties
    {
        private readonly DriveItem _driveItem;

        public OneDriveFileDataProperties(DriveItem driveItem)
        {
            _driveItem = driveItem;
        }

        /// <inheritdoc />
        public Task<MusicFileProperties?> GetMusicPropertiesAsync()
        {
            if (_driveItem.Audio is null)
                return Task.FromResult<MusicFileProperties?>(null);

            var audio = _driveItem.Audio;

            return Task.FromResult<MusicFileProperties?>(new MusicFileProperties
            {
                Album = audio.Album,
                AlbumArtist = audio.AlbumArtist,
                Artist = audio.AlbumArtist,
                Bitrate = (uint?)audio.Bitrate,
                Composers = audio.Composers.IntoList(),
                Conductors = null,
                Duration = TimeSpan.FromMilliseconds(audio.Duration ?? 0),
                Genres = audio.Genre.IntoList(),
                Producers = null,
                Publisher = null,
                Rating = null,
                Subtitle = null,
                Title = audio.Title,
                TrackNumber = (uint?)audio.Track,
                Writers = null,
                Year = (uint?)audio.Year,
            });
        }
    }
}
