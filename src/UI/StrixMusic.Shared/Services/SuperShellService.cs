using System;
using StrixMusic.Sdk.Services.SuperShell;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.Services
{
    /// <inheritdoc cref="ISuperShellService"/>
    public class SuperShellService : ISuperShellService
    {
        /// <inheritdoc/>
        public event EventHandler<SuperShellDisplayState>? VisibilityChanged;

        /// <inheritdoc />
        public void Hide()
        {
            VisibilityChanged?.Invoke(this, SuperShellDisplayState.Hidden);
        }

        /// <inheritdoc />
        public void Show(SuperShellDisplayState shellDisplayState)
        {
            VisibilityChanged?.Invoke(this, shellDisplayState);
        }
    }
}
