using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data.Base;
using System;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interface for ViewModels that use <see cref="IPlayableCollectionBase" />. 
    /// </summary>
    public interface IPlayableCollectionViewModel : IPlayableCollectionBase
    {
        /// <summary>
        /// Command to change the name, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

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