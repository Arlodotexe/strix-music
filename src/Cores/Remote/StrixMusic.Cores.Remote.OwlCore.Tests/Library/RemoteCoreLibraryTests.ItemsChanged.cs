using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Plugins.CoreRemote;
using StrixMusic.Cores.Remote.OwlCore.Tests.Mock;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Cores.Remote.OwlCore.Tests
{
    public partial class RemoteCoreLibraryTests
    {
        [TestMethod]
        public async Task RemoteAlbumItemsChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreateAlbum(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICoreAlbumCollectionItem>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>>>();

            remoteClientCore.Library.AlbumItemsChanged += ClientOnChanged;
            remoteHostCore.Library.AlbumItemsChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddAlbumItemAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.AlbumItemsChanged -= ClientOnChanged;
            remoteHostCore.Library.AlbumItemsChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }

        [TestMethod]
        public async Task RemoteArtistItemsChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreateArtist(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICoreArtistCollectionItem>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>>>();

            remoteClientCore.Library.ArtistItemsChanged += ClientOnChanged;
            remoteHostCore.Library.ArtistItemsChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddArtistItemAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.ArtistItemsChanged -= ClientOnChanged;
            remoteHostCore.Library.ArtistItemsChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }

        [TestMethod]
        public async Task RemotePlaylistItemsChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreatePlaylist(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICorePlaylistCollectionItem>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>>>();

            remoteClientCore.Library.PlaylistItemsChanged += ClientOnChanged;
            remoteHostCore.Library.PlaylistItemsChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddPlaylistItemAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.PlaylistItemsChanged -= ClientOnChanged;
            remoteHostCore.Library.PlaylistItemsChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }

        [TestMethod]
        public async Task RemoteTracksChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreateTrack(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICoreTrack>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreTrack>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreTrack>>>();

            remoteClientCore.Library.TracksChanged += ClientOnChanged;
            remoteHostCore.Library.TracksChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddTrackAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.TracksChanged -= ClientOnChanged;
            remoteHostCore.Library.TracksChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }

        [TestMethod]
        public async Task RemoteImagesChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreateImage(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICoreImage>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreImage>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreImage>>>();

            remoteClientCore.Library.ImagesChanged += ClientOnChanged;
            remoteHostCore.Library.ImagesChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddImageAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.ImagesChanged -= ClientOnChanged;
            remoteHostCore.Library.ImagesChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreImage>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreImage>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreImage>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreImage>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }

        [TestMethod]
        public async Task RemoteUrlsChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreateUrl(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICoreUrl>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreUrl>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICoreUrl>>>();

            remoteClientCore.Library.UrlsChanged += ClientOnChanged;
            remoteHostCore.Library.UrlsChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddUrlAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.UrlsChanged -= ClientOnChanged;
            remoteHostCore.Library.UrlsChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreUrl>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreUrl>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICoreUrl>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICoreUrl>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }

        [TestMethod]
        public async Task RemoteChildItemsChanged()
        {
            var core = new MockCore();
            var remoteClientCore = new RemoteCore(core.InstanceId); // Set up for receiving.
            var remoteHostCore = new RemoteCore(core); // Wrap around the actual core

            await remoteClientCore.InitAsync();

            var newItem = MockCoreItemFactory.CreatePlayableCollectionGroup(remoteClientCore);

            var expectedCollectionChangedData = new CollectionChangedItem<ICorePlayableCollectionGroup>(newItem, 1);

            // Task is completed when event is emitted.
            var clientEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>>>();
            var hostEventEmittedTaskCompletionSource = new TaskCompletionSource<IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>>>();

            remoteClientCore.Library.ChildItemsChanged += ClientOnChanged;
            remoteHostCore.Library.ChildItemsChanged += HostOnChanged;

            // Simulate an item being added directly from core's backend. 
            await core.Library.AddChildAsync(expectedCollectionChangedData.Data, expectedCollectionChangedData.Index);

            var clientEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;
            var hostEmittedAddedItems = await clientEventEmittedTaskCompletionSource.Task;

            Assert.AreNotEqual(0, clientEmittedAddedItems.Count);
            Assert.AreNotEqual(0, hostEmittedAddedItems.Count);

            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), clientEmittedAddedItems, recursive: false);
            Helpers.SmartAssertEqual(expectedCollectionChangedData.IntoList(), hostEmittedAddedItems, recursive: false);

            remoteClientCore.Library.ChildItemsChanged -= ClientOnChanged;
            remoteHostCore.Library.ChildItemsChanged -= HostOnChanged;

            await core.DisposeAsync();
            await remoteHostCore.DisposeAsync();
            await remoteClientCore.DisposeAsync();

            void ClientOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> removedItems)
            {
                clientEventEmittedTaskCompletionSource.SetResult(addedItems);
            }

            void HostOnChanged(
                object sender,
                IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> addedItems,
                IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> removedItems)
            {
                hostEventEmittedTaskCompletionSource.SetResult(addedItems);
            }
        }
    }
}
