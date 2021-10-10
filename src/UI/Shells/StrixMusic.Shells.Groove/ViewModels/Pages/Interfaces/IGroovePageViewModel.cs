namespace StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces
{
    /// <summary>
    /// An interface for ViewModels to display elements in the groove main content area.
    /// </summary>
    public interface IGroovePageViewModel
    {
        /// <summary>
        /// Gets if the Page should show the header under main content.
        /// </summary>
        bool ShowLargeHeader { get; }

        /// <summary>
        /// Gets the resource key for the title.
        /// </summary>
        string PageTitleResource { get; }
    }
}
