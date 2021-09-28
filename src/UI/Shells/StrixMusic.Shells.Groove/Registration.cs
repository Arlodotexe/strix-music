using StrixMusic.Sdk.Uno.Services.ShellManagement;

namespace StrixMusic.Shell.Groove
{
    public static class Registration
    {
        static Registration()
        {
            var metadata = new ShellMetadata(
                displayName: "Groove Music",
                description: "A faithful recreation of the Groove Music app from Windows 10");

            ShellRegistry.Register(() => new GrooveShell(), metadata);
        }
    }
}
