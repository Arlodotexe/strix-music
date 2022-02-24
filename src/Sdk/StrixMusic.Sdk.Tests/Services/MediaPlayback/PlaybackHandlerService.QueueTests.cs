using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Tests.Mock.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    [TestClass]
    public class PlaybackHandlerServiceQueueTests
    {
        private PlaybackHandlerService? _handlerService;
        private IAudioPlayerService? _audioPlayer;
        private List<PlaybackItem>? _previousItems;
        private List<PlaybackItem>? _nextItems;
        private PlaybackItem? _currentItem;
        private MockCoreTrack? _mockTrack;

        [TestInitialize]
        public void Setup()
        {
            _audioPlayer = new MockAudioPlayerService();
            _handlerService = new PlaybackHandlerService();

            var mockCore = new MockCore();

            var mockTrack = new MockCoreTrack(mockCore, string.Empty, string.Empty);
            _handlerService.RegisterAudioPlayer(_audioPlayer, mockCore.InstanceId);
            _mockTrack = mockTrack;

            _previousItems = new List<PlaybackItem>();
            _nextItems = new List<PlaybackItem>();
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
            Assert.IsNotNull(_audioPlayer);
            Assert.IsNotNull(_mockTrack);
            Assert.AreEqual(_previousItems.Count, 0);
            Assert.AreEqual(_nextItems.Count, 0);

            // Generate previous items
            for (int i = 0; i < numberOfPreviousItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(_mockTrack, i.ToString(), Stream.Null, "mp3");

                var playbackItem = GetPlaybackItem(mediaSourceConfig);

                _previousItems.Add(playbackItem);
                _handlerService.PushPrevious(playbackItem);
            }

            // If there is anything in PreviousItems, there must be a CurrentItem.
            if (numberOfPreviousItems > 0)
            {
                var playbackItem = GetPlaybackItem(new MediaSourceConfig(_mockTrack, numberOfPreviousItems.ToString(), Stream.Null, "mp3"));

                _handlerService.CurrentItem = playbackItem;
                _audioPlayer.CurrentSource = _handlerService.CurrentItem;
                _currentItem = _handlerService.CurrentItem;
            }

            // Generate next items
            for (int i = 0; i < numberOfNextItems; i++)
            {
                var mediaSourceConfig = new MediaSourceConfig(_mockTrack, (numberOfPreviousItems + (i + 1)).ToString(), Stream.Null, "mp3");

                var playbackItem = GetPlaybackItem(mediaSourceConfig);

                _nextItems.Add(playbackItem);
                _handlerService.InsertNext(i, playbackItem);
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
        public async Task ShuffleAndUnshuffleWithForward_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);

            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            Assert.IsTrue(_handlerService.ShuffleState);

            Assert.AreEqual(_handlerService.NextItems.Count, numberOfPreviousItems + numberOfNextItems);
            Assert.AreEqual(_handlerService.PreviousItems.Count, 0);

            // Trigger manual forwarding for queue.
            await _handlerService.NextAsync();

            Assert.AreEqual(_handlerService.NextItems.Count, numberOfPreviousItems + numberOfNextItems - 1);
            Assert.AreEqual(_handlerService.PreviousItems.Count, 1);

            // Turn shuffle off.
            await _handlerService.ToggleShuffleAsync();

            // The next items and previous items COUNT is not necesserily equal to original prev and next items because we changed the currentItem.
            // Original Order.
            var unshuffledList = new List<PlaybackItem?>();
            unshuffledList.AddRange(_previousItems);
            unshuffledList.Add(_currentItem);
            unshuffledList.AddRange(_nextItems);

            // Processed Unshuffled list.
            var newUnShuffledList = new List<PlaybackItem?>();
            newUnShuffledList.AddRange(_handlerService.PreviousItems);
            newUnShuffledList.Add(_handlerService.CurrentItem);
            newUnShuffledList.AddRange(_handlerService.NextItems);

            CollectionAssert.AreEqual(unshuffledList, newUnShuffledList);

            CollectionAssert.AllItemsAreNotNull(_handlerService.NextItems.ToList());
            CollectionAssert.AllItemsAreNotNull(_handlerService.PreviousItems.ToList());
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
        public async Task ShuffleAndUnshuffleWithForwardAndBackward_Queue(int numberOfPreviousItems, int numberOfNextItems)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);

            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            Assert.IsTrue(_handlerService.ShuffleState);

            // Trigger manual forwarding for queue.

            Assert.AreEqual(_handlerService.NextItems.Count, numberOfPreviousItems + numberOfNextItems);
            Assert.AreEqual(_handlerService.PreviousItems.Count, 0);

            await _handlerService.NextAsync();

            Assert.AreEqual(_handlerService.NextItems.Count, numberOfPreviousItems + numberOfNextItems - 1);
            Assert.AreEqual(_handlerService.PreviousItems.Count, 1);

            // Trigger manual rewinding for queue.
            await _handlerService.PreviousAsync();

            Assert.AreEqual(_handlerService.NextItems.Count, numberOfPreviousItems + numberOfNextItems);
            Assert.AreEqual(_handlerService.PreviousItems.Count, 0);

            // Turn shuffle off.
            await _handlerService.ToggleShuffleAsync();

            // The next items and previous items COUNT is not necesserily equal to original prev and next items because we changed the currentItem.
            // Original Order.
            var unshuffledList = new List<PlaybackItem?>();
            unshuffledList.AddRange(_previousItems);
            unshuffledList.Add(_currentItem);
            unshuffledList.AddRange(_nextItems);

            // Processed Unshuffled list.
            var newUnShuffledList = new List<PlaybackItem?>();
            newUnShuffledList.AddRange(_handlerService.PreviousItems);
            newUnShuffledList.Add(_handlerService.CurrentItem);
            newUnShuffledList.AddRange(_handlerService.NextItems);

            CollectionAssert.AreEqual(unshuffledList, newUnShuffledList);

            CollectionAssert.AllItemsAreNotNull(_handlerService.NextItems.ToList());
            CollectionAssert.AllItemsAreNotNull(_handlerService.PreviousItems.ToList());
        }


        [TestMethod]
        [DataRow(1, 10, 8)]
        [DataRow(2, 9, 3)]
        [DataRow(3, 8, 3, 4)]
        [DataRow(4, 7, 3, 3)]
        [DataRow(5, 6, 8)]
        [DataRow(6, 5, 2, 4)]
        [DataRow(7, 4, 2, 7)]
        [DataRow(8, 3, 6, 9, 1)]
        [DataRow(9, 2, 4)]
        [DataRow(10, 1, 3)]
        [Timeout(800)]
        public async Task ShuffleAndUnshuffleByAddingNewTracks(int numberOfPreviousItems, int numberOfNextItems, params int[] nextIndexes)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);
            Assert.IsNotNull(_mockTrack);

            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            Assert.IsTrue(_handlerService.ShuffleState);
            var newItems = new List<PlaybackItem>();

            for (int i = 0; i < nextIndexes.Length; i++)
            {
                var itemToAdd = new MediaSourceConfig(_mockTrack, $"New Item: {nextIndexes[i]}", Stream.Null, "mp3");

                var playbackItem = GetPlaybackItem(itemToAdd);

                _handlerService.InsertNext(nextIndexes[i], playbackItem);
                newItems.Add(playbackItem);
            }

            // Turn shuffle off.
            await _handlerService.ToggleShuffleAsync();

            var newNextItems = _handlerService.NextItems.ToList();
            var newPrevItems = _handlerService.PreviousItems.ToList();

            // Remove the newly added items before comparing if present in previous or next items.
            foreach (var item in newItems)
            {
                newNextItems.Remove(item);
                newPrevItems.Remove(item);
            }

            CollectionAssert.AreEqual(_nextItems, newNextItems);
            CollectionAssert.AreEqual(_previousItems, newPrevItems);

            CollectionAssert.AllItemsAreNotNull(newNextItems);
            CollectionAssert.AllItemsAreNotNull(newPrevItems);
        }



        [TestMethod]
        [DataRow(1, 10, 8)]
        [DataRow(2, 9, 3)]
        [DataRow(3, 8, 3, 4)]
        [DataRow(4, 7, 3)]
        [DataRow(5, 6, 8)]
        [DataRow(6, 5, 2, 4)]
        [DataRow(7, 4, 2, 7)]
        [DataRow(8, 3, 6, 9, 1)]
        [DataRow(9, 2, 4)]
        [DataRow(10, 1, 3)]
        [Timeout(800)]
        public async Task ShuffleAndUnshuffleByRemovingTracks(int numberOfPreviousItems, int numberOfNextItems, params int[] nextIndexes)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);

            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);
            var itemsRemoved = new List<PlaybackItem>();

            for (int i = 0; i < nextIndexes.Length; i++)
            {
                itemsRemoved.Add(_handlerService.NextItems.ElementAt(nextIndexes[i]));
                _handlerService.RemoveNext(nextIndexes[i]);
            }

            Assert.IsTrue(_handlerService.ShuffleState);

            // Turn shuffle off.
            await _handlerService.ToggleShuffleAsync();

            // Remove the removed items from the orginal previous and next items(if exists).
            foreach (var item in itemsRemoved)
            {
                _nextItems.Remove(item);
                _previousItems.Remove(item);
            }

            CollectionAssert.AreEqual(_nextItems, _handlerService.NextItems.ToList());
            CollectionAssert.AreEqual(_previousItems, _handlerService.PreviousItems.ToList());

            CollectionAssert.AllItemsAreNotNull(_handlerService.NextItems.ToList());
            CollectionAssert.AllItemsAreNotNull(_handlerService.PreviousItems.ToList());
        }

        [TestMethod]
        [DataRow(1, 10, 8, 6)]
        [DataRow(2, 9, 3, 6)]
        [DataRow(3, 8, 2, 4)]
        [DataRow(4, 7, 3, 1)]
        [DataRow(5, 6, 8, 9)]
        [DataRow(6, 5, 2, 4)]
        [DataRow(7, 4, 2, 7)]
        [DataRow(8, 3, 6, 9)]
        [DataRow(9, 2, 4, 8)]
        [DataRow(10, 1, 3, 6)]
        [Timeout(800)]
        // AKA: Swapping of items test.
        public async Task ShuffleRemoveItemAddItemThenUnshuffle(int numberOfPreviousItems, int numberOfNextItems, int oldIndex, int newIndex)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);

            // Shuffle.
            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            Assert.IsTrue(_handlerService.ShuffleState);

            // Removing an item.
            var itemToSwap = _handlerService.NextItems.ElementAt(oldIndex);
            _handlerService.RemoveNext(oldIndex);

            // Remove the removed item from the orginal previous and next items(if exists).
            _nextItems.Remove(itemToSwap);
            _previousItems.Remove(itemToSwap);

            // Adding an item.
            _handlerService.InsertNext(newIndex, itemToSwap);

            // Unshuffle.
            await _handlerService.ToggleShuffleAsync();

            var newNextItems = _handlerService.NextItems.ToList();
            var newPrevItems = _handlerService.PreviousItems.ToList();

            // Syncing original next and previous items with the new next and previous items.
            if (newNextItems.Contains(itemToSwap))
            {
                _nextItems.InsertOrAdd(newNextItems.IndexOf(itemToSwap), itemToSwap);
            }
            else if (newPrevItems.Contains(itemToSwap))
            {
                _previousItems.InsertOrAdd(newPrevItems.IndexOf(itemToSwap), itemToSwap);
            }

            CollectionAssert.AreEqual(newNextItems, _handlerService.NextItems.ToList());
            CollectionAssert.AreEqual(newPrevItems, _handlerService.PreviousItems.ToList());

            CollectionAssert.AllItemsAreNotNull(_handlerService.NextItems.ToList());
            CollectionAssert.AllItemsAreNotNull(_handlerService.PreviousItems.ToList());
        }

        [TestMethod]
        [DataRow(1, 10, 8)]
        [DataRow(2, 9, 3)]
        [DataRow(3, 8, 2)]
        [DataRow(4, 7, 3)]
        [DataRow(5, 6, 8)]
        [DataRow(6, 5, 2)]
        [DataRow(7, 4, 2)]
        [DataRow(8, 3, 6)]
        [DataRow(9, 2, 4)]
        [DataRow(10, 1, 3)]
        [Timeout(800)]
        public async Task ShuffleAddItemRemoveItemThenUnshuffle(int numberOfPreviousItems, int numberOfNextItems, int addIndex)
        {
            Assert.IsNotNull(_previousItems);
            Assert.IsNotNull(_nextItems);
            Assert.IsNotNull(_handlerService);
            Assert.IsNotNull(_mockTrack);

            // Shuffle.
            await Shuffle_Queue(numberOfPreviousItems, numberOfNextItems);

            var itemToAdd = new MediaSourceConfig(_mockTrack, $"New Item: {addIndex}", Stream.Null, "mp3");

            Assert.IsTrue(_handlerService.ShuffleState);

            var playbackItem = GetPlaybackItem(itemToAdd);

            // Adding an item.
            _handlerService.InsertNext(addIndex, playbackItem);

            // Removing an item.
            _handlerService.RemoveNext(addIndex);

            // Unshuffle.
            await _handlerService.ToggleShuffleAsync();


            CollectionAssert.AreEqual(_nextItems, _handlerService.NextItems.ToList());
            CollectionAssert.AreEqual(_previousItems, _handlerService.PreviousItems.ToList());

            CollectionAssert.AllItemsAreNotNull(_handlerService.NextItems.ToList());
            CollectionAssert.AllItemsAreNotNull(_handlerService.PreviousItems.ToList());
        }


        private PlaybackItem GetPlaybackItem(MediaSourceConfig mediaSourceConfig)
        {
            Assert.IsNotNull(_mockTrack, nameof(_mockTrack));

            return new PlaybackItem()
            {
                MediaConfig = mediaSourceConfig,
                Track = null,
            };
        }
    }
}
