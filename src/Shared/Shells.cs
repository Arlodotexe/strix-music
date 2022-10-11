namespace StrixMusic;

/// <summary>
/// Container for metadata about shells that are available to use in this app.
/// </summary>
public static class Shells
{
    /// <summary>
    /// Metadata used to identify this shell before instantiation.
    /// </summary>
    public static ShellMetadata Sandbox { get; } =
        new(id: "default.sandbox",
            displayName: "Sandbox",
            description: "Used by devs to test and create default controls for other shells.");

    /// <summary>
    /// Metadata used to identify this shell before instantiation.
    /// </summary>
    public static ShellMetadata ZuneDesktop { get; } = new(
        id: "Zune.Desktop.4.8",
        displayName: "Zune Desktop",
        description: "A faithful recreation of the iconic Zune client for Windows",
        inputMethods: InputMethods.Mouse,
        minWindowSize: new(width: 700, height: 600)
    );
}
