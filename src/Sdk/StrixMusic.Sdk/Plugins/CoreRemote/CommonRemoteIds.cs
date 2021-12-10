namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Common Ids used for core remoting.
    /// </summary>
    public static class CommonRemoteIds
    {
        /// <summary>
        /// The remoting ID for a core's main library.
        /// </summary>
        public static string RootLibrary => nameof(RootLibrary);

        /// <summary>
        /// The remoting ID for a core's main discoverables collection.
        /// </summary>
        public static string RootDiscoverables => nameof(RootDiscoverables);

        /// <summary>
        /// The remoting ID for a core's main recently played collection.
        /// </summary>
        public static string RootRecentlyPlayed => nameof(RootRecentlyPlayed);

        /// <summary>
        /// The remoting ID for a core's pin collection.
        /// </summary>
        public static string Pins => nameof(Pins);
    }
}
