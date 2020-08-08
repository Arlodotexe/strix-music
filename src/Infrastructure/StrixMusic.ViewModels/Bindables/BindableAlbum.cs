using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <inheritdoc/>
    public class BindableAlbum : ObservableObject, IAlbum
    {
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableAlbum"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public BindableAlbum(IAlbum album)
        {
            _album = album;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _album.Tracks;

        /// <inheritdoc/>
        public int TotalTrackCount => _album.TotalTrackCount;

        /// <inheritdoc/>
        public IArtist Artist => _album.Artist;

        /// <inheritdoc/>
        public Uri? CoverImageUri => _album.CoverImageUri;

        /// <inheritdoc/>
        public string Id => _album.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _album.SourceCore;

        /// <inheritdoc/>
        public string Name => _album.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _album.Images;

        /// <inheritdoc/>
        public Uri Url => _album.Url;

        /// <inheritdoc/>
        public string? Description => _album.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _album.Owner;

        /// <inheritdoc/>
        public PlaybackState State => _album.State;

        /// <inheritdoc/>
        public ITrack? PlayingTrack => _album.PlayingTrack;

        /// <inheritdoc/>
        public void Pause()
        {
            _album.Pause();
        }

        /// <inheritdoc/>
        public void Play()
        {
            _album.Play();
        }

        /// <inheritdoc/>
        public Task PopulateTracks(int limit, int offset = 0)
        {
            return _album.PopulateTracks(limit, offset);
        }
    }
}
