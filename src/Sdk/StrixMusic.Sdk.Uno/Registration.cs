using StrixMusic.Sdk.Uno.Services.ShellManagement;

namespace StrixMusic.Shells.Sandbox
{
    /// <summary>
    /// Handles registration for this shell.
    /// </summary>
    public static class Registration
    {
        static Registration()
        {
            Metadata = new ShellMetadata(id: "default.sandbox",
                                         displayName: "Sandbox",
                                         description: "Used by devs to test and create default controls for other shells.");
        }

        /// <summary>
        /// Executes shell registration.
        /// </summary>
        public static void Execute()
        {
            ShellRegistry.Register(() => new Default.DefaultShell(), Metadata);
        }

        /// <summary>
        /// Registered metadata for this shell.
        /// </summary>
        public static ShellMetadata Metadata { get; }
    }
}
