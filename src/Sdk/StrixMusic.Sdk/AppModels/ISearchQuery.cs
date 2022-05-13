// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ISearchQuery : ISearchQueryBase, IAppModel, IMerged<ICoreSearchQuery>
    {
    }
}
