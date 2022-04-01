// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreSearchResults"/>
    /// </summary>
    public sealed class RemoteCoreSearchResults : RemoteCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreSearchResults"/>.
        /// </summary>
        public RemoteCoreSearchResults(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Search Results")
        {
        }
    }
}
