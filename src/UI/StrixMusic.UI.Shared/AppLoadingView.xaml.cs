using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using OwlCore.Services;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;
using StrixMusic.Sdk.Uno.Assembly;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.MediaPlayback;
using StrixMusic.Shared.Services;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The loading view used to initialize the app on startup. The user sees a splash screen, a text status indicator, and icons representing each core.
    /// </summary>
    public sealed partial class AppLoadingView : UserControl
    {
        private DefaultSettingsService? _settingsService;
        private LocalizationResourceLoader? _localizationService;
        private IPlaybackHandlerService? _playbackHandlerService;
        private IReadOnlyList<CoreAssemblyInfo>? _coreRegistry;
        private Dictionary<string, CoreAssemblyInfo>? _configuredCoreRegistry;

        /// <summary>
        /// The ViewModel for this page.
        /// </summary>
        public MainViewModel ViewModel => CurrentWindow.MainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLoadingView"/> class.
        /// </summary>
        public AppLoadingView()
        {
            this.InitializeComponent();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif
        }

        private void UpdateStatus(string text)
        {
            if (_localizationService != null)
            {
                text = _localizationService[Constants.Localization.StartupResource, text];
            }

            PART_Status.Text = text;
        }

        private void UpdateStatusRaw(string text)
        {
            PART_Status.Text = text;
        }

        private async void AppLoadingView_OnLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeServices();
            await InitializeAssemblies();
            await ManuallyRegisterCore<Core.MusicBrainz.MusicBrainzCore>("10ebf838-6a4e-4421-8fcb-c05f91fe0495");
            await ManuallyRegisterCore<Core.LocalFiles.LocalFilesCore>("10ebf138-6a4f-4421-8fcb-c15f91fe0495");
            // await ManuallyRegisterCore<Core.Apollo.ApolloCore>("10ebf838-6a4e-4421-8fcb-c05f91fe0496");
            await InitializeCoreRanking();
            await InitializeOutOfBoxSetupIfNeeded();
            await InitializeConfiguredCores();

            UpdateStatusRaw($"Done loading, navigating to {nameof(MainPage)}");
            CurrentWindow.NavigationService?.NavigateTo(typeof(MainPage));
        }

        /// <summary>
        /// Checks if we have all the data we need from the assemblies cached already.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task InitializeAssemblies()
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            Assembly[]? assemblies = null;

            // Core registry
            _coreRegistry = await Task.Run(() => _settingsService.GetValue<IReadOnlyList<CoreAssemblyInfo>>(nameof(SettingsKeys.CoreRegistry)));

            if (Equals(_coreRegistry, SettingsKeys.CoreRegistry))
            {
                assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

                // Copy assemblies to local scope, they'll go out of scope before core registry init finishes.
                var localAssemblies = assemblies;

                Task.Run(() => InitializeCoreRegistry(localAssemblies)).FireAndForget();
            }

            // Shell registry
            var shellRegistryData = await Task.Run(() => _settingsService.GetValue<IReadOnlyList<ShellAssemblyInfo>>(nameof(SettingsKeysUI.ShellRegistry)));

            if (Equals(shellRegistryData, SettingsKeysUI.ShellRegistry))
            {
                assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
                Task.Run(() => InitializeShellRegistry(assemblies)).FireAndForget();
            }
        }

        private async Task InitializeShellRegistry(Assembly[] assemblies)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            const string shellAssemblyRegex = @"^(?:StrixMusic\.Shells\.)(\w{3,})[^.]";
            var shellRegistryData = new List<ShellAssemblyInfo>();

            foreach (Assembly assembly in assemblies)
            {
                // Check if the assembly name is shell.
                var match = Regex.Match(assembly.FullName, shellAssemblyRegex);

                if (!match.Success)
                    continue;

                // Gets the AssemblyName of the shell from the Regex.
                string assemblyName = match.Groups[1].Value;

                // Find the shell attribute
                var shellAttribute = assembly.GetCustomAttribute<ShellAttribute>();

                var attributeData = new ShellAttributeData(
                    shellAttribute.ShellSubType.AssemblyQualifiedName,
                    shellAttribute.DisplayName,
                    shellAttribute.DeviceFamily,
                    shellAttribute.InputMethod,
                    shellAttribute.MaxWindowSize.Width,
                    shellAttribute.MaxWindowSize.Height,
                    shellAttribute.MinWindowSize.Width,
                    shellAttribute.MinWindowSize.Height);

                shellRegistryData.Add(new ShellAssemblyInfo(assemblyName, attributeData));
            }

            if (shellRegistryData.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException($"{nameof(shellRegistryData)} contains no elements after registry initialization. App cannot function without at least 1 shell.");
                return;
            }

            await _settingsService.SetValue<IReadOnlyList<ShellAssemblyInfo>>(nameof(SettingsKeysUI.ShellRegistry), shellRegistryData);
        }

        private async Task InitializeCoreRegistry(Assembly[] assemblies)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            // TODO: IMPORTANT
            // Need to rename all core namespaces to StrixMusic.Cores.Something, and change the below when done.

            const string shellAssemblyRegex = @"^(?:StrixMusic\.Core\.)(\w{3,})[^.]";
            var coreRegistryData = new List<CoreAssemblyInfo>();

            foreach (Assembly assembly in assemblies)
            {
                // Find the core attribute
                var coreAttribute = assembly.GetCustomAttribute<CoreAttribute>();

                if (coreAttribute is null)
                    continue;

                // Check if the namespace is for a shell.
                var match = Regex.Match(coreAttribute.CoreType.Namespace, shellAssemblyRegex);
                if (!match.Success)
                    continue;

                string assemblyName = match.Groups[1].Value;

                var attributeData = new CoreAttributeData(coreAttribute.CoreType.AssemblyQualifiedName);

                coreRegistryData.Add(new CoreAssemblyInfo(assemblyName, attributeData));
            }

            _coreRegistry = coreRegistryData;

            if (_coreRegistry.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException($"{nameof(_coreRegistry)} contains no elements after registry initialization. App cannot function without at least 1 core.");
                return;
            }

            await _settingsService.SetValue<IReadOnlyList<CoreAssemblyInfo>>(nameof(SettingsKeys.CoreRegistry), coreRegistryData);
        }

        private async Task ManuallyRegisterCore<T>(string id)
            where T : ICore
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            Guard.IsNotNull(_coreRegistry, nameof(_coreRegistry));

            Guard.HasSizeGreaterThan(_coreRegistry, 0, nameof(_coreRegistry));

            _configuredCoreRegistry ??= new Dictionary<string, CoreAssemblyInfo>()
                                        ?? await Task.Run(() => _settingsService.GetValue<Dictionary<string, CoreAssemblyInfo>>(nameof(SettingsKeys.ConfiguredCores)));

            foreach (var coreData in _coreRegistry)
            {
                var coreDataType = Type.GetType(coreData.AttributeData.CoreTypeAssemblyQualifiedName);

                if (coreDataType == typeof(T))
                {
                    _configuredCoreRegistry.Add($"{coreDataType}.{id}", coreData);
                }
            }

            await Task.Run(() => _settingsService.SetValue<Dictionary<string, CoreAssemblyInfo>>(nameof(SettingsKeys.ConfiguredCores), _configuredCoreRegistry));
        }

        private async Task InitializeCoreRanking()
        {
            Guard.IsNotNull(_configuredCoreRegistry, nameof(_configuredCoreRegistry));
            Guard.IsNotNull(_coreRegistry, nameof(_coreRegistry));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var existingCoreRanking = await Task.Run(() => _settingsService.GetValue<List<string>>(nameof(SettingsKeys.CoreRanking)));

            var coreRanking = new List<string>();

            foreach (var instanceId in existingCoreRanking)
            {
                var coreAssemblyInfo = _configuredCoreRegistry.FirstOrDefault(x => x.Key == instanceId).Value;
                if (coreAssemblyInfo is null)
                    continue;

                // If this core isn't configured anymore, don't add it to the ranking.
                if (_coreRegistry.Any(x => x.AttributeData.CoreTypeAssemblyQualifiedName == coreAssemblyInfo.AttributeData.CoreTypeAssemblyQualifiedName))
                {
                    coreRanking.Add(instanceId);
                }
            }

            // If no cores exist in ranking, initialize with all loaded cores.
            if (coreRanking.Count == 0)
            {
                // TODO: Show abstractUI and let the user rank the cores manually.
                foreach (var configuredCoreData in _configuredCoreRegistry)
                    coreRanking.Add(configuredCoreData.Key);
            }

            await _settingsService.SetValue<List<string>>(nameof(SettingsKeys.CoreRanking), coreRanking);
        }

        private async Task InitializeServices()
        {
            UpdateStatus("...");

            IServiceCollection services = new ServiceCollection();
            _localizationService = new LocalizationResourceLoader();
            _localizationService.RegisterProvider(Constants.Localization.StartupResource);

            _localizationService.RegisterProvider(Constants.Localization.SuperShellResource);
            _localizationService.RegisterProvider(Constants.Localization.CommonResource);
            _localizationService.RegisterProvider(Constants.Localization.MusicResource);

            UpdateStatus("InitServices");

            var contextualServiceLocator = new ContextualServiceLocator();

            var textStorageService = new TextStorageService();
            _settingsService = new DefaultSettingsService(textStorageService);

            var fileSystemService = new FileSystemService();
            var cacheFileSystemService = new DefaultCacheService();

            services.AddSingleton(_localizationService);
            services.AddSingleton(contextualServiceLocator);
            services.AddSingleton<ITextStorageService>(textStorageService);
            services.AddSingleton<ISettingsService>(_settingsService);
            services.AddSingleton<CacheServiceBase>(cacheFileSystemService);
            services.AddSingleton<ISharedFactory, SharedFactory>();
            services.AddSingleton<IFileSystemService>(fileSystemService);

            _playbackHandlerService = new PlaybackHandlerService();
            services.AddSingleton(_playbackHandlerService);

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            // UpdateStatus("Initializing filesystem");
            UpdateStatus("InitFilesystem");

            await fileSystemService.InitAsync();
            await cacheFileSystemService.InitAsync();
        }

        private Task InitializeOutOfBoxSetupIfNeeded()
        {
            Guard.IsNotNull(_coreRegistry, nameof(_coreRegistry));

            // Todo: If coreRegistry is empty, show out of box setup page.
            if (_coreRegistry.Count == 0)
            {
                // TODO temp out of box setup
                // show supershell
                // wait for EventAsTask (supershell closed)
            }

            return Task.CompletedTask;
        }

        private async Task InitializeConfiguredCores()
        {
            Guard.IsNotNull(_configuredCoreRegistry, nameof(_configuredCoreRegistry));
            Guard.IsNotNull(_localizationService, nameof(_localizationService));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            Guard.IsGreaterThan(_configuredCoreRegistry.Count, 0, nameof(_configuredCoreRegistry.Count));

            UpdateStatus("InitCoreReg");

            // UpdateStatus("Creating core instances");
            UpdateStatus("CreatingCores");

            var cores = await Task.Run(() => _configuredCoreRegistry.Select(x =>
            {
                var (instanceId, coreAssemblyInfo) = x;
                var coreDataType = Type.GetType(coreAssemblyInfo.AttributeData.CoreTypeAssemblyQualifiedName);

                return (ICore)Activator.CreateInstance(coreDataType, instanceId);
            }).ToList());

            //UpdateStatus("Initializing cores");
            UpdateStatus("InitCores");
            var initData = await cores.InParallel(async core =>
            {
                var services = await CreateInitialCoreServices(core);
                return (core, services);
            });

            await Task.Run(() => CurrentWindow.MainViewModel.InitializeCoresAsync(initData));

            // UpdateStatus("Setting up media players");
            UpdateStatus("SetupMedia");
            cores.ForEach(SetupMediaPlayer);
        }

        private async Task<IServiceCollection> CreateInitialCoreServices(ICore core)
        {
            var services = new ServiceCollection();
            StorageFolder rootStorageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(core.InstanceId, Windows.Storage.CreationCollisionOption.OpenIfExists);

            services.AddSingleton<IFileSystemService>(new FileSystemService(rootStorageFolder));

            return services;
        }

        private void SetupMediaPlayer(ICore core)
        {
            Guard.IsNotNull(_playbackHandlerService, nameof(_playbackHandlerService));

            if (core.CoreConfig.PreferredPlayerType == MediaPlayerType.Standard)
            {
                var mediaPlayerElement = CurrentWindow.AppFrame.MainPage.CreateMediaPlayerElement();

                _playbackHandlerService.RegisterAudioPlayer(new AudioPlayerService(mediaPlayerElement), core);
            }
        }
    }
}
