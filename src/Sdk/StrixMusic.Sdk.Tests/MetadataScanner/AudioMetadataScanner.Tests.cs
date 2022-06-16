using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.FileMetadata.Scanners;
using StrixMusic.Sdk.Tests.Mock.FileSystem;

namespace StrixMusic.Sdk.Tests.Services.MetadataScanner
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

            var metadata = await scanner.ScanMusicFiles(new[] { file });
            Assert.IsTrue(metadata.Count() > 0);

            foreach (var item in metadata)
            {
                Assert.IsNotNull(item.AlbumArtistMetadata);
                Assert.IsTrue(item.AlbumArtistMetadata.Count() > 0);

                Assert.IsNotNull(item.TrackArtistMetadata);
                Assert.IsTrue(item.TrackArtistMetadata.Count() > 1);
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

            var metadata = await scanner.ScanMusicFiles(new[] { file });
            Assert.IsTrue(metadata.Count() > 0);

            foreach (var item in metadata)
            {
                Assert.IsNotNull(item.TrackMetadata);
                Assert.IsNotNull(item.TrackMetadata.Duration > TimeSpan.FromSeconds(1));

                Assert.IsNotNull(item.TrackMetadata.ArtistIds);
                Assert.AreNotEqual(0, item.TrackMetadata.ArtistIds.Count());
                Assert.IsTrue(!item.TrackMetadata.ArtistIds.Any(string.IsNullOrWhiteSpace));

                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.TrackMetadata.AlbumId));

                Assert.IsNotNull(item.TrackMetadata.Genres);
                Assert.AreEqual(0, item.TrackMetadata.Genres.Count());

                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.TrackMetadata.Id));
                Assert.IsTrue(string.IsNullOrWhiteSpace(item.TrackMetadata.Title));

                Assert.IsNotNull(item.AlbumArtistMetadata);
                Assert.IsTrue(item.AlbumArtistMetadata.Count() == 1);
                Assert.IsTrue(item.AlbumArtistMetadata.All(x => x.Name is not null));
                Assert.IsTrue(item.AlbumArtistMetadata.All(x => x.AlbumIds is not null));
                Assert.IsTrue(item.AlbumArtistMetadata.All(x => x.AlbumIds!.All(x => !string.IsNullOrWhiteSpace(x))));
                Assert.IsTrue(item.AlbumArtistMetadata.All(x => x.TrackIds is not null));
                Assert.IsTrue(item.AlbumArtistMetadata.All(x => x.TrackIds!.All(x => !string.IsNullOrWhiteSpace(x))));

                Assert.IsNotNull(item.TrackArtistMetadata);
                Assert.IsTrue(item.TrackArtistMetadata.Count() == 1);
                Assert.IsTrue(item.TrackArtistMetadata.All(x => x.Name is not null));
                Assert.IsTrue(item.TrackArtistMetadata.All(x => x.AlbumIds is not null));
                Assert.IsTrue(item.TrackArtistMetadata.All(x => x.AlbumIds!.All(x => !string.IsNullOrWhiteSpace(x))));
                Assert.IsTrue(item.TrackArtistMetadata.All(x => x.TrackIds is not null));
                Assert.IsTrue(item.TrackArtistMetadata.All(x => x.TrackIds!.All(x => !string.IsNullOrWhiteSpace(x))));

                Assert.IsNotNull(item.TrackArtistMetadata);
                Assert.IsTrue(item.TrackArtistMetadata.Count() == 1);
            }
        }
    }
}
