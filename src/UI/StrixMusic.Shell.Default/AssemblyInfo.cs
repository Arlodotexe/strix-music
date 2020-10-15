using StrixMusic.Shell.Default;
using StrixMusic.Shell.Default.Assembly;

[assembly: Shell(
    shellClass: typeof(DefaultShell),
    displayName: "Default Shell")
]