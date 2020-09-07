using System;
using StrixMusic.Sdk.Services.SuperShell;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.Services
{
    /// <inheritdoc cref="ISuperShellService"/>
    public class SuperShellService : ISuperShellService
    {
        /// <inheritdoc/>
        public event EventHandler<SuperShellDisplay>? VisibilityChanged;

        /// <inheritdoc />
        public void Hide()
        {
            VisibilityChanged?.Invoke(this, SuperShellDisplay.Hidden);
        }

        /// <inheritdoc />
        public void Show(SuperShellDisplay shellDisplay)
        {
            VisibilityChanged?.Invoke(this, shellDisplay);
        }
    }
}
