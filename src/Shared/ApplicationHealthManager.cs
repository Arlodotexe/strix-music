using System;
using System.IO;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Settings;
using Windows.Storage;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

namespace StrixMusic;

/// <summary>
/// Manages the health of an application.
/// </summary>
public class ApplicationHealthManager
{
    private readonly string _healthFileName = "managed_app_health.json";

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationHealthManager"/> class.
    /// </summary>
    /// <param name="app"></param>
    public ApplicationHealthManager(App app)
    {
        App = app;

        app.UnhandledException += AppOnUnhandledException;
        LoadState();
    }

    /// <summary>
    /// The current application state.
    /// </summary>
    public ApplicationHealthState State { get; set; } = new();

    private void AppOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var shutdownReason = e.Message;
        var shouldCrash = !State.ReasonWhitelist.Contains(shutdownReason);

        State.UnhealthyShutdownReason = shutdownReason;
        State.StackTrace = e.Exception.StackTrace;

        // Remove the stack trace from the reason message, if present.
        State.UnhealthyShutdownReason = State.UnhealthyShutdownReason.Replace(State.StackTrace ?? string.Empty, string.Empty);

        State.UnhealthyShutdown = shouldCrash;
        e.Handled = !shouldCrash;

        if (shouldCrash)
            FlushState();
    }

    /// <summary>
    /// Flushes the current state to disk.
    /// </summary>
    public void FlushState()
    {
        try
        {
            using var dataStream = AppSettingsSerializer.Singleton.Serialize(State);
            var bytes = dataStream.ToBytes();

            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, _healthFileName);
            File.WriteAllBytes(path, bytes);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

    /// <summary>
    /// Loads the flushed state form disk.
    /// </summary>
    public void LoadState()
    {
        try
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, _healthFileName);

            using var dataStream = File.OpenRead(path);

            State = AppSettingsSerializer.Singleton.Deserialize<ApplicationHealthState>(dataStream);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }

    /// <summary>
    /// The app being managed.
    /// </summary>
    public App App { get; }
}
