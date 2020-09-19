namespace StrixMusic.Core.MusicBrainz.Statics
{
    /// <summary>
    /// Contains the various relationship queries that are needed for each object type when interacting with the API.
    /// </summary>
    public static class RelationshipQueries
    {
        /// <summary>
        /// The relationship queries needed when getting Recordings from the API.
        /// </summary>
        public static readonly string[] Recordings = { "artist-credits", "isrcs", "releases" };

        /// <summary>
        /// The relationship queries needed when getting Releases from the API.
        /// </summary>
        public static readonly string[] Releases = { "recordings", "tags" };

        /// <summary>
        /// The relationship queries needed when getting Artists from the API.
        /// </summary>
        public static readonly string[] Artists = { "releases" };

        /// <summary>
        /// The full relationship query needed when getting Recordings from the API.
        /// </summary>
        public static string RecordingsQuery => "inc=" + string.Join(",", Recordings);

        /// <summary>
        /// The full relationship query needed when getting Releases from the API.
        /// </summary>
        public static string ReleasesQuery => "inc=" + string.Join(",", Releases);

        /// <summary>
        /// The full relationship query needed when getting Artists from the API.
        /// </summary>
        public static string ArtistsQuery => "inc=" + string.Join(",", Artists);
    }
}
