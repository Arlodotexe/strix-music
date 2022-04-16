// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// A base class for playable collections in a core.
    /// </summary>
    public interface ICorePlayableCollection : IPlayableCollectionBase, ICoreCollection, ICoreMember
    {
    }
}
