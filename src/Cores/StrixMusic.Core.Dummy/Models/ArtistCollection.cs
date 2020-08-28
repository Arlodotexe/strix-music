using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.Core.Dummy.Implementations;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Models
{
    /// <summary>
    /// A generic list of <see cref="DummyArtist"/>
    /// </summary>
    public class ArtistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistCollection"/> class.
        /// </summary>
        /// <param name="tracks">The <see cref="DummyArtist"/>s in the <see cref="TrackCollection"/>.</param>
        /// <param name="core">The <see cref="DummyCore"/>.</param>
        public ArtistCollection(List<DummyArtist> artists, DummyCore core)
        {
            SourceCore = core;
            Items = artists;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionBase> Items { get; }

        /// <inheritdoc/>
        public string Id => string.Empty;

        /// <inheritdoc/>
        public string Name => "Artists";

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile? Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITrack? PlayingTrack => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalItemsCount { get => Items?.Count() ?? 0; set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public void Play()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                UrlChanged += value;
            }

            remove
            {
                UrlChanged -= value;
            }
        }
    }
}
