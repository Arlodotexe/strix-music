using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive.ConfigPanels
{
    /// <summary>
    /// An <see cref="AbstractUICollection"/> that allows the user to provide custom keys for authenticating an application against OneDrive.
    /// </summary>
    internal class OneDriveAppKeyConfigPanel : AbstractUICollection, IDisposable
    {
        private readonly OneDriveCoreSettings _settings;

        /// <inheritdoc />
        public OneDriveAppKeyConfigPanel(OneDriveCoreSettings settings)
            : base(nameof(OneDriveAppKeyConfigPanel))
        {
            _settings = settings;
            Title = "Configure application identify";
            Subtitle = "Authenticate your Microsoft account against your own trusted app identity";

            ClientIdTb = new AbstractTextBox("ClientId", string.Empty, "Enter client id");
            TenantTb = new AbstractTextBox("Tenant Id", string.Empty, "Enter tenant id");
            RedirectUriTb = new AbstractTextBox("Redirect Uri", string.Empty, "Enter redirect uri");

            Add(ClientIdTb);
            Add(TenantTb);
            Add(RedirectUriTb);

            AttachEvents();
        }

        private void AttachEvents()
        {
            ClientIdTb.ValueChanged += OnTextBoxChanged;
            TenantTb.ValueChanged += OnTextBoxChanged;
            RedirectUriTb.ValueChanged += OnTextBoxChanged;
        }

        private void DetachEvents()
        {
            ClientIdTb.ValueChanged -= OnTextBoxChanged;
            TenantTb.ValueChanged -= OnTextBoxChanged;
            RedirectUriTb.ValueChanged -= OnTextBoxChanged;
        }

        private async void OnTextBoxChanged(object sender, string value)
        {
            if (sender == ClientIdTb)
                _settings.ClientId = value.Trim();

            if (sender == TenantTb)
                _settings.TenantId = value.Trim();

            if (sender == RedirectUriTb)
                _settings.RedirectUri = value.Trim();

            await _settings.SaveAsync();
        }

        /// <summary>
        /// Text entry for a custom Client ID when logging into OneDrive.
        /// </summary>
        /// <remarks>
        /// This text box should never display an existing settings value.
        /// </remarks>
        public AbstractTextBox ClientIdTb { get; set; }

        /// <summary>
        /// Text entry for a custom Tenant ID when logging into OneDrive.
        /// </summary>
        /// <remarks>
        /// This text box should never display an existing settings value.
        /// </remarks>
        public AbstractTextBox TenantTb { get; set; }

        /// <summary>
        /// Text entry for a custom RedirectUri when logging into OneDrive.
        /// </summary>
        /// <remarks>
        /// This text box should never display an existing settings value.
        /// </remarks>
        public AbstractTextBox RedirectUriTb { get; set; }

        /// <inheritdoc />
        public void Dispose() => DetachEvents();
    }
}
