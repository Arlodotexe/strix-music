using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Tests.Mock.AppModels;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Tests.ViewModels
{
    [TestClass]
    public class TrackCollectionViewModelTests
    {
        [DataRow(1), DataRow(5), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task PopulateMoreImagesAsync_PopulatesBeforeTaskCompletes(int itemCount)
        {
            SynchronizationContext.SetSynchronizationContext(new());

            var data = new Mock.AppModels.MockTrackCollection();

            foreach (var i in Enumerable.Range(0, itemCount))
                await data.AddImageAsync(new Mock.AppModels.MockImage(), i);

            var vm = new TrackCollectionViewModel(data, new MockStrixDataRoot());

            Assert.IsFalse(vm.IsInitialized);
            Assert.AreEqual(0, vm.Images.Count);

            await vm.PopulateMoreImagesAsync(itemCount);

            Assert.AreEqual(itemCount, vm.Images.Count);
        }

        [DataRow(1), DataRow(5), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task PopulateMoreUrlsAsync_PopulatesBeforeTaskCompletes(int itemCount)
        {
            SynchronizationContext.SetSynchronizationContext(new());

            var data = new Mock.AppModels.MockTrackCollection();

            foreach (var i in Enumerable.Range(0, itemCount))
                await data.AddUrlAsync(new Mock.AppModels.MockUrl(), i);

            var vm = new TrackCollectionViewModel(data, new MockStrixDataRoot());

            Assert.IsFalse(vm.IsInitialized);
            Assert.AreEqual(0, vm.Urls.Count);

            await vm.PopulateMoreUrlsAsync(itemCount);

            Assert.AreEqual(itemCount, vm.Urls.Count);
        }

        [DataRow(1), DataRow(5), DataRow(100)]
        [TestMethod, Timeout(5000)]
        public async Task PopulateMoreTracksAsync_PopulatesBeforeTaskCompletes(int itemCount)
        {
            SynchronizationContext.SetSynchronizationContext(new());

            var data = new Mock.AppModels.MockTrackCollection();

            foreach (var i in Enumerable.Range(0, itemCount))
                await data.AddTrackAsync(new Mock.AppModels.MockTrack(), i);

            var vm = new TrackCollectionViewModel(data, new MockStrixDataRoot());

            Assert.IsFalse(vm.IsInitialized);
            Assert.AreEqual(0, vm.Tracks.Count);

            await vm.PopulateMoreTracksAsync(itemCount);

            Assert.AreEqual(itemCount, vm.Tracks.Count);
        }
    }
}
