// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    public interface ISearchQueryBase
    {
        /// <summary>
        /// The text that was entered to create this search entry.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// The date and time this entry was created.
        /// </summary>
        public DateTime CreatedAt { get; }
    }
}
