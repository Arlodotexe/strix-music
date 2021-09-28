using System;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive
{
    public static class Registration
    {
        static Registration()
        {
            var metadata = new CoreMetadata(
                    id: nameof(OneDriveCore),
                    displayName: "OneDrive",
                    logoUri: new Uri("ms-appx:///Assets/Cores/OneDrive/Logo.svg"));

            CoreRegistry.Register(coreFactory: instanceId => new OneDriveCore(instanceId), metadata);
        }
    }
}
