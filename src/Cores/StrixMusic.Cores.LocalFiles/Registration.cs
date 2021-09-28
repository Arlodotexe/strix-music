using System;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.LocalFiles
{
    public static class Registration
    {
        static Registration()
        {
            var metadata = new CoreMetadata(
                    id: nameof(LocalFilesCore),
                    displayName: "Local Files",
                    logoUri: new Uri("ms-appx:///Assets/Cores/LocalFiles/Logo.svg"));

            CoreRegistry.Register(coreFactory: instanceId => new LocalFilesCore(instanceId), metadata);
        }
    }
}
