using System;
using OwlCore.AbstractUI.Models;
using StrixMusic.Services;

namespace StrixMusic.Controls;

/// <summary>
/// An AbstractUI panel for performing advanced app settings.
/// </summary>
public class AdvancedAppSettingsPanel : AbstractUICollection, IDisposable
{
    private readonly RecoverySettingsPanel _recoverySettings;
    private readonly LoggingSettingsPanel _loggingSettings;

    /// <summary>
    /// Creates a new instance of <see cref="AdvancedAppSettingsPanel"/>.
    /// </summary>
    public AdvancedAppSettingsPanel(AppSettings appSettings)
        : base(nameof(AdvancedAppSettingsPanel))
    {
        _recoverySettings = new RecoverySettingsPanel();
        _loggingSettings = new LoggingSettingsPanel(appSettings);

        Add(_loggingSettings);
        Add(_recoverySettings);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _recoverySettings.Dispose();
        _loggingSettings.Dispose();
    }
}
