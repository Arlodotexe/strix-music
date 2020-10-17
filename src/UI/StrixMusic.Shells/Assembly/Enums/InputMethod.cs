using System;

namespace StrixMusic.Sdk.Uno.Assembly.Enums
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
