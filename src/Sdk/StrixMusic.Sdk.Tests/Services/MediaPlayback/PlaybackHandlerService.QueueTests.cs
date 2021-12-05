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
    public class PlaybackHandlerServiceQueueTests
    {
        private PlaybackHandlerService _handlerService;
        private List<IMediaSourceConfig> _previousItems;
        private List<IMediaSourceConfig> _nextItems;

        [TestInitialize]
        public void Setup()
        {
            var audioPlayerRegistry = new Dictionary<string, IAudioPlayerService>()
            {
                { "MockCore", new MockAudioPlayerService() }
            };

            _handlerService = new PlaybackHandlerService(audioPlayerRegistry.First().Value, audioPlayerRegistry);

            _previousItems = new List<IMediaSourceConfig>();
            _nextItems = new List<IMediaSourceConfig>();
        }

        [TestCleanup]
        public void Teardown()
        {
            _handlerService.Dispose();

            _previousItems.Clear();
            _nextItems.Clear();

            _previousItems = null;
            _nextItems = null;
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
        public async Task Shuffle_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            Assert.AreEqual(_previousItems.Count, 0);
            Assert.AreEqual(_nextItems.Count, 0);

            // Generate previous items
            for (int i = 0; i < numberOfPreviousItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(new MockCoreTrack(), i.ToString(), null, "mp3");
                _previousItems.Add(mediaSourceConfig);
                _handlerService.PushPrevious(mediaSourceConfig);
            }

            // Generate next items
            for (int i = 0; i < numberOfNextItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(new MockCoreTrack(), (numberOfPreviousItems + i).ToString(), null, "mp3");
                _nextItems.Add(mediaSourceConfig);
                _handlerService.InsertNext(i, mediaSourceConfig);
            }

            Assert.IsFalse(_handlerService.ShuffleState);

            // Shuffle is on.
            await _handlerService.ToggleShuffleAsync();

            // Make sure previous items are emptied.
            Assert.AreEqual(_handlerService.PreviousItems.Count, 0);

            // Maximum number of items that can be at the same place.
            int shuffleTolerence = 4;
            int numberOfElementsOnSameIndex = 0;

            for (int i = 0; i < _handlerService.NextItems.Count; i++)
            {
                if (_nextItems.Count <= i)
                    continue;

                string originalNext = _nextItems[i].Id;
                if (originalNext == _handlerService.NextItems[i].Id)
                {
                    numberOfElementsOnSameIndex++;
                }

                // Make sure it shuffles "enough", and x items don't end up in the same place again.
                Assert.AreNotEqual(numberOfElementsOnSameIndex, shuffleTolerence);
            }

            // Make sure all items from previous and next exist in next after shuffle.
            CollectionAssert.IsSubsetOf(_nextItems, _handlerService.NextItems.ToList());
            CollectionAssert.IsSubsetOf(_previousItems, _handlerService.NextItems.ToList());
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
        public async Task ShuffleAndUnshuffle_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            Assert.IsTrue(_handlerService.ShuffleState);

            // Turn shuffle off.
            await _handlerService.ToggleShuffleAsync();

            CollectionAssert.AreEqual(_nextItems, _handlerService.NextItems.ToList());
            CollectionAssert.AreEqual(_previousItems, _handlerService.PreviousItems.ToList());
        }
    }
}
