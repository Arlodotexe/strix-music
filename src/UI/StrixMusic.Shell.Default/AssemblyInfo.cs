using StrixMusic.Shell.Default;
using StrixMusic.Shells.Assembly;

[assembly: Shell(
    shellClass: typeof(DefaultShell),
    displayName: "Default Shell")
]