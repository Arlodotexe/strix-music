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

        private bool eventExecutionCompleted = false;

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        [Timeout(30000)]
        public async Task MultipleArtistsFromTrackTest()
        {
            var audioFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Services\MetadataScanner\Samples");
            var cacheFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Services\MetadataScanner\Cache");

            // Clean cache.

            if (Directory.Exists(cacheFolder))
                Directory.Delete(cacheFolder, true);

            var directory = new DirectoryInfo(cacheFolder);
            directory.Create();

            var _fileMetadataScanner = new FileMetadataManager(new MockFolderData(audioFilePath), new MockFolderData(cacheFolder));
            _fileMetadataScanner.ScanningCompleted += FileMetadataScanner_ScanningCompleted;


            _fileMetadataScanner.ScanTypes = MetadataScanTypes.TagLib;

            await _fileMetadataScanner.ScanAsync();

            // Waits for the Scanning even completion.
            while (!eventExecutionCompleted)
                await Task.Delay(100);
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

                eventExecutionCompleted = true;
            }
        }
    }
}
