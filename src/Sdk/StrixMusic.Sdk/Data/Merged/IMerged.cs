using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
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