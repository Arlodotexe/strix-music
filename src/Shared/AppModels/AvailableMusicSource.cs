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
public partial class AvailableMusicSource : ObservableObject
{
    private readonly Func<Task<ICoreImage>> _imageFactory;
    private string _name;
    private string _description;
    private ICoreImage? _coreImage;

    private Func<string, Task<SettingsBase>> _defaultSettingsFactory;

    /// <summary>
    /// Provided an instance id, this returns a new settings instance for this music source.
    /// </summary>
    public Func<string, Task<SettingsBase>> DefaultSettingsFactory
    {
        get => _defaultSettingsFactory;
        set => SetProperty(ref _defaultSettingsFactory, value);
    }

    /// <summary>
    /// An image that represents this music source.
    /// </summary>
    public ICoreImage? CoreImage
    {
        get => _coreImage;
        set => SetProperty(ref _coreImage, value);
    }

    /// <summary>
    /// A description of this music source.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// The displayed name of this music source.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Creates a new instance of <see cref="AvailableMusicSource"/>.
    /// </summary>
    /// <param name="name">The display name of the available music source.</param>
    /// <param name="description">The description of the available music source.</param>
    /// <param name="imageFactory">The imageFactory to use for the music source.</param>
    /// <param name="defaultSettingsFactory">A factory used to create a settings instance. Passed parameter is the instance id of the core, which must be defined in a single place (not here).</param>
    public AvailableMusicSource(string name, string description, Func<Task<ICoreImage>> imageFactory, Func<string, Task<SettingsBase>> defaultSettingsFactory)
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
