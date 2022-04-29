using System;
using Windows.Storage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Services;

namespace StrixMusic.Controls;

/// <summary>
/// An AbstractUI panel for configuring the logging settings.
/// </summary>
public class LoggingSettingsPanel : AbstractUICollection, IDisposable
{
    private readonly AppSettings _settings;
    private readonly AbstractButton _openLogFolderButton;
    private readonly AbstractBoolean _loggingToggle;

    /// <summary>
    /// Creates a new instance of <see cref="LoggingSettingsPanel"/>.
    /// </summary>
    public LoggingSettingsPanel(AppSettings settings)
        : base(nameof(LoggingSettingsPanel))
    {
        Title = "App recovery";

        _settings = settings;
        _openLogFolderButton = new AbstractButton("openLogFolder", "View logs");
        _loggingToggle = new AbstractBoolean("loggingToggle", "Use logging")
        {
            Subtitle = "Requires restart. When enabled, the app will save debug information to disk while running.",
            State = settings.IsLoggingEnabled,
        };

        Add(_loggingToggle);
        Add(_openLogFolderButton);
        
        AttachEvents();
    }

    private void AttachEvents()
    {
        _openLogFolderButton.Clicked += OpenLogFolderButton_Clicked;
        _loggingToggle.StateChanged += LoggingToggle_StateChanged;
    }

    private void DetachEvents()
    {
        _openLogFolderButton.Clicked -= OpenLogFolderButton_Clicked;
        _loggingToggle.StateChanged -= LoggingToggle_StateChanged;
    }

    private async void LoggingToggle_StateChanged(object? sender, bool e)
    {
        _settings.IsLoggingEnabled = e;
        await _settings.SaveAsync();
    }

    private async void OpenLogFolderButton_Clicked(object? sender, EventArgs e)
    {
        var logFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);
        await Windows.System.Launcher.LaunchFolderAsync(logFolder).AsTask();
    }

    /// <inheritdoc/>
    public void Dispose() => DetachEvents();
}
