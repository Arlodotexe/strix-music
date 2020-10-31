using System.Collections.Generic;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// Properties used by all interfaces that interact with a Core in the Sdk (ViewModel, Merged items, etc).
    /// </summary>
    public interface ISdkMember
    {
        /// <summary>
        /// The source core which created the parent.
        /// </summary>
        public IEnumerable<ICore> SourceCore { get; }
    }
}