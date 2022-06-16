// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// An item that has been merged from multiple sources.
    /// </summary>
    /// <typeparam name="T">The type that makes up this merged item.</typeparam>
    public interface IMerged<T> : IEquatable<T>, IMerged
        where T : ICoreModel
    {
        /// <summary>
        /// The sources that make up this merged item.
        /// </summary>
        IReadOnlyList<T> Sources { get; }
    }

    /// <summary>
    /// A non-generic version of <see cref="IMerged{T}"/> that provides notification support for when any of the merged sources have changed.
    /// </summary>
    /// <seealso cref="IMerged{T}"/>
    public interface IMerged
    {
        /// <summary>
        /// Raised when any of the sources have changed.
        /// </summary>
        public event EventHandler? SourcesChanged;
    }
}
