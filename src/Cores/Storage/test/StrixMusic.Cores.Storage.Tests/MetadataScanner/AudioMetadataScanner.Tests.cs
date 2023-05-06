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
        [TestMethod]
        public async Task NoMetadata(string fileName, int scanTypes)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"MetadataScanner\Samples", fileName);
            var file = new SystemFile(filePath);

            var metadata = await AudioMetadataScanner.ScanMusicFileAsync(file, (MetadataScanTypes)scanTypes, CancellationToken.None);
            
            // The scanner returns null if no metadata found.
            Assert.IsNull(metadata);
        }
    }
}
