using System;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.LocalFiles
{
    public static class Registration
    {
        static Registration()
        {
            Metadata = new CoreMetadata(id: nameof(LocalFilesCore),
                                        displayName: "Local Files",
                                        logoUri: new Uri("ms-appx:///Assets/Cores/LocalFiles/Logo.svg"),
                                        sdkVersion: Version.Parse("0.0.0.0"));
        }

        public static CoreMetadata Metadata { get; }

        /// <summary>
        /// Executes core registration.
        /// </summary>
        public static void Execute()
        {
            CoreRegistry.Register(coreFactory: instanceId => new LocalFilesCore(instanceId), Metadata);
        }
    }
}
