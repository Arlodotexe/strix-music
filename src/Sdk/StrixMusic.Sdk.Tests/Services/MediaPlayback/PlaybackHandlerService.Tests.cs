using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.Tests.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    [TestClass]
    public class PlaybackHandlerServiceTest
    {

        [TestMethod]
        [DataRow(0, 11)]
        [DataRow(1, 10)]
        [DataRow(2, 9)]
        [DataRow(3, 8)]
        [DataRow(4, 7)]
        [DataRow(5, 6)]
        [DataRow(6, 5)]
        [DataRow(7, 4)]
        [DataRow(8, 3)]
        [DataRow(9, 2)]
        [DataRow(10, 1)]
        [Timeout(800)]
        public async Task Shuffle_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            var prevItems = new List<IMediaSourceConfig>();
            for (int i = 0; i < numberOfPreviousItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(new MockCoreTrack(), i.ToString(), null, "mp3");
                prevItems.Add(mediaSourceConfig);
            }

            var nextItems = new List<IMediaSourceConfig>();
            for (int i = 0; i < numberOfNextItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(new MockCoreTrack(), (numberOfPreviousItems + i).ToString(), null, "mp3");
                nextItems.Add(mediaSourceConfig);
            }

            var audioPlayerDictionary = new Dictionary<string, IAudioPlayerService>();
            var audioPlayerService = new MockAudioPlayerService();
            audioPlayerDictionary.Add("MockCore", audioPlayerService);

            var handlerService = new PlaybackHandlerService(prevItems, nextItems, new MockAudioPlayerService(), audioPlayerDictionary);

            using (handlerService)
            {
                // Shuffle is on.
                await handlerService.ToggleShuffleAsync();

                // Make sure previous items are emptied.
                Assert.AreEqual(prevItems.Count, 0);

                // Maximum number of items that can be at the same place.
                int shuffleTolerence = 4;
                int numberOfElementsOnSameIndex = 0;

                for (int i = 0; i < handlerService.NextItems.Count; i++)
                {
                    if (nextItems[i].Id == handlerService.NextItems[i].Id)
                    {
                        numberOfElementsOnSameIndex++;
                    }

                    // Make sure shuffle actually shuffles .
                    Assert.AreNotEqual(numberOfElementsOnSameIndex, shuffleTolerence);
                }

                int previousItemsFound = 0;

                for (int i = 0; i < previousItemsFound; i++)
                {
                    if (nextItems[i].Id == prevItems[i].Id)
                    {
                        previousItemsFound++;
                    }
                }

                // Make sure all items from previous and next exist in next after shuffle.
                Assert.AreEqual(previousItemsFound, prevItems.Count);
            }

        }

        [TestMethod]
        [DataRow(0, 11)]
        [DataRow(1, 10)]
        [DataRow(2, 9)]
        [DataRow(3, 8)]
        [DataRow(4, 7)]
        [DataRow(5, 6)]
        [DataRow(6, 5)]
        [DataRow(7, 4)]
        [DataRow(8, 3)]
        [DataRow(9, 2)]
        [DataRow(10, 1)]
        [Timeout(800)]
        public async Task UnShuffle_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            var prevItems = new List<IMediaSourceConfig>();

            for (int i = 0; i < numberOfPreviousItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(new MockCoreTrack(), i.ToString(), null, "mp3");
                prevItems.Add(mediaSourceConfig);
            }

            var originalPrevItems = prevItems.ToList();

            var nextItems = new List<IMediaSourceConfig>();
            for (int i = 0; i < numberOfNextItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(new MockCoreTrack(), (numberOfPreviousItems + i).ToString(), null, "mp3");
                nextItems.Add(mediaSourceConfig);
            }
            var originalNextItems = nextItems.ToList();

            var audioPlayerDictionary = new Dictionary<string, IAudioPlayerService>();
            var audioPlayerService = new MockAudioPlayerService();
            audioPlayerDictionary.Add("MockCore", audioPlayerService);

            var handlerService = new PlaybackHandlerService(prevItems, nextItems, new MockAudioPlayerService(), audioPlayerDictionary);

            using (handlerService)
            {
                // Shuffle is on.
                await handlerService.ToggleShuffleAsync();

                // TODO: Simulating next track played, not currently because of MergedTrack dependency in the NextAsync.
                //await handlerService.NextAsync();
                //await handlerService.NextAsync();

                // Shuffle is off.
                await handlerService.ToggleShuffleAsync();

                int numberOfSameNextItems = 0;
                for (int i = 0; i < handlerService.NextItems.Count; i++)
                {
                    if (originalNextItems[i].Id == handlerService.NextItems[i].Id)
                    {
                        numberOfSameNextItems++;
                    }
                }
                // Check if next items are restored correctly.
                Assert.AreEqual(handlerService.NextItems.Count, numberOfSameNextItems);

                int numberOfSamePreviousItems = 0;
                for (int i = 0; i < handlerService.PreviousItems.Count; i++)
                {
                    if (originalPrevItems[i].Id == handlerService.PreviousItems.ElementAt(i).Id)
                    {
                        numberOfSamePreviousItems++;
                    }
                }

                // Check if previous items are restored correctly.
                Assert.AreEqual(handlerService.PreviousItems.Count, numberOfSamePreviousItems);
            }

        }
    }
}
