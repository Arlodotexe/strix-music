using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Shells.Default;

[assembly: Shell(
    shellClass: typeof(DefaultShell),
    displayName: "Default",
    description: "A fallback skin. Used when something goes horribly wrong, and by devs to test default controls.")
]