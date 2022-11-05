using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.AppModels;

[ObservableObject]
public partial class AvailableCore
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
    [ObservableProperty] private ICoreImage? _coreImage;
    [ObservableProperty] private Func<Task<SettingsBase>> _defaultSettingsFactory;

    /// <summary>
    /// Creates a new instance of <see cref="AvailableCore"/>.
    /// </summary>
    /// <param name="name">The display name of the available core.</param>
    /// <param name="description">The description of the available core.</param>
    /// <param name="defaultSettingsFactory">A factory used to create a settings instance.</param>
    public AvailableCore(string name, string description, Func<Task<SettingsBase>> defaultSettingsFactory)
    {
        _name = name;
        _description = description;
        _defaultSettingsFactory = defaultSettingsFactory;
    }

    /// <summary>
    /// Submits the provided <paramref name="settings"/> to be created as a core.
    /// </summary>
    /// <param name="settings">The settings instance used to create the core.</param>
    [RelayCommand]
    public void CreateCore(SettingsBase settings) => CreateCoreRequested?.Invoke(this, settings);

    /// <summary>
    /// Raised when a settings instance is created and submitted for usage.
    /// </summary>
    public event EventHandler<SettingsBase>? CreateCoreRequested;
}
