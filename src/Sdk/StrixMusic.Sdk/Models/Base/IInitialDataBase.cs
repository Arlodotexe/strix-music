// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Used as a common interface for data that is created in the UI.
    /// </summary>
    public interface IInitialDataBase : ICollectionItemBase, IAsyncDisposable
    {
    }
}