// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// An item that has been merged.
    /// </summary>
    /// <typeparam name="T">The type that makes up this merged item.</typeparam>
    public interface IMerged<T> : IEquatable<T>
        where T : ICoreMember
    {
        /// <summary>
        /// The sources that make up this merged item.
        /// </summary>
        IReadOnlyList<T> Sources { get; }

        /// <summary>
        /// The source cores which created the parent.
        /// </summary>
        IReadOnlyList<ICore> SourceCores { get; }
    }
}
