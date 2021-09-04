using StrixMusic.Core.LocalFiles;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Text;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.OneDriveCore.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace StrixMusic.Core.OneDriveCore
{
    ///  <inheritdoc/>
    public class OneDriveCoreConfig : LocalFilesCoreConfig
    {
        private AbstractTextBox _clientIdTb;
        private AbstractTextBox _tenantTb;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreConfig"/> class.
        /// </summary>
        public OneDriveCoreConfig(ICore sourceCore)
            : base(sourceCore)
        {
            SetupAbstractUISettings();
        }

        public override Task SetupConfigurationServices(IServiceCollection services)
        {
            services.AddTransient(typeof(AuthenticationManager));

            Services = services.BuildServiceProvider();

            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public override void SetupAbstractUISettings()
        {
            _clientIdTb = new AbstractTextBox("ClientId", string.Empty)
            {
                PlaceholderText = "Enter client id here.",
            };

            _tenantTb = new AbstractTextBox("Tenant Id", string.Empty)
            {
                PlaceholderText = "Enter tenant id here.",
            };


            var startButton = new AbstractButton("Start", "Get Started");

            startButton.Clicked += StartButton_Clicked;

            AbstractUIElements = new List<AbstractUIElementGroup>
            {
                new AbstractUIElementGroup("SettingsGroup")
                {
                    Items = new List<AbstractUIElement>
                    {
                        _clientIdTb,
                        _tenantTb,
                        startButton
                    },
                }
            };
        }

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            var authManager = Services.GetService<AuthenticationManager>();

            if (!string.IsNullOrWhiteSpace(_clientIdTb.Value) && !string.IsNullOrWhiteSpace(_tenantTb.Value))
            {
                authManager.Init(_clientIdTb.Value, _tenantTb.Value);

                await authManager.GenerateGraphToken();
            }
            else
            {
                //TODO: Show error.
            }
        }
    }
}
