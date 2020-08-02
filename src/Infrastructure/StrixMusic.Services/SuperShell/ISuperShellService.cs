using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Services.SuperShell
{
    /// <summary>
    /// Interacts with the app's SuperShell, that overlays the entire app.
    /// </summary>
    public interface ISuperShellService
    {
        /// <summary>
        /// Unhides the SuperShell and displays the specified view.
        /// </summary>
        /// <param name="shellDisplay">A <see cref="SuperShellDisplays"/> specifying which view will be shown.</param>
        void Show(SuperShellDisplays shellDisplay);

        /// <summary>
        /// Hides the SuperShell.
        /// </summary>
        void Hide();

        /// <summary>
        /// Occurs when the SuperShell is shown or hidden.
        /// </summary>
        event EventHandler<SuperShellDisplays> VisibilityChanged;
    }
}
