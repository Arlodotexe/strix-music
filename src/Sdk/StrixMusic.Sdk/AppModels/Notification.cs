// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using OwlCore.AbstractUI.Models;
using OwlCore.Remoting;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// Represents a notification containing basic ui elements that can be dismissed.
    /// </summary>
    public sealed class Notification
    {
        /// <summary>
        /// Raised when the Notification is dismissed.
        /// </summary>
        public event EventHandler? Dismissed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="abstractUIElementGroup">The <see cref="AbstractUICollection"/> to display for the notification content.</param>
        public Notification(AbstractUICollection abstractUIElementGroup)
        {
            AbstractUICollection = abstractUIElementGroup;
        }

        /// <summary>
        /// The <see cref="OwlCore.AbstractUI.Models.AbstractUICollection"/> to be displayed for the notification.
        /// </summary>
        public AbstractUICollection AbstractUICollection { get; }

        /// <summary>
        /// Raises the <see cref="Dismissed"/> event.
        /// </summary>
        public void Dismiss()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
