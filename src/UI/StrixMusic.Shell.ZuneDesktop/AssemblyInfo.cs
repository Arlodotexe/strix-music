using StrixMusic.Shell.Default.Assembly;
using StrixMusic.Shell.Default.Assembly.Enums;
using StrixMusic.Shell.ZuneDesktop.Controls;

[assembly: Shell(
    shellClass: typeof(ZuneShell),
    displayName: "Zune Desktop",
    deviceFamily: DeviceFamily.Desktop,
    inputMethod: InputMethod.Mouse,
    minWidth: 700,
    minHeight: 600)
]
