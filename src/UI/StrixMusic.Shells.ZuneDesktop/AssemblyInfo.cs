using StrixMusic.Shells.ZuneDesktop;
using StrixMusic.Shells.Assembly;
using StrixMusic.Shells.Assembly.Enums;

[assembly: Shell(
    shellClass: typeof(ZuneShell),
    displayName: "Zune Desktop",
    deviceFamily: DeviceFamily.Desktop,
    inputMethod: InputMethod.Mouse,
    minWidth: 700,
    minHeight: 600)
]
