using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Interfaces;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Services.StorageService;
using StrixMusic.Sdk.Services.SuperShell;

namespace StrixMusic.Shared
{
    /// <summary>
    /// A <see langword="class"/> to initialize the <see cref="MainViewModel"/>.
    /// </summary>
    public class IocLocator
    {
        static IocLocator()
        {
            Ioc.Default.ConfigureServices(async services =>
            {
                var textStorageService = new TextStorageService();
                var settingsService = new DefaultSettingsService(textStorageService);

                services.AddSingleton<ITextStorageService>(textStorageService);
                services.AddSingleton<ISettingsService>(settingsService);
                services.AddSingleton<ISuperShellService, SuperShellService>();
                services.AddSingleton<IFileSystemService, FileSystemService>();

                // Todo: If coreRegistry is null, show out of box setup page.
                var coreRegistry = await settingsService.GetValue<Dictionary<string, Type>>(nameof(SettingsKeys.CoreRegistry));

                if (coreRegistry != null)
                {
                    var cores = coreRegistry.Select(x => (ICore)Activator.CreateInstance(x.Value, x.Key));

                    services.AddSingleton(cores);
                }

                services.AddSingleton<MainViewModel>();
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocLocator"/> class and the <see cref="MainViewModel"/>.
        /// </summary>
        public IocLocator()
        {
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel Main { get; } = Ioc.Default.GetService<MainViewModel>();
    }
}