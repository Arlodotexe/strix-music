using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Sdk.Uno.Assembly.Enums;
using StrixMusic.Shells.ZuneDesktop;

[assembly: Shell(
    shellClass: typeof(ZuneShell),
    displayName: "Zune Desktop",
    deviceFamily: DeviceFamily.Desktop,
    inputMethod: InputMethod.Mouse,
    minWidth: 700,
    minHeight: 600)
]
