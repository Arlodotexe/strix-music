using StrixMusic.Sdk.Components;

/// <summary>
/// Tells the state of the navigation of <see cref="IFolderExplorer"/>
/// </summary>
public enum NavigationState
{
    /// <summary>
    /// No navigation occurred.
    /// </summary>
    None,

    /// <summary>
    /// The navigation to the next folder in hierarchy requested.
    /// </summary>
    Forward,

    /// <summary>
    /// The navigation back requested.
    /// </summary>
    Back,
}