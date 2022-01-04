using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.Tests.Mock.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    [TestClass]
    public class PlaybackHandlerServiceQueueTests
    {
        private PlaybackHandlerService? _handlerService;
        private List<IMediaSourceConfig>? _previousItems;
        private List<IMediaSourceConfig>? _nextItems;

        [TestInitialize]
        public void Setup()
        {
            var player = new MockAudioPlayerService();
            _handlerService = new PlaybackHandlerService();
            _handlerService.RegisterAudioPlayer(player, "MockCore");

            _previousItems = new List<IMediaSourceConfig>();
            _nextItems = new List<IMediaSourceConfig>();
        }

        [TestCleanup]
        public void Teardown()
        {
            _handlerService?.Dispose();

            _previousItems?.Clear();
            _nextItems?.Clear();

            _previousItems = null;
            _nextItems = null;
        }

        [TestMethod]
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
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);
            Assert.AreEqual(_previousItems.Count, 0);
            Assert.AreEqual(_nextItems.Count, 0);

            var mockTrack = new MockCoreTrack(new MockCore(), string.Empty, string.Empty);

            // Generate previous items
            for (int i = 0; i < numberOfPreviousItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(mockTrack, i.ToString(), Stream.Null, "mp3");
                _previousItems.Add(mediaSourceConfig);
                _handlerService.PushPrevious(mediaSourceConfig);
            }

            // If there is anything in PreviousItems, there must be a CurrentItem.
            if (numberOfPreviousItems > 0)
            {
                _handlerService.CurrentItem = new MediaSourceConfig(mockTrack, numberOfPreviousItems.ToString(), Stream.Null, "mp3");
            }

            // Generate next items
            for (int i = 0; i < numberOfNextItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(mockTrack, (numberOfPreviousItems + (i + 1)).ToString(), Stream.Null, "mp3");
                _nextItems.Add(mediaSourceConfig);
                _handlerService.InsertNext(i, mediaSourceConfig);
            }

            Assert.IsFalse(_handlerService.ShuffleState);

            // Shuffle is on.
            await _handlerService.ToggleShuffleAsync();

            // Make sure previous items are emptied.
            Assert.AreEqual(_handlerService.PreviousItems.Count, 0);

            // Make sure no items end up at the same place.
            for (int o = 0; o < _nextItems.Count; o++)
                Assert.AreNotEqual(o, _handlerService.NextItems[o]);

            // Make sure all items from previous and next exist in next after shuffle.
            CollectionAssert.IsSubsetOf(_nextItems, _handlerService.NextItems.ToList());
            CollectionAssert.IsSubsetOf(_previousItems, _handlerService.NextItems.ToList());
        }

        [TestMethod]
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
        public async Task ShuffleAndUnshuffle_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);

            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            Assert.IsTrue(_handlerService.ShuffleState);

            // Turn shuffle off.
            await _handlerService.ToggleShuffleAsync();

            CollectionAssert.AreEqual(_nextItems, _handlerService.NextItems.ToList());
            CollectionAssert.AreEqual(_previousItems, _handlerService.PreviousItems.ToList());

            CollectionAssert.AllItemsAreNotNull(_handlerService.NextItems.ToList());
            CollectionAssert.AllItemsAreNotNull(_handlerService.PreviousItems.ToList());
        }
    }
}
