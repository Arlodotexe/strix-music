using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Storage;

namespace StrixMusic.Settings;

/// <summary>
/// A base class for getting, setting and validating setting values as properties. Fast access in memory, with data persistence in a file system.
/// </summary>
public abstract class CoreSettingsBase : SettingsBase
{
    private readonly Dictionary<string, bool> _settingsValidity = new();
    private bool _canCreateCore;

    [JsonIgnore]
    public override IModifiableFolder Folder => base.Folder;

    [JsonIgnore]
    public override bool HasUnsavedChanges => base.HasUnsavedChanges;

    /// <summary>
    /// Creates a new instance of <see cref="CoreSettingsBase"/>.
    /// </summary>
    /// <param name="folder">The folder where settings are stored.</param>
    /// <param name="settingSerializer">The serializer used to serialize and deserialize settings to and from disk.</param>
    protected CoreSettingsBase(IModifiableFolder folder, IAsyncSerializer<Stream> settingSerializer)
        : base(folder, settingSerializer)
    {
    }

    /// <summary>
    /// Gets a value that indicates if a core can be created 
    /// </summary>
    public bool CanCreateCore
    {
        get => _canCreateCore;
        private set
        {
            if (value == _canCreateCore)
                return;

            _canCreateCore = value;
            OnPropertyChanged(nameof(CanCreateCore));
        }
    }

    /// <summary>
    /// Validates if a given setting is valid for core creation.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <returns>A value that indicates if the provided setting is valid for core creation.</returns>
    public abstract bool IsSettingValidForCoreCreation(string propertyName, object? value);

    /// <summary>
    /// Gets a setting value by the property name.
    /// </summary>
    /// <param name="settingName">The name of the property.</param>
    /// <returns>The current value of the property with the name <paramref name="settingName"/>.</returns>
    public abstract object GetSettingByName(string settingName);

    /// <inheritdoc />
    protected override void SetSetting<T>(T value, [CallerMemberName] string key = "")
    {
        TryUpdateSettingValidity(key, value);

        base.SetSetting(value, key);
    }

    /// <inheritdoc />
    public override async Task LoadAsync(CancellationToken? cancellationToken = null)
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not null)
                TryUpdateSettingValidity(e.PropertyName, GetSettingByName(e.PropertyName));
        }

        PropertyChanged += OnPropertyChanged;

        await base.LoadAsync(cancellationToken);

        PropertyChanged -= OnPropertyChanged;
    }

    private void TryUpdateSettingValidity(string key, object? value)
    {
        var valueIsValid = IsSettingValidForCoreCreation(key, value);
        _settingsValidity.TryGetValue(key, out var valueWasValid);

        if (value is null && _settingsValidity.ContainsKey(key))
        {
            // Remove if null
            _settingsValidity.Remove(key);
        }
        else if (valueWasValid != valueIsValid && !_settingsValidity.TryAdd(key, valueIsValid))
        {
            // Otherwise add/update
            _settingsValidity[key] = valueIsValid;
        }

        // Fast-path. A single invalid value means no core can be created.
        if (CanCreateCore && !valueIsValid)
        {
            CanCreateCore = false;
        }
        else
        {
            // Slow path. Make sure every value is valid.
            CanCreateCore = _settingsValidity.All(x => x.Value);
        }
    }
}
