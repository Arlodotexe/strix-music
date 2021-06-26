using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Nito.AsyncEx;
using OwlCore.Events;
using OwlCore.Remoting;
using OwlCore.Remoting.Attributes;
using StrixMusic.Core.External.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.External.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public class ExternalCoreLibrary : ExternalCorePlayableCollectionGroupBase, ICoreLibrary
    {
        private IFileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public ExternalCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            MemberRemote = new MemberRemote(this, Id);
        }

        /// <inheritdoc/>
        public override async Task InitAsync()
        {
            IsInitialized = true;

            await base.InitAsync();

            AttachEvents();
        }

        private void AttachEvents()
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

        }

        private void DetachEvents()
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

        }

        /// <summary>
        /// Determines if collection base is initialized or not.
        /// </summary>
        [RemoteProperty]
        public override bool IsInitialized { get; set; }

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <summary>
        /// A remote method that exists to proxy the remote firing of <see cref="PlaylistItemsChanged"/>.
        /// </summary>
        [RemoteMethod, RemoteOptions(RemotingDirection.Inbound)]
        public void OnTrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <summary>
        /// A remote method that exists to proxy the remote firing of <see cref="PlaylistItemsChanged"/>.
        /// </summary>
        [RemoteMethod, RemoteOptions(RemotingDirection.Inbound)]
        public void OnPlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> removedItems)
        {
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
        }
    }
}
