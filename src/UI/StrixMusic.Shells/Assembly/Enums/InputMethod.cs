using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Shell.Default.Assembly.Enums
{
    /// <summary>
    /// The input methods supported by the shell.
    /// </summary>
    [Flags]
    public enum InputMethod
    {
        Mouse = 0x1,
        Touch = 0x2,
        Controller = 0x4,
    }
}
