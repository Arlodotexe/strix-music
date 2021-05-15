using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Shells.Strix;

[assembly: Shell(
    shellClass: typeof(StrixShell),
    displayName: "Strix",
    description: "A refreshing take on conventional music app design. It's fast, fluid, and feels natural to use.")
]