using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces.Enums
{
    /// <summary>
    /// Describes the type of device used for playback.
    /// </summary>
    public enum DeviceType
    {
        Local,
        Remote,
        Bluetooth,
        DLNA,
        Chromecast,
    }
}
