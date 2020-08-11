using System;

namespace StrixMusic.Shell.Default.Assembly.Enums
{
    /// <summary>
    /// The Device family of the Shell.
    /// </summary>
    [Flags]
    public enum DeviceFamily
    {
        Desktop = 0x1,
        Mobile = 0x2,
        Console = 0x4,
    }
}
