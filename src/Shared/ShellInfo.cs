using System.Collections.Generic;

namespace StrixMusic;

/// <summary>
/// Container for metadata about shells that are available to use in this app.
/// </summary>
public static class ShellInfo
{
    /// <summary>
    /// The metadata for the Zune Desktop shell.
    /// </summary>
    public static ShellMetadata ZuneDesktop { get; } = new(displayName: "Zune Desktop", description: "A faithful recreation of the iconic Zune client for Windows", inputMethods: InputMethods.Mouse, minWindowSize: new(width: 700, height: 600));

    /// <summary>
    /// The metadata for the Groove Music shell.
    /// </summary>
    public static ShellMetadata GrooveMusic { get; } = new(displayName: "Groove Music", description: "A faithful recreation of the Groove Music app from Windows 10");

    /// <summary>
    /// The metadata for the sandbox shell. Used by devs to testa nd create default controls for other shells.
    /// </summary>
    public static ShellMetadata Sandbox { get; } = new(displayName: "Sandbox", description: "Used by devs to test and create default controls for other shells.");

    /// <summary>
    /// The metadata for all available shells.
    /// </summary>
    public static Dictionary<StrixMusicShells, ShellMetadata> All { get; } = new()
    {
        { StrixMusicShells.Sandbox, Sandbox },
        { StrixMusicShells.GrooveMusic, GrooveMusic },
        { StrixMusicShells.ZuneDesktop, ZuneDesktop },
    };
}
