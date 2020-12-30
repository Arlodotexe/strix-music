using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;

namespace StrixMusic.Sdk.Services.Settings
{
    /// <summary>
    /// Provides templatable AbstractUI elements for the default app settings.
    /// </summary>
    public class DefaultAbstractUISettings : IAsyncInit
    {
        private readonly ISettingsService _settingsService;
        private IReadOnlyList<ShellAssemblyInfo>? _shellRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAbstractUISettings"/> class.
        /// </summary>
        public DefaultAbstractUISettings()
        {
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();

            ThemeSettings = CreateThemeSettingItems();

            AllElementGroups = new List<AbstractUIElementGroup>()
            {
                ThemeSettings,
            };
        }

        /// <summary>
        /// The Abstract UI elements for the default settings.
        /// </summary>
        public List<AbstractUIElementGroup> AllElementGroups { get; private set; }

        /// <summary>
        /// The AbstractUI element group for theme settings.
        /// </summary>
        public AbstractUIElementGroup ThemeSettings { get; private set; }

        private AbstractUIElementGroup CreateThemeSettingItems()
        {
            Guard.IsNotNull(_shellRegistry, nameof(_shellRegistry));

            var shellSelectorItems = new List<AbstractUIMetadata>();

            foreach (var shell in _shellRegistry)
            {
                shellSelectorItems.Add(new AbstractUIMetadata(shell.AssemblyName)
                {
                    Title = shell.DisplayName,
                });
            }

            var shellSelector = new AbstractMultiChoiceUIElement("shellSelector", shellSelectorItems[0], shellSelectorItems);

            shellSelector.ItemSelected += ShellSelector_ItemSelected;

            return new AbstractUIElementGroup(Guid.NewGuid().ToString(), PreferredOrientation.Vertical)
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
            await _settingsService.SetValue<string>(nameof(SettingsKeys.PreferredShell), e.Id);
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            _shellRegistry = await _settingsService.GetValue<IReadOnlyList<ShellAssemblyInfo>>(nameof(SettingsKeysUI.ShellRegistry)).RunInBackground();
        }
    }
}