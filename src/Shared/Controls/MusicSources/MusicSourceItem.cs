using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.ComponentModel;
using StrixMusic.AppModels;

namespace StrixMusic.Controls.MusicSources;

/// <summary>
/// Holds the data needed to modify the settings for a music source.
/// </summary>
[ObservableObject]
public sealed partial class MusicSourceItem
{
    /// <summary>
    /// Creates a new instance of <see cref="MusicSourceItem"/>.
    /// </summary>
    internal MusicSourceItem(SettingsBase settings, AvailableMusicSource musicSource)
    {
        Settings = settings;
        MusicSource = musicSource;
    }

    /// <summary>
    /// The settings being modified.
    /// </summary>
    public SettingsBase Settings { get; }

    /// <summary>
    /// Information about the music source being created.
    /// </summary>
    public AvailableMusicSource MusicSource { get; }

    /// <summary>
    /// The command to use when editing has completed.
    /// </summary>
    public IRelayCommand<MusicSourceItem>? EditingFinishedCommand { get; set; }
}
