using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Shells.Groove;

[assembly: Shell(
    shellClass: typeof(GrooveShell),
    displayName: "Groove Music",
    description: "A faithful recreation of the Groove Music app on Windows 10")
]