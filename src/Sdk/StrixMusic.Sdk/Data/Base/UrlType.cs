namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// The various types of URLs that might appear in a music app.
    /// </summary>
    public enum UrlType
    {
        /// <summary>
        /// Url type is not known, not provided, or does not fit into any other <see cref="UrlType"/>.
        /// </summary>
        Unspecified,

        /// <summary>
        /// An official website.
        /// </summary>
        OfficialHomepage,

        /// <summary>
        /// A social network such as Twitter, Facebook or Instagram.
        /// </summary>
        SocialNetwork,

        /// <summary>
        /// A streaming service (free or paid) where the user can listen to the linked music.
        /// </summary>
        MusicStreaming,

        /// <summary>
        /// The linked resource allows the user to purchase and download music.
        /// </summary>
        PurchaseForDownload,

        /// <summary>
        /// The linked resource provides tour or concert information.
        /// </summary>
        ToursAndConcerts,

        /// <summary>
        /// A database such as MusicBrainz or Discogs that provides detailed information.
        /// </summary>
        Database,

        /// <summary>
        /// A website such as Genius or Musixmatch that provides lyrics.
        /// </summary>
        Lyrics,
    }
}