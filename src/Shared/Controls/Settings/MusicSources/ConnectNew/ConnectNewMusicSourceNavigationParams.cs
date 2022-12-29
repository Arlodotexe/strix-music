using System.Net.Http;
using System.Threading.Tasks;
using StrixMusic.AppModels;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew;

/// <summary>
/// Holds the parameters sent between all pages in the <see cref="ConnectNewMusicSource"/> page pipeline.
/// </summary>
public class ConnectNewMusicSourceNavigationParams
{
    /// <summary>
    /// Holds a <see cref="TaskCompletionSource{TResult}"/> that can be set to complete to indicate setup is finished, or canceled to indicate setup was canceled.
    /// </summary>
    public TaskCompletionSource<object?> SetupCompleteTaskCompletionSource { get; } = new();

    /// <summary>
    /// The source that the user selected to connect, if any.
    /// </summary>
    public AvailableMusicSource? SelectedSourceToConnect { get; internal set; }

    /// <summary>
    /// Holds the <see cref="AppRoot"/> where the new music source will be added.
    /// </summary>
    public AppRoot? AppRoot { get; internal set; }

    /// <summary>
    /// The message handler to use for any HTTP requests made during setup.
    /// </summary>
    public HttpMessageHandler HttpMessageHandler { get; set; } = new HttpClientHandler();
}
