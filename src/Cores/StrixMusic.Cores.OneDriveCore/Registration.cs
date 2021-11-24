using System;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive
{
    public static class Registration
    {
        static Registration()
        {
            Metadata = new CoreMetadata(id: nameof(OneDriveCore),
                                        displayName: "OneDrive",
                                        logoUri: new Uri("ms-appx:///Assets/Cores/OneDrive/Logo.svg"));
        }

        public static CoreMetadata Metadata { get; }

        /// <summary>
        /// Executes core registration.
        /// </summary>
        public static void Execute()
        {
            CoreRegistry.Register(coreFactory: instanceId => new OneDriveCore(instanceId), Metadata);
        }
    }
}
