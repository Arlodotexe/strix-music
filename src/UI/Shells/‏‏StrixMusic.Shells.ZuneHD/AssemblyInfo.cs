using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Shells.ZuneHD;

[assembly: Shell(
    shellClass: typeof(ZuneHDShell),
    displayName: "ZuneHD")
]