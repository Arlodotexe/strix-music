﻿using Microsoft.Extensions.DependencyInjection;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// Manages added and removing core instances.
    /// </summary>
    public interface ICoreManagementService : IAsyncInit
    {
        /// <summary>
        /// Raised when a core is added to the <see cref="SettingsKeys.CoreInstanceRegistry"/>.
        /// </summary>
        event EventHandler<CoreInstanceEventArgs>? CoreInstanceRegistered;

        /// <summary>
        /// Raised when a core is removed from the <see cref="SettingsKeys.CoreInstanceRegistry"/>.
        /// </summary>
        event EventHandler<CoreInstanceEventArgs>? CoreInstanceUnregistered;

        /// <summary>
        /// Gets a list of all cores that the user has set up and configured.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<Dictionary<string, CoreAssemblyInfo>> GetCoreInstanceRegistryAsync();


        /// <summary>
        /// Gets a sorted list of cores instance IDs that indicate to the user's preferred ranking.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public Task<IReadOnlyList<string>> GetCoreInstanceRanking();

        /// <summary>
        /// Gets a list of all cores that are available to be created.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IReadOnlyList<CoreAssemblyInfo>> GetCoreRegistryAsync();

        /// <summary>
        /// Registers a core into the <see cref="SettingsKeys.CoreInstanceRegistry"/>.
        /// </summary>
        /// <param name="coreAssemblyInfo">The assembly info for the core being registered.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the <see cref="ICore.InstanceId"/> used to uniquely identify the core instance.</returns>
        Task<string> RegisterCoreInstanceAsync(CoreAssemblyInfo coreAssemblyInfo);

        /// <summary>
        /// Unregisters a core from the <see cref="SettingsKeys.CoreInstanceRegistry"/>.
        /// </summary>
        /// <param name="instanceId">The ID of the core to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UnregisterCoreInstanceAsync(string instanceId);

        /// <summary>
        /// Given a core intance, return the <see cref="CoreAssemblyInfo"/> that was used to create it.
        /// </summary>
        /// <param name="core">The core instance to check.</param>
        /// <returns>The assembly info used to create the given <paramref name="core"/>.</returns>
        CoreAssemblyInfo GetCoreAssemblyInfoForCore(ICore core);

        /// <summary>
        /// Creates the services that are injected into a core on InitAsync.
        /// </summary>
        /// <param name="core">The core to create services for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IServiceCollection> CreateInitialCoreServicesAsync(ICore core);
    }
}