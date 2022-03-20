// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// ViewModel for an <see cref="ICoreConfig"/>
    /// </summary>
    public sealed class CoreConfigViewModel : ObservableObject, ISdkViewModel, ICoreConfig
    {
        private readonly ICoreConfig _coreConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreConfigViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="coreConfig">The instance of <see cref="ICoreConfig"/> to wrap around for this view model.</param>
        internal CoreConfigViewModel(MainViewModel root, ICoreConfig coreConfig)
        {
            Root = root;
            _coreConfig = coreConfig;

            AbstractUIElements = new () { new(_coreConfig.AbstractUIElements) };

            AttachEvents();
        }

        private void AttachEvents() => _coreConfig.AbstractUIElementsChanged += CoreConfig_AbstractUIElementsChanged;

        private void DetachEvents() => _coreConfig.AbstractUIElementsChanged -= CoreConfig_AbstractUIElementsChanged;

        private async void CoreConfig_AbstractUIElementsChanged(object sender, EventArgs e) => await Threading.OnPrimaryThread(() =>
        {
            AbstractUIElements.Clear();
            AbstractUIElements.Add(new AbstractUICollectionViewModel(_coreConfig.AbstractUIElements));
        });

        /// <inheritdoc/>
        AbstractUICollection ICoreConfigBase.AbstractUIElements => _coreConfig.AbstractUIElements;

        /// <inheritdoc cref="ICoreConfigBase.AbstractUIElements" />
        public ObservableCollection<AbstractUICollectionViewModel> AbstractUIElements { get; }

        /// <inheritdoc/>
        public MediaPlayerType PlaybackType => _coreConfig.PlaybackType;

        /// <inheritdoc />
        public event EventHandler? AbstractUIElementsChanged;

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc/>
        public ICore SourceCore => _coreConfig.SourceCore;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _coreConfig.DisposeAsync();
        }
    }
}
