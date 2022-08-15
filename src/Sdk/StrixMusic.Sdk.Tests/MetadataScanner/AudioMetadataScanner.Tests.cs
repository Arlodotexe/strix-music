using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.FileMetadata.Scanners;
using StrixMusic.Sdk.Tests.Mock.FileSystem;

namespace StrixMusic.Sdk.Tests.MetadataScanner
{
    [TestClass]
    public class AudioMetadataScannerTests
    {
        [DataRow("MultipleSongArtists.mp3", MetadataScanTypes.TagLib)]
        [DataRow("MultipleSongArtists2.mp3", MetadataScanTypes.TagLib)]
        [TestMethod, Timeout(1000)]
        public async Task AlbumArtist_And_MultipleTrackArtist(string fileName, MetadataScanTypes scanTypes)
        {
            var scanner = new AudioMetadataScanner(degreesOfParallelism: 2);
            scanner.ScanTypes = scanTypes;

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"MetadataScanner\Samples", fileName);
            var file = new SystemFileData(filePath);

            var metadata = await scanner.ScanMusicFilesAsync(new[] { file }, default).ToListAsync();
            Assert.IsTrue(metadata.Count > 0);

            foreach (var item in metadata)
            {
                Assert.IsNotNull(item.Metadata.AlbumArtistMetadata);
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.Any());

                Assert.IsNotNull(item.Metadata.TrackArtistMetadata);
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.Count() > 1);
            }
        }

        [DataRow("NoMetadata.mp3", MetadataScanTypes.TagLib)]
        [TestMethod, Timeout(1000)]
        public async Task NoMetadata(string fileName, MetadataScanTypes scanTypes)
        {
            var scanner = new AudioMetadataScanner(degreesOfParallelism: 2);
            scanner.ScanTypes = scanTypes;

            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"MetadataScanner\Samples", fileName);
            var file = new SystemFileData(filePath);

            var metadata = await scanner.ScanMusicFilesAsync(new[] { file }, default).ToListAsync();
            Assert.IsTrue(metadata.Count > 0);

            foreach (var item in metadata)
            {
                Assert.IsNotNull(item.Metadata.TrackMetadata);
                Assert.IsNotNull(item.Metadata.TrackMetadata.Duration > TimeSpan.FromSeconds(1));

                Assert.IsNotNull(item.Metadata.TrackMetadata.ArtistIds);
                Assert.AreNotEqual(0, item.Metadata.TrackMetadata.ArtistIds.Count());
                Assert.IsTrue(!item.Metadata.TrackMetadata.ArtistIds.Any(string.IsNullOrWhiteSpace));

                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Metadata.TrackMetadata.AlbumId));

                Assert.IsNotNull(item.Metadata.TrackMetadata.Genres);
                Assert.AreEqual(0, item.Metadata.TrackMetadata.Genres.Count());

                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Metadata.TrackMetadata.Id));
                Assert.IsTrue(string.IsNullOrWhiteSpace(item.Metadata.TrackMetadata.Title));

                Assert.IsNotNull(item.Metadata.AlbumArtistMetadata);
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.Count == 1);
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.All(x => x.Name is not null));
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.All(x => x.AlbumIds is not null));
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.All(x => x.AlbumIds!.All(x => !string.IsNullOrWhiteSpace(x))));
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.All(x => x.TrackIds is not null));
                Assert.IsTrue(item.Metadata.AlbumArtistMetadata.All(x => x.TrackIds!.All(x => !string.IsNullOrWhiteSpace(x))));

                Assert.IsNotNull(item.Metadata.TrackArtistMetadata);
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.Count() == 1);
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.All(x => x.Name is not null));
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.All(x => x.AlbumIds is not null));
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.All(x => x.AlbumIds!.All(x => !string.IsNullOrWhiteSpace(x))));
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.All(x => x.TrackIds is not null));
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.All(x => x.TrackIds!.All(x => !string.IsNullOrWhiteSpace(x))));

                Assert.IsNotNull(item.Metadata.TrackArtistMetadata);
                Assert.IsTrue(item.Metadata.TrackArtistMetadata.Count() == 1);
            }
        }
    }
}
