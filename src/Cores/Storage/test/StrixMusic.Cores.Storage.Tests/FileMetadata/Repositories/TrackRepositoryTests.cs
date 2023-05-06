using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using StrixMusic.Cores.Storage.FileMetadata.Models;
using StrixMusic.Cores.Storage.FileMetadata.Repositories;

namespace StrixMusic.Cores.Storage.Tests.FileMetadata.Repositories
{
    [TestClass]
    public class TrackRepositoryTests
    {
        [TestMethod]
        public async Task IsSortedByTrackNumber()
        {
            var repo = new TrackRepository();

            var data = Enumerable.Range(0, 100).Select(x => new TrackMetadata
            {
                Id = $"{Guid.NewGuid()}",
                Title = $"{Guid.NewGuid()}",
                TrackNumber = (uint)x,
            }).ToArray();

            data.Shuffle();

            await repo.AddOrUpdateAsync(data);

            var items = await repo.GetItemsAsync(offset: 0, limit: data.Length);
            Assert.AreNotEqual(0, items.Count());

            var expectedItemIdOrder = items.OrderBy(x => x.TrackNumber).Select(x => x.Id);
            var actualItemIdOrder = items.Select(x => x.Id);

            CollectionAssert.AreEqual(expectedItemIdOrder.ToList(), actualItemIdOrder.ToList());
        }

        [TestMethod]
        public async Task IsGroupedByDiscNumber()
        {
            var repo = new TrackRepository();

            var discOne = Enumerable.Range(0, 100).Select(x => new TrackMetadata()
            {
                Id = $"{Guid.NewGuid()}",
                Title = $"{Guid.NewGuid()}",
                TrackNumber = (uint)x,
                DiscNumber = 1,
            }).ToArray();

            var discTwo = Enumerable.Range(0, 100).Select(x => new TrackMetadata()
            {
                Id = $"{Guid.NewGuid()}",
                Title = $"{Guid.NewGuid()}",
                TrackNumber = (uint)x,
                DiscNumber = 2,
            }).ToArray();

            discOne.Shuffle();
            discTwo.Shuffle();

            await repo.AddOrUpdateAsync(discTwo);
            await repo.AddOrUpdateAsync(discOne);

            var items = await repo.GetItemsAsync(offset: 0, limit: discOne.Length);
            Assert.AreNotEqual(0, items.Count());

            var expectedItemIdOrder = items.OrderBy(x => x.TrackNumber)
                                           .GroupBy(x => x.DiscNumber)
                                           .SelectMany(x => x, (x, y) => y.Id);

            var actualItemIdOrder = items.Select(x => x.Id);

            CollectionAssert.AreEqual(expectedItemIdOrder.ToList(), actualItemIdOrder.ToList());
        }
    }
}
