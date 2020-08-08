using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <inheritdoc/>
    public class BindablePlayableCollection : ObservableObject, IPlayableCollectionBase
    {
        private readonly IPlayableCollectionBase _playableCollectionBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindablePlayableCollection"/> class.
        /// </summary>
        public BindablePlayableCollection(IPlayableCollectionBase playableCollectionBase)
        {
            _playableCollectionBase = playableCollectionBase;
        }

        /// <inheritdoc/>
        public string Id => _playableCollectionBase.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _playableCollectionBase.SourceCore;

        /// <inheritdoc/>
        public string Name => _playableCollectionBase.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _playableCollectionBase.Images;

        /// <inheritdoc/>
        public Uri Url => _playableCollectionBase.Url;

        /// <inheritdoc/>
        public string? Description => _playableCollectionBase.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _playableCollectionBase.Owner;

        /// <inheritdoc/>
        public PlaybackState State => _playableCollectionBase.State;

        /// <inheritdoc/>
        public ITrack? PlayingTrack => _playableCollectionBase.PlayingTrack;

        /// <inheritdoc/>
        public void Pause()
        {
            _playableCollectionBase.Pause();
        }

        /// <inheritdoc/>
        public void Play()
        {
            _playableCollectionBase.Play();
        }
    }
}
