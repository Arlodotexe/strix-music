// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Contains a history of playable items which were selected from search results.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ISearchHistory : ISearchHistoryBase, IPlayableCollectionGroup, IPlayable, ISdkMember, IMerged<ICoreSearchHistory>
    {
    }
}
