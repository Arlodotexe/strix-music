using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Provisos;
using StrixMusic.Sdk.CoreManagement;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;
using Windows.Storage;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <summary>
    /// Manages added and removing core instances.
    /// </summary>
    public sealed class CoreManagementService : ICoreManagementService
    {
        private readonly ISettingsService _settingsService;
        private Dictionary<string, CoreMetadata>? _coreInstanceRegistry;
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
        public Task<Dictionary<string, CoreMetadata>> GetCoreInstanceRegistryAsync()
        {
            return _settingsService.GetValue<Dictionary<string, CoreMetadata>>(nameof(SettingsKeys.CoreInstanceRegistry));
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<string>> GetCoreInstanceRanking()
        {
            return _settingsService.GetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking));
        }

        /// <inheritdoc/>
        public async Task<string> RegisterCoreInstanceAsync(CoreMetadata coreMetadata)
        {
            _coreInstanceRegistry ??= new Dictionary<string, CoreMetadata>();

            Guard.IsTrue(IsInitialized, nameof(IsInitialized));
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));
            Guard.HasSizeGreaterThan(CoreRegistry.MetadataRegistry, 0, nameof(CoreRegistry.MetadataRegistry));

            foreach (var metadata in CoreRegistry.MetadataRegistry)
            {
                if (metadata.Id != coreMetadata.Id)
                    continue;

                var guid = Guid.NewGuid();
                var instanceId = $"{metadata.Id}.{guid}";

                _coreInstanceRegistry.Add(instanceId, metadata);
                _coreRanking.Add(instanceId);
                await _settingsService.SetValue<Dictionary<string, CoreMetadata>>(nameof(SettingsKeys.CoreInstanceRegistry), _coreInstanceRegistry);
                await _settingsService.SetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking), _coreRanking.AsReadOnly());

                CoreInstanceRegistered?.Invoke(this, new CoreInstanceEventArgs(instanceId, coreMetadata));
                return instanceId;
            }

            return ThrowHelper.ThrowArgumentException<string>($"Could not find core with ID {coreMetadata.Id} in the registry.");
        }

        /// <inheritdoc/>
        public async Task UnregisterCoreInstanceAsync(string instanceId)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));
            Guard.HasSizeGreaterThan(CoreRegistry.MetadataRegistry, 0, nameof(CoreRegistry.MetadataRegistry));
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreInstanceRegistry.Count, 0, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreRanking.Count, 0, nameof(_coreRanking));

            if (_coreInstanceRegistry.TryGetValue(instanceId, out var value))
            {
                _coreInstanceRegistry.Remove(instanceId);
                _coreRanking.Remove(instanceId);
            }

            // TODO: Wait for MainViewModel to fully dispose of core.
            CoreInstanceUnregistered?.Invoke(this, new CoreInstanceEventArgs(instanceId, value));

            var rootStorageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(instanceId, CreationCollisionOption.OpenIfExists);
            await rootStorageFolder.DeleteAsync();

            await _settingsService.SetValue<Dictionary<string, CoreMetadata>>(nameof(SettingsKeys.CoreInstanceRegistry), _coreInstanceRegistry);
            await _settingsService.SetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking), _coreRanking.AsReadOnly());
        }

        /// <inheritdoc/>
        public async Task<IServiceCollection> CreateInitialCoreServicesAsync(ICore core)
        {
            var services = new ServiceCollection();
            var rootStorageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(core.InstanceId, CreationCollisionOption.OpenIfExists);

            // The same INotificationService instance should be used across all core instances.
            var notificationService = Ioc.Default.GetRequiredService<INotificationService>();
            var sharedFactory = Ioc.Default.GetRequiredService<ISharedFactory>();
            var launcherService = Ioc.Default.GetRequiredService<ILauncher>();

            services.AddSingleton(await sharedFactory.CreateFileSystemServiceAsync(rootStorageFolder.Path));
            services.AddSingleton(notificationService);
            services.AddSingleton(launcherService);

            return services;
        }

        /// <inheritdoc/>
        public CoreMetadata GetCoreMetadata(ICore core)
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

            var ranking = await GetCoreInstanceRanking();
            _coreRanking = ranking.ToList();
        }
    }
}