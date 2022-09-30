﻿using System.Threading;
using System;
using NLog.Config;
using NLog.Targets;
using OwlCore.Diagnostics;
using StrixMusic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace StrixMusic
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            if (e.PrelaunchActivated == false)
            {
                TryEnablePrelaunch();

                // Ensure the current window is active
                Window.Current.Activate();
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is FrameworkElement rootElement))
            {
                // Place the frame in the current Window
                Window.Current.Content = new AppFrame();
            }

            // Bi-directional language support
#if NETFX_CORE
            // https://github.com/unoplatform/uno/issues/21
            var flowDirectionSetting = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues["LayoutDirection"];
            if (flowDirectionSetting == "LTR")
            {
                Window.Current.GetAppFrame().FlowDirection = FlowDirection.LeftToRight;
            }
            else
            {
                Window.Current.GetAppFrame().FlowDirection = FlowDirection.RightToLeft;
            }
#endif
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        /// <summary>
        /// Encapsulates the call to CoreApplication.EnablePrelaunch() so that the JIT
        /// won't encounter that call (and prevent the app from running when it doesn't
        /// find it), unless this method gets called. This method should only
        /// be called when the caller determines that we are running on a system that
        /// supports CoreApplication.EnablePrelaunch().
        /// </summary>
        private void TryEnablePrelaunch()
        {
#if NETFXCORE
            Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
#endif
        }

        private static void SetupLogger()
        {
            var logPath = ApplicationData.Current.LocalCacheFolder.Path + @"\Logs\${date:format=yyyy-MM-dd}.log";

            NLog.LogManager.Configuration = CreateConfig(shouldArchive: true);

            // Event is connected for the lifetime of the application
            Logger.MessageReceived += Logger_MessageReceived;

            Logger.LogInformation("Logger initialized");

            LoggingConfiguration CreateConfig(bool shouldArchive)
            {
                var config = new LoggingConfiguration();

                var fileTarget = new FileTarget("filelog")
                {
                    FileName = logPath,
                    EnableArchiveFileCompression = shouldArchive,
                    MaxArchiveDays = 7,
                    ArchiveNumbering = ArchiveNumberingMode.Sequence,
                    ArchiveOldFileOnStartup = shouldArchive,
                    KeepFileOpen = true,
                    OpenFileCacheTimeout = 10,
                    AutoFlush = false,
                    OpenFileFlushTimeout = 10,
                    ConcurrentWrites = false,
                    CleanupFileName = false,
                    OptimizeBufferReuse = true,
                    Layout = "${message}",
                };

                config.AddTarget(fileTarget);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, "filelog");

                var debuggerTarget = new DebuggerTarget("debuggerTarget")
                {
                    OptimizeBufferReuse = true,
                    Layout = "${message}",
                };

                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, debuggerTarget);
                config.AddTarget(debuggerTarget);

                return config;
            }
        }

        private static void Logger_MessageReceived(object? sender, LoggerMessageEventArgs e)
        {
            var message = $"{DateTime.UtcNow:O} [{e.Level}] [Thread {Thread.CurrentThread.ManagedThreadId}] L{e.CallerLineNumber} {System.IO.Path.GetFileName(e.CallerFilePath)} {e.CallerMemberName} {(e.Exception is not null ? $"Exception: {e.Exception} |" : string.Empty)} {e.Message}";

            NLog.LogManager.GetLogger(string.Empty).Log(NLog.LogLevel.Info, message);
            Console.WriteLine(message);
        }
    }
}
