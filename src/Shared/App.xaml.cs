using Microsoft.Extensions.Logging;
using OwlCore.Storage.Uwp;
using StrixMusic.AppModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using StrixMusic.Controls;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace StrixMusic;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    private readonly ApplicationHealthManager _healthManager;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        _healthManager = new(this);
        InitializeLogging();

        this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
        this.Suspending += OnSuspending;
#endif
    }

    /// <summary>
    /// Gets the main window of the app.
    /// </summary>
    internal Window? MainWindow { get; private set; }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if DEBUG
        if (System.Diagnostics.Debugger.IsAttached)
        {
            DebugSettings.EnableFrameRateCounter = true;
        }
#endif

#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
        MainWindow = new Window();
        MainWindow.Activate();
#endif

#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
            MainWindow = new Window();
            MainWindow.Activate();
#endif

#if __WASM__
        MainWindow = Window.Current;
#endif

#if NETFX_CORE
        MainWindow = Windows.UI.Xaml.Window.Current;
#endif

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        MainWindow.Content ??= CreateMainWindowContent();

#if !(NET6_0_OR_GREATER && WINDOWS)
        if (args.PrelaunchActivated == false)
#endif
        {
            // Ensure the current window is active
            TryEnablePrelaunch();
            MainWindow.Activate();
        }
    }

    /// <summary>
    /// Creates the default content for the <see cref="MainWindow"/>.
    /// </summary>
    /// <returns></returns>
    internal UIElement CreateMainWindowContent()
    {
        AppRoot MakeAppRoot()
        {

            var appRoot = new AppRoot(new WindowsStorageFolder(ApplicationData.Current.LocalFolder))
            {
#if __WASM__
        HttpMessageHandler = new Uno.UI.Wasm.WasmHttpHandler(),
#endif
            };
            return appRoot;
        }

        if (_healthManager.State.UnhealthyShutdown)
            return new AppRecovery(_healthManager, MakeAppRoot);
        else
            return new AppFrame(MakeAppRoot());
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

        //TODO: Save application state and stop any background activity
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
#if NETFX_CORE
        Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
#endif
    }

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    private static void InitializeLogging()
    {
#if DEBUG
        // Logging is disabled by default for release builds, as it incurs a significant
        // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
        // is a concern for your application, keep this disabled. If you're running on web or 
        // desktop targets, you can use url or command line parameters to enable it.
        //
        // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ && !__MACCATALYST__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
            // builder.AddDebug();
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // RemoteControl and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });


#if HAS_UNO
        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }
}
