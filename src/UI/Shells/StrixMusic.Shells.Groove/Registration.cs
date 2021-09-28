using StrixMusic.Sdk.Uno.Services.ShellManagement;

namespace StrixMusic.Shells.Groove
{
    /// <summary>
    /// Handles registration for this shell.
    /// </summary>
    public static class Registration
    {
        /// <summary>
        /// Executes shell registration.
        /// </summary>
        public static void Execute()
        {
            var metadata = new ShellMetadata(
                id: "GrooveMusic.10.21061.10121.0",
                displayName: "Groove Music",
                description: "A faithful recreation of the Groove Music app from Windows 10");

            ShellRegistry.Register(() => new GrooveShell(), metadata);
        }
    }
}
