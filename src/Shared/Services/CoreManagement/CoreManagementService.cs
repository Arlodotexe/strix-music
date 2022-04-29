using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.CoreModels;
using Windows.Storage;

namespace StrixMusic.Services.CoreManagement
{
    /// <summary>
    /// Manages added and removing core instances.
    /// </summary>
    public sealed class CoreManagementService : ICoreManagementService
    {
        private readonly AppSettings _settings;

        /// <summary>
        /// Creates a new instance of <see cref="CoreManagementService"/>.
        /// </summary>
        /// <param name="settings">Settings service used to store core information.</param>
        public CoreManagementService(AppSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc/>
        public async Task<string> RegisterCoreInstanceAsync(CoreMetadata coreMetadata)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));
            Guard.HasSizeGreaterThan(CoreRegistry.MetadataRegistry, 0, nameof(CoreRegistry.MetadataRegistry));

            foreach (var metadata in CoreRegistry.MetadataRegistry)
            {
                if (metadata.Id != coreMetadata.Id)
                    continue;

                var guid = Guid.NewGuid();
                var instanceId = $"{metadata.Id}.{guid}";

                _settings.CoreInstanceRegistry.Add(instanceId, metadata);
                _settings.CoreRanking.Add(instanceId);

                await _settings.SaveAsync();

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
            Guard.IsGreaterThan(_settings.CoreInstanceRegistry.Count, 0, nameof(_settings.CoreInstanceRegistry));
            Guard.IsGreaterThan(_settings.CoreRanking.Count, 0, nameof(_settings.CoreRanking));

            if (_settings.CoreInstanceRegistry.TryGetValue(instanceId, out var value))
            {
                _settings.CoreInstanceRegistry.Remove(instanceId);
                _settings.CoreRanking.Remove(instanceId);
            }

            // TODO: Wait to fully dispose of core.
            CoreInstanceUnregistered?.Invoke(this, new CoreInstanceEventArgs(instanceId, value));

            var rootStorageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(instanceId, CreationCollisionOption.OpenIfExists);
            await rootStorageFolder.DeleteAsync();

            await _settings.SaveAsync();
        }

        /// <inheritdoc/>
        public CoreMetadata GetCoreMetadata(ICore core)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));

            return _settings.CoreInstanceRegistry.First(x => x.Key == core.InstanceId).Value;
        }

        /// <inheritdoc/>
        public event EventHandler<CoreInstanceEventArgs>? CoreInstanceRegistered;

        /// <inheritdoc/>
        public event EventHandler<CoreInstanceEventArgs>? CoreInstanceUnregistered;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            await _settings.LoadAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, CoreMetadata>> GetCoreInstanceRegistryAsync()
        {
            await _settings.LoadAsync();
            return _settings.CoreInstanceRegistry;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<string>> GetCoreInstanceRanking()
        {
            await _settings.LoadAsync();
            return _settings.CoreRanking;
        }
    }
}
