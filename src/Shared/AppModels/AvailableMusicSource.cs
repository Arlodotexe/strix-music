using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.AppModels;

/// <summary>
/// Represents an individual music source that can be created.
/// </summary>
[ObservableObject]
public partial class AvailableMusicSource
{
    private readonly Func<Task<ICoreImage>> _imageFactory;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
    [ObservableProperty] private ICoreImage? _coreImage;
    [ObservableProperty] private Func<Task<SettingsBase>> _defaultSettingsFactory;

    /// <summary>
    /// Creates a new instance of <see cref="AvailableMusicSource"/>.
    /// </summary>
    /// <param name="name">The display name of the available music source.</param>
    /// <param name="description">The description of the available music source.</param>
    /// <param name="imageFactory">The imageFactory to use for the music source.</param>
    /// <param name="defaultSettingsFactory">A factory used to create a settings instance.</param>
    public AvailableMusicSource(string name, string description, Func<Task<ICoreImage>> imageFactory, Func<Task<SettingsBase>> defaultSettingsFactory)
    {
        _name = name;
        _description = description;
        _imageFactory = imageFactory;
        _defaultSettingsFactory = defaultSettingsFactory;
    }

    /// <summary>
    /// Loads the image for the available music source.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    [RelayCommand]
    public async Task LoadImageAsync() => CoreImage = await _imageFactory();
}
