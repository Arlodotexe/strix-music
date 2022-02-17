// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Models.Base;
using System;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interface for ViewModels that use <see cref="IPlayableCollectionBase" />. 
    /// </summary>
    public interface IPlayableCollectionViewModel : ISdkViewModel, IPlayableViewModel, IPlayableCollectionBase
    {
        /// <summary>
        /// Command to change the name, if supported.
        /// </summary>
        public IAsyncRelayCommand<string> ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Command to change the description, if supported.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Command to change the duration, if supported.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }
    }
}