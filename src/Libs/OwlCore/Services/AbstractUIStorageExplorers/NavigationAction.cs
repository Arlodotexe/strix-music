/// <summary>
/// The most recent navigation action.
/// </summary>
public enum NavigationAction
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