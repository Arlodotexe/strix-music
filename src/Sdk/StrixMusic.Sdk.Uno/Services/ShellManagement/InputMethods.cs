using System;

namespace StrixMusic.Sdk.Uno.Services.ShellManagement
{
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
        /// The shell supports and is shown on devices with mouse support.
        /// </summary>
        Mouse = 1,
        
        /// <summary>
        /// The shell supports and is shown on devices with a touchscreen.
        /// </summary>
        Touch = 2,
        
        /// <summary>
        /// The shell supports and is shown on devices that use a controller.
        /// </summary>
        Controller = 4,
    }
}
