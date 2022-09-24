using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Storage.SystemIO;
using StrixMusic.Cores.Storage.FileMetadata;
using StrixMusic.Cores.Storage.FileMetadata.Scanners;

namespace StrixMusic.Cores.Storage.Tests.MetadataScanner
{
    [TestClass]
    public class AudioMetadataScannerTests
    {
        [DataRow("MultipleSongArtists.mp3", MetadataScanTypes.TagLib)]
        [DataRow("MultipleSongArtists2.mp3", MetadataScanTypes.TagLib)]
        [TestMethod, Timeout(1000)]
        public async Task MultipleTrackArtists(string fileName, int scanType)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"MetadataScanner\Samples", fileName);
            var file = new SystemFile(filePath);

            var metadata = await AudioMetadataScanner.ScanMusicFileAsync(file, (MetadataScanTypes)scanType, CancellationToken.None);
            Assert.IsNotNull(metadata);

            Assert.IsNotNull(metadata.AlbumArtistMetadata);
            Assert.IsTrue(metadata.AlbumArtistMetadata.Any());

            Assert.IsNotNull(metadata.TrackArtistMetadata);
            Assert.IsTrue(metadata.TrackArtistMetadata.Count() > 1);
        }

        [DataRow("NoMetadata.mp3", MetadataScanTypes.TagLib)]
        [TestMethod, Timeout(1000)]
        public async Task NoMetadata(string fileName, int scanTypes)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"MetadataScanner\Samples", fileName);
            var file = new SystemFile(filePath);

            var metadata = await AudioMetadataScanner.ScanMusicFileAsync(file, (MetadataScanTypes)scanTypes, CancellationToken.None);
            Assert.IsNotNull(metadata);

            Assert.IsNotNull(metadata.TrackMetadata);
            Assert.IsNotNull(metadata.TrackMetadata.Duration > TimeSpan.FromSeconds(1));

            Assert.IsNotNull(metadata.TrackMetadata.ArtistIds);
            Assert.AreNotEqual(0, metadata.TrackMetadata.ArtistIds.Count());
            Assert.IsTrue(!metadata.TrackMetadata.ArtistIds.Any(string.IsNullOrWhiteSpace));

            Assert.IsTrue(!string.IsNullOrWhiteSpace(metadata.TrackMetadata.AlbumId));

            Assert.IsNotNull(metadata.TrackMetadata.Genres);
            Assert.AreEqual(0, metadata.TrackMetadata.Genres.Count());

            Assert.IsTrue(!string.IsNullOrWhiteSpace(metadata.TrackMetadata.Id));
            Assert.IsTrue(string.IsNullOrWhiteSpace(metadata.TrackMetadata.Title));

            Assert.IsNotNull(metadata.AlbumArtistMetadata);
            Assert.IsTrue(metadata.AlbumArtistMetadata.Count == 1);
            Assert.IsTrue(metadata.AlbumArtistMetadata.All(x => x.Name is not null));
            Assert.IsTrue(metadata.AlbumArtistMetadata.All(x => x.AlbumIds is not null));
            Assert.IsTrue(metadata.AlbumArtistMetadata.All(x => x.AlbumIds!.All(x => !string.IsNullOrWhiteSpace(x))));
            Assert.IsTrue(metadata.AlbumArtistMetadata.All(x => x.TrackIds is not null));
            Assert.IsTrue(metadata.AlbumArtistMetadata.All(x => x.TrackIds!.All(x => !string.IsNullOrWhiteSpace(x))));

            Assert.IsNotNull(metadata.TrackArtistMetadata);
            Assert.IsTrue(metadata.TrackArtistMetadata.Count() == 1);
            Assert.IsTrue(metadata.TrackArtistMetadata.All(x => x.Name is not null));
            Assert.IsTrue(metadata.TrackArtistMetadata.All(x => x.AlbumIds is not null));
            Assert.IsTrue(metadata.TrackArtistMetadata.All(x => x.AlbumIds!.All(x => !string.IsNullOrWhiteSpace(x))));
            Assert.IsTrue(metadata.TrackArtistMetadata.All(x => x.TrackIds is not null));
            Assert.IsTrue(metadata.TrackArtistMetadata.All(x => x.TrackIds!.All(x => !string.IsNullOrWhiteSpace(x))));

            Assert.IsNotNull(metadata.TrackArtistMetadata);
            Assert.IsTrue(metadata.TrackArtistMetadata.Count() == 1);
        }
    }
}
