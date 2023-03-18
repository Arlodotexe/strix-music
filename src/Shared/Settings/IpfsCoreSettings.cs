using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OwlCore.ComponentModel;
using OwlCore.Kubo;
using OwlCore.Storage;
using OwlCore.Storage.Memory;

namespace StrixMusic.Settings
{
    /// <summary>
    /// A container for the settings needed to instantiate a Ipfs media source.
    /// </summary>
    public class IpfsCoreSettings : CoreSettingsBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="IpfsCoreSettings"/>.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="settingSerializer"></param>
        public IpfsCoreSettings(IModifiableFolder folder) : base(folder, AppSettingsSerializer.Singleton)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicSourcesSettings"/> class.
        /// </summary>
        public IpfsCoreSettings()
            : this(new MemoryFolder(Guid.NewGuid().ToString(), nameof(IpfsCoreSettings)))
        {
        }

        /// <summary>
        /// Gets or sets the instance ID of the source.
        /// </summary>
        public string InstanceId
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// Gets or sets an ID 
        /// </summary>
        public string? IpfsCidPath
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <inheritdoc />
        public override object GetSettingByName(string settingName) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool IsSettingValidForCoreCreation(string propertyName, object? value)
        {
            return !string.IsNullOrWhiteSpace((string?)value ?? string.Empty);
        }
    }
}
