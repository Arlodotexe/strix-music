using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collections;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Items
{
    /// <summary>
    /// Defines the current state of the zune <see cref="CollectionContent"/>.
    /// </summary>
    public enum CollectionContentType
    {
        /// <summary>
        /// Flag for zune album collection view.
        /// </summary>
        Albums,

        /// <summary>
        /// Flag for zune artist collection view.
        /// </summary>
        Artist,

        /// <summary>
        /// Flag for zune tracks collection view.
        /// </summary>
        Tracks,

        /// <summary>
        /// Flag for zune genre collection view.
        /// </summary>
        Genres
    }
}
