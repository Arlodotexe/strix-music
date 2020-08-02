using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces.Enums
{
    /// <summary>
    /// Current state the playable collection is in.
    /// </summary>
    public enum PlayableCollectionState
    {
        Unloaded,
        Playing,
        Paused,
        Queued,
    }
}
