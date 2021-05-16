using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Shells.Default;

[assembly: Shell(
    shellClass: typeof(DefaultShell),
    displayName: "Sandbox",
    description: "Used by devs to test and create default controls for other shells.")
]