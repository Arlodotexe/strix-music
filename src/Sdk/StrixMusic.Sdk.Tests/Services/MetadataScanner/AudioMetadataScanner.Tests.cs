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
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        [DataRow(true)]
        [Timeout(10000)]
        public async Task MultipleArtistsFromTrackTest(bool isId3 = false)
        {
            var audioFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Services\MetadataScanner\Samples");
            var cacheFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Services\MetadataScanner\Cache");

            // Clean cache.

            var fm = new FileMetadataManager(new SystemFolderData(audioFilePath), new SystemFolderData(Path.GetTempPath()));

            if (isId3)
                fm.ScanTypes = MetadataScanTypes.TagLib;

            var scanner = new AudioMetadataScanner(fm);
            scanner.CacheFolder = new SystemFolderData(cacheFolder);

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

        private async void FileMetadataScanner_ScanningCompleted(object? sender, EventArgs e)
        {
            // Waits for the track to appear in the repository.
            await Task.Delay(2000);

            if (sender is FileMetadataManager fm)
            {
                var tracks = await fm.Tracks.GetItemsAsync(0, 99);

                bool faultyTrackMetadata = false;
                foreach (var item in tracks)
                {
                    Assert.IsNotNull(item.Id);

                    var results = await fm.Artists.GetArtistsByTrackId(item.Id, 0, 99);

                    if (results.Count < 2)
                    {
                        faultyTrackMetadata = true;
                        break;
                    }
                }

                // There should be no tracks with less than 2 artists.
                Assert.IsFalse(faultyTrackMetadata);
            }
        }
    }
}
