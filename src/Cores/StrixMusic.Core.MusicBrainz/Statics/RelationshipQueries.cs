namespace StrixMusic.Core.MusicBrainz.Statics
{
    /// <summary>
    /// Contains the various relationship queries that are needed for each object type when interacting with the API.
    /// </summary>
    public static class RelationshipQueries
    {
        private static readonly string[] _recordingQueries = { "artist-credits", "isrcs", "releases" };

        /// <summary>
        /// The relationship queries needed when getting tracks from the API.
        /// </summary>
        public static string Recordings => "inc=" + string.Join(",", _recordingQueries);
    }
}
