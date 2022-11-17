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
    private readonly Func<Task<ICoreImage>> _imageFactory;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
    [ObservableProperty] private ICoreImage? _coreImage;
    [ObservableProperty] private Func<Task<SettingsBase>> _defaultSettingsFactory;

    /// <summary>
    /// Creates a new instance of <see cref="AvailableCore"/>.
    /// </summary>
    /// <param name="name">The display name of the available core.</param>
    /// <param name="description">The description of the available core.</param>
    /// <param name="imageFactory">The imageFactory to use for the core.</param>
    /// <param name="defaultSettingsFactory">A factory used to create a settings instance.</param>
    public AvailableCore(string name, string description, Func<Task<ICoreImage>> imageFactory, Func<Task<SettingsBase>> defaultSettingsFactory)
    {
        _name = name;
        _description = description;
        _imageFactory = imageFactory;
        _defaultSettingsFactory = defaultSettingsFactory;
    }

    /// <summary>
    /// Loads the imageFactory for the core.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    [RelayCommand]
    public async Task LoadImageAsync() => CoreImage = await _imageFactory();
}
