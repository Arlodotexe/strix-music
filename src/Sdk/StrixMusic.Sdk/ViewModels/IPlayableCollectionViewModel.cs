using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interface for ViewModels that use <see cref="IPlayableCollectionBase" />. 
    /// </summary>
    public interface IPlayableCollectionViewModel : IPlayableCollectionBase
    {
        /// <summary>
        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }
    }
}