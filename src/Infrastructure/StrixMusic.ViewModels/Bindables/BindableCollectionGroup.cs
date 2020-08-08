using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class BindableCollectionGroup : ObservableObject, IPlayableCollectionGroup
    {
        /// <summary>
        /// An external evaluator that returns a key, used for grouping multiple collection groups.
        /// </summary>
        private readonly Func<IPlayableCollectionBase, string>? _decideItemCombine;

        /// <summary>
        /// Backing field for the items. Is modified with merged items from all inputted collections.
        /// </summary>
        private readonly IList<IPlayableCollectionBase> _playableCollections = new List<IPlayableCollectionBase>();

        private readonly IPlayableCollectionGroup _collectionBase;
        private readonly Dictionary<string, List<IPlayableCollectionBase>> _sortedPlayableCollections;
        private int _totalItemsCount = 0;

        /// <summary>
        /// The sorted playable collections.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<IPlayableCollectionBase>> SortedPlayableCollections => (IReadOnlyDictionary<string, IReadOnlyList<IPlayableCollectionBase>>)_sortedPlayableCollections;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionBase">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        /// <param name="decideItemCombine">When called, return an item from <see cref="IPlayableCollectionBase"/> to use as a key when grouping items.</param>
        public BindableCollectionGroup(IPlayableCollectionBase collectionBase, Func<IPlayableCollectionBase, string> decideItemCombine)
        {
            _sortedPlayableCollections = new Dictionary<string, List<IPlayableCollectionBase>>();
            _decideItemCombine = decideItemCombine;

            _collectionBase = (IPlayableCollectionGroup)collectionBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionBase">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public BindableCollectionGroup(IPlayableCollectionBase collectionBase)
        {
            _sortedPlayableCollections = new Dictionary<string, List<IPlayableCollectionBase>>();

            _collectionBase = (IPlayableCollectionGroup)collectionBase;
        }

        private void MergeCollectionGroups(IPlayableCollectionBase[] collections, Func<IPlayableCollectionBase, string>? decideItemCombine)
        {
            _totalItemsCount += collections.Count();

            // Sort and combine the collections.
            foreach (var collection in collections)
            {
                // Externally defined value to sort by. If null, no sorting is done and they all land under the same key.
                var sortingKey = decideItemCombine?.Invoke(collection) ?? string.Empty;

                if (_sortedPlayableCollections.TryGetValue(sortingKey, out List<IPlayableCollectionBase> value))
                    value.Add(collection);  
                else
                    _sortedPlayableCollections.Add(sortingKey, new List<IPlayableCollectionBase>() { collection });
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionBase> Items => _collectionBase.SubItems;

        /// <inheritdoc/>
        public string Id => _collectionBase.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _collectionBase.SourceCore;

        /// <inheritdoc/>
        public string Name => _collectionBase.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _collectionBase.Images;

        /// <inheritdoc/>
        public Uri Url => _collectionBase.Url;

        /// <inheritdoc/>
        public string? Description => _collectionBase.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _collectionBase.Owner;

        /// <inheritdoc/>
        public PlaybackState State => _collectionBase.State;

        /// <inheritdoc/>
        public ITrack? PlayingTrack => _collectionBase.PlayingTrack;

        /// <inheritdoc/>
        public int TotalItemsCount => _totalItemsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup>? MergedFrom => _collectionBase.MergedFrom;

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> SubItems => _collectionBase.SubItems;

        /// <inheritdoc/>
        public IReadOnlyList<IPlaylist> Playlists => _collectionBase.Playlists;

        /// <inheritdoc/>
        public int TotalPlaylistCount => _collectionBase.TotalPlaylistCount;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _collectionBase.Tracks;

        /// <inheritdoc/>
        public int TotalTracksCount => _collectionBase.TotalTracksCount;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _collectionBase.Albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _collectionBase.TotalAlbumsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _collectionBase.Artists;

        /// <inheritdoc/>
        public int TotalArtistsCount => _collectionBase.TotalArtistsCount;

        /// <inheritdoc/>
        public void Pause()
        {
            _collectionBase.Pause();
        }

        /// <inheritdoc/>
        public void Play()
        {
            _collectionBase.Play();
        }

        /// <inheritdoc/>
        public Task PopulateItems(int limit, int offset)
        {

            for (int i = offset; i < limit; i++)
            {

            }

            foreach (var item in SortedPlayableCollections)
            {

            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PopulatePlaylists(int limit, int offset = 0)
        {
            return _collectionBase.PopulatePlaylists(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateTracks(int limit, int offset = 0)
        {
            return _collectionBase.PopulateTracks(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateAlbums(int limit, int offset = 0)
        {
            return _collectionBase.PopulateAlbums(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateArtists(int limit, int offset = 0)
        {
            return _collectionBase.PopulateArtists(limit, offset);
        }
    }
}
