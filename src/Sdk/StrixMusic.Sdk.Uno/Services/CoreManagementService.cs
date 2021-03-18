using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;
using Windows.Storage;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <summary>
    /// Manages added and removing core instances.
    /// </summary>
    public class CoreManagementService : ICoreManagementService
    {
        private readonly ISettingsService _settingsService;
        private Dictionary<string, CoreAssemblyInfo>? _coreInstanceRegistry;
        private IReadOnlyList<CoreAssemblyInfo>? _coreRegistry;
        private List<string>? _coreRanking;

        /// <summary>
        /// Creates a new instance of <see cref="CoreManagementService"/>.
        /// </summary>
        /// <param name="settingsService">Settings service used to store core information.</param>
        public CoreManagementService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<CoreAssemblyInfo>> GetCoreRegistryAsync()
        {
            return _settingsService.GetValue<IReadOnlyList<CoreAssemblyInfo>>(nameof(SettingsKeys.CoreRegistry));
        }

        /// <inheritdoc/>
        public Task<Dictionary<string, CoreAssemblyInfo>> GetCoreInstanceRegistryAsync()
        {
            return _settingsService.GetValue<Dictionary<string, CoreAssemblyInfo>>(nameof(SettingsKeys.CoreInstanceRegistry));
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<string>> GetCoreInstanceRanking()
        {
            return _settingsService.GetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking));
        }

        /// <inheritdoc/>
        public async Task<string> RegisterCoreInstanceAsync(CoreAssemblyInfo coreAssemblyInfo)
        {
            _coreInstanceRegistry ??= new Dictionary<string, CoreAssemblyInfo>();

            Guard.IsTrue(IsInitialized, nameof(IsInitialized));
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));
            Guard.IsNotNull(_coreRegistry, nameof(_coreRegistry));
            Guard.HasSizeGreaterThan(_coreRegistry, 0, nameof(_coreRegistry));

            foreach (var coreData in _coreRegistry)
            {
                if (coreData.AttributeData.CoreTypeAssemblyQualifiedName != coreAssemblyInfo.AttributeData.CoreTypeAssemblyQualifiedName)
                    continue;

                var coreDataType = Type.GetType(coreData.AttributeData.CoreTypeAssemblyQualifiedName);
                var guid = Guid.NewGuid();
                var instanceId = $"{coreDataType}.{guid}";

                _coreInstanceRegistry.Add(instanceId, coreData);
                _coreRanking.Add(instanceId);
                await _settingsService.SetValue<Dictionary<string, CoreAssemblyInfo>>(nameof(SettingsKeys.CoreInstanceRegistry), _coreInstanceRegistry);
                await _settingsService.SetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking), _coreRanking.AsReadOnly());

                CoreInstanceRegistered?.Invoke(this, new CoreInstanceEventArgs(instanceId, coreAssemblyInfo));
                return instanceId;
            }

            return ThrowHelper.ThrowArgumentException<string>($"Could not find core assembly {coreAssemblyInfo.AssemblyName} in the registry.");
        }

        /// <inheritdoc/>
        public async Task UnregisterCoreInstanceAsync(string instanceId)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));
            Guard.IsNotNull(_coreRegistry, nameof(_coreRegistry));
            Guard.HasSizeGreaterThan(_coreRegistry, 0, nameof(_coreRegistry));
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreInstanceRegistry.Count, 0, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreRanking.Count, 0, nameof(_coreRanking));

            if (_coreInstanceRegistry.TryGetValue(instanceId, out var value))
            {
                _coreInstanceRegistry.Remove(instanceId);
                _coreRanking.Remove(instanceId);
            }

            CoreInstanceUnregistered?.Invoke(this, new CoreInstanceEventArgs(instanceId, value));

            await _settingsService.SetValue<Dictionary<string, CoreAssemblyInfo>>(nameof(SettingsKeys.CoreInstanceRegistry), _coreInstanceRegistry);
            await _settingsService.SetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking), _coreRanking.AsReadOnly());
        }

        /// <inheritdoc/>
        public async Task<IServiceCollection> CreateInitialCoreServicesAsync(ICore core)
        {
            var services = new ServiceCollection();
            StorageFolder rootStorageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(core.InstanceId, CreationCollisionOption.OpenIfExists);

            // The same INotificationService instance should be used across all core instances.
            var notificationService = Ioc.Default.GetRequiredService<INotificationService>();
            var sharedFactory = Ioc.Default.GetRequiredService<ISharedFactory>();

            services.AddSingleton(await sharedFactory.CreateFileSystemServiceAsync(rootStorageFolder.Path));
            services.AddSingleton(notificationService);

            return services;
        }

        /// <inheritdoc/>
        public CoreAssemblyInfo GetCoreAssemblyInfoForCore(ICore core)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));

            return _coreInstanceRegistry.First(x => x.Key == core.InstanceId).Value;
        }

        /// <inheritdoc/>
        public event EventHandler<CoreInstanceEventArgs>? CoreInstanceRegistered;

        /// <inheritdoc/>
        public event EventHandler<CoreInstanceEventArgs>? CoreInstanceUnregistered;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            _coreInstanceRegistry = await GetCoreInstanceRegistryAsync();
            _coreRegistry = await GetCoreRegistryAsync();

            var ranking = await GetCoreInstanceRanking();
            _coreRanking = ranking.ToList();
        }
    }
}