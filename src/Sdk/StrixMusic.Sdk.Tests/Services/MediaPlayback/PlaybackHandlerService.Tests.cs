using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
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
        [DataRow(11, 0)]
        [Timeout(800)]
        public async Task Shuffle_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            var prevItems = new List<IMediaSourceConfig>();
            for (int i = 0; i < numberOfPreviousItems; i++)
            {
                prevItems.Add(new MockMediaSourceConfig());
            }

            var nextItems = new List<IMediaSourceConfig>();
            for (int i = 0; i < numberOfNextItems; i++)
            {
                nextItems.Add(new MockMediaSourceConfig());
            }

            var handlerService = new PlaybackHandlerService(prevItems, nextItems, new MockAudioPlayerService());

            using (handlerService)
            {
                // Shuffle is on.
                await handlerService.ToggleShuffleAsync();

                Assert.AreEqual(prevItems.Count, 0);
                // TODO: more assertions.
            }

        }
    }
}
