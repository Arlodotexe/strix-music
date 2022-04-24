using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Available sorting states for Zune Collection View.
    /// </summary>
    public enum ZuneSortState
    {
        // NOTE: MORE STATES TO BE ADDED.

        /// <summary>
        /// Alphabetical sorting in ascending order.
        /// </summary>
        AZ,

        /// <summary>
        /// Alphabetical sorting in descending order.
        /// </summary>
        ZA,

        /// <summary>
        /// Groups the collection by artists.
        /// </summary>
        Artists,

        /// <summary>
        /// Groups the collection by release year.
        /// </summary>
        ReleaseYear,

        /// <summary>
        /// Sorts the collection by added date.
        /// </summary>
        DateAdded,
    }
}
