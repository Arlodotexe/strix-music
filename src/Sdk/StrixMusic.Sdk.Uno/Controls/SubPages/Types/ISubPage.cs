namespace StrixMusic.Sdk.Uno.Controls.SubPages.Types
{
    /// <summary>
    /// Implemented by SubPages that want to specify general configs.
    /// </summary>
    public interface ISubPage
    {
        /// <summary>
        /// Gets the header for the SubPage.
        /// </summary>
        public string Header { get; }
    }
}
