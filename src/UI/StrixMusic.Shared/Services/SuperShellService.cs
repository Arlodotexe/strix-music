using System;
using StrixMusic.Services.SuperShell;
using Windows.UI.Xaml;

namespace StrixMusic.Services
{
    /// <inheritdoc cref="ISuperShellService"/>
    public class SuperShellService : ISuperShellService
    {
        /// <inheritdoc/>
        public event EventHandler<SuperShellDisplays>? VisibilityChanged;

        /// <inheritdoc />
        public void Hide()
        {
            VisibilityChanged?.Invoke(this, SuperShellDisplays.Hidden);
        }

        /// <inheritdoc />
        public void Show(SuperShellDisplays shellDisplay)
        {
            VisibilityChanged?.Invoke(this, shellDisplay);
        }
    }
}
