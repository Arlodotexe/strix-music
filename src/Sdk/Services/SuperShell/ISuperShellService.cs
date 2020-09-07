using System;

namespace StrixMusic.Sdk.Services.SuperShell
{
    /// <summary>
    /// Interacts with the app's SuperShell, that overlays the entire app.
    /// </summary>
    public interface ISuperShellService
    {
        /// <summary>
        /// Unhides the SuperShell and displays the specified view.
        /// </summary>
        /// <param name="shellDisplay">A <see cref="SuperShellDisplay"/> specifying which view will be shown.</param>
        void Show(SuperShellDisplay shellDisplay);

        /// <summary>
        /// Hides the SuperShell.
        /// </summary>
        void Hide();

        /// <summary>
        /// Occurs when the SuperShell is shown or hidden.
        /// </summary>
        event EventHandler<SuperShellDisplay> VisibilityChanged;
    }
}
