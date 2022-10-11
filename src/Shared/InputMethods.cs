using System;

namespace StrixMusic;

/// <summary>
/// The input methods supported by the shell.
/// </summary>
[Flags]
public enum InputMethods
{
    /// <summary>
    /// The shell does not support any input methods and will not be shown to the user.
    /// </summary>
    None = 0,
        
    /// <summary>
    /// The shell is optimized for use with a mouse.
    /// </summary>
    Mouse = 1,

    /// <summary>
    /// The shell is optimized for use with a touch screen.
    /// </summary>
    Touch = 2,

    /// <summary>
    /// The shell is optimized for use with a game controller.
    /// </summary>
    Controller = 4,
}
