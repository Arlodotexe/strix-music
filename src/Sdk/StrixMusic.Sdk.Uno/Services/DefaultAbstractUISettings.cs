using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractUI.Models;
using OwlCore.Provisos;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.ShellManagement;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// Provides templatable AbstractUI elements for the default app settings.
    /// </summary>
    public class DefaultAbstractUISettings : IAsyncInit
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAbstractUISettings"/> class.
        /// </summary>
        public DefaultAbstractUISettings()
        {
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();

            ThemeSettings = CreateThemeSettingItems();

            AllElementGroups = new List<AbstractUICollection>()
            {
                ThemeSettings,
            };
        }

        /// <inheritdoc />
        public bool IsInitialized { get; set; }

        /// <summary>
        /// The Abstract UI elements for the default settings.
        /// </summary>
        public List<AbstractUICollection> AllElementGroups { get; private set; }

        /// <summary>
        /// The AbstractUI element group for theme settings.
        /// </summary>
        public AbstractUICollection ThemeSettings { get; private set; }

        private AbstractUICollection CreateThemeSettingItems()
        {
            var shellSelectorItems = new List<AbstractUIMetadata>();

            foreach (var shell in ShellRegistry.MetadataRegistry)
            {
                shellSelectorItems.Add(new AbstractUIMetadata(shell.Id)
                {
                    Title = shell.DisplayName,
                });
            }

            var shellSelector = new AbstractMultiChoice("shellSelector", shellSelectorItems[0], shellSelectorItems);

            shellSelector.ItemSelected += ShellSelector_ItemSelected;

            return new AbstractUICollection(Guid.NewGuid().ToString(), PreferredOrientation.Vertical)
            {
                Title = "Theme",
                IconCode = "\uE2B1",
                Items = new List<AbstractUIElement>
                {
                    shellSelector,
                },
            };
        }

        private async void ShellSelector_ItemSelected(object sender, AbstractUIMetadata e)
        {
            await _settingsService.SetValue<string>(nameof(SettingsKeysUI.PreferredShell), e.Id);
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            IsInitialized = true;
        }
    }
}