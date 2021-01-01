using System.Collections.Generic;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// Properties used by all interfaces that interact with a Core in the Sdk (ViewModel, Merged items, etc).
    /// </summary>
    public interface ISdkMember<out T>
        where T : ICoreMember
    {
        /// <summary>
        /// The source cores which created the parent.
        /// </summary>
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        internal IReadOnlyList<T> Sources { get; }
    }
}