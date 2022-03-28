using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.OneDrive.Services;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.OneDrive.ConfigPanels
{
    /// <summary>
    /// An <see cref="AbstractUICollection"/> of generic configuration options.
    /// </summary>
    internal sealed class GeneralCoreConfigPanel : AbstractUICollection, IDisposable
    {
        private readonly OneDriveCoreSettings _settings;
        private readonly INotificationService? _notificationService;
        private Notification? _scannerRequiredNotification;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralCoreConfigPanel"/>.
        /// </summary>
        internal GeneralCoreConfigPanel(OneDriveCoreSettings settings, INotificationService? notificationService = null)
            : base(nameof(GeneralCoreConfigPanel))
        {
            Title = "OneDrive settings";

            _settings = settings;
            _notificationService = notificationService;

            UseTagLibScannerToggle = new AbstractBoolean("useTagLibScannerToggle", "Use TagLib")
            {
                Subtitle = "TagLib is more accurate and supports more formats, but is slower (not recommended).",
            };

            UseFilePropsScannerToggle = new AbstractBoolean("useFilePropsScannerToggle", "Use file properties")
            {
                Subtitle = "File properties are very fast, but provide less data.",
            };

            var metadataScanType = new AbstractUICollection("metadataScanType")
            {
                UseTagLibScannerToggle,
                UseFilePropsScannerToggle,
            };

            metadataScanType.Title = "Scanner type";
            metadataScanType.Subtitle = "Requires restart.";

            Add(metadataScanType);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _settings.PropertyChanged += OnSettingChanged;
            UseTagLibScannerToggle.StateChanged += OnUseTagLibScannerToggleChanged;
            UseFilePropsScannerToggle.StateChanged += OnUseFilePropsScannerToggleChanged;
        }

        private void DetachEvents()
        {
            _settings.PropertyChanged -= OnSettingChanged;
            UseTagLibScannerToggle.StateChanged -= OnUseTagLibScannerToggleChanged;
            UseFilePropsScannerToggle.StateChanged -= OnUseFilePropsScannerToggleChanged;
        }

        protected async void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            var isFilePropToggle = e.PropertyName == nameof(OneDriveCoreSettings.ScanWithFileProperties);
            var isTagLibToggle = e.PropertyName == nameof(OneDriveCoreSettings.ScanWithTagLib);

            if (!_settings.ScanWithFileProperties && !_settings.ScanWithTagLib)
            {
                _scannerRequiredNotification?.Dismiss();
                _scannerRequiredNotification = _notificationService?.RaiseNotification("Whoops", "At least one metadata scanner is required.");

                if (isFilePropToggle)
                    _settings.ScanWithFileProperties = true;

                if (isTagLibToggle)
                    _settings.ScanWithTagLib = true;

                return;
            }

            // AbstractUI doesn't set state if already the same, preventing an execution loop here.
            if (isFilePropToggle)
                UseFilePropsScannerToggle.State = _settings.ScanWithFileProperties;

            if (isTagLibToggle)
                UseTagLibScannerToggle.State = _settings.ScanWithTagLib;

            await _settings.SaveAsync();
        }

        private void OnUseFilePropsScannerToggleChanged(object sender, bool e) => _settings.ScanWithFileProperties = e;

        private void OnUseTagLibScannerToggleChanged(object sender, bool e) => _settings.ScanWithTagLib = e;

        /// <summary>
        /// A toggle that indicates if the file metadata scanner should ignore and previously scanned data.
        /// </summary>
        public AbstractBoolean InitWithEmptyReposToggle { get; }

        /// <summary>
        /// A toggle that indicates if TagLibSharp should be used to read metadata from audio files.
        /// </summary>
        public AbstractBoolean UseTagLibScannerToggle { get; }

        /// <summary>
        /// A toggle that indicates if file properties should be used to read metadata from audio files.
        /// </summary>
        public AbstractBoolean UseFilePropsScannerToggle { get; }

        /// <summary>
        /// A button that, when clicked, should trigger a rescan of metadata.
        /// </summary>
        public AbstractButton RescanButton { get; }

        /// <inheritdoc />
        public void Dispose() => DetachEvents();
    }
}
