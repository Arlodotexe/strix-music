using System;
using System.Collections.Generic;
using System.Text;

namespace CoreInterfaces.Enums
{
    /// <summary>
    /// Current state the track is in.
    /// </summary>
    public enum TrackState
    {
        Unloaded,
        Queued,
        Playing,
        Paused,
    }
}
