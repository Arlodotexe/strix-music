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
        [TestMethod]
        [DataRow(MetadataScanTypes.TagLib)]
        [Timeout(10000)]
        public async Task MultipleArtistsFromTrackTest(MetadataScanTypes scanTypes)
        {
            var audioFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Services\MetadataScanner\Samples");

            var scanner = new AudioMetadataScanner(degreesOfParallelism: 2);
            scanner.ScanTypes = scanTypes;
            scanner.CacheFolder = new SystemFolderData(Path.GetTempPath());

            var folder = new SystemFolderData(audioFilePath);
            var files = await folder.GetFilesAsync();

            var metadata = await scanner.ScanMusicFiles(files);

            Assert.IsTrue(metadata.Count() > 0);
            foreach (var item in metadata)
            {
                Assert.IsNotNull(item.ArtistMetadataCollection);
                Assert.IsTrue(item.ArtistMetadataCollection.Count() > 1);
            }
        }
    }
}
