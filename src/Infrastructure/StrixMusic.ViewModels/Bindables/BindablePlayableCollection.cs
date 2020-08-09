using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public Uri? Url => _playableCollectionBase.Url;

        /// <inheritdoc/>
        public string? Description => _playableCollectionBase.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _playableCollectionBase.Owner;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _playableCollectionBase.PlaybackState;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _playableCollectionBase.PlaybackStateChanged += value;
            }

            remove
            {
                _playableCollectionBase.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _playableCollectionBase.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _playableCollectionBase.PlayAsync();
        }
    }
}
