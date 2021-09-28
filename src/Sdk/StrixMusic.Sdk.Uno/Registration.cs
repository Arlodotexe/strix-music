using StrixMusic.Sdk.Uno.Services.ShellManagement;
using Windows.Foundation;

namespace StrixMusic.Sdk.Uno
{
    /// <summary>
    /// Handles registration for this shell.
    /// </summary>
    public static class Registration
    {
        static Registration()
        {
            var metadata = new ShellMetadata(id: "default.sandbox",
                                             displayName: "Dev Sandbox",
                                             description: "Used by devs to test and create default controls for other shells.");

            ShellRegistry.Register(() => new StrixMusic.Shells.Default.DefaultShell(), metadata);
        }
    }
}
