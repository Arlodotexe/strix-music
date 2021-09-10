using System;
using OwlCore.AbstractStorage;

namespace OwlCore.Services.AbstractUIStorageExplorers.Handlers
{
    /// <summary>
    /// Event arguments for navigation.
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Flag to determine if the back button was tapped.
        /// </summary>
        public bool BackNavigationOccurred { get; set; }

        /// <summary>
        /// The tapped folder, it will be null if back button was tapped.
        /// </summary>
        public IFolderData? TappedFolder { get; set; }
    }
}