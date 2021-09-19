namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Holds details about a genre.
    /// </summary>
    public interface IGenreBase : ICollectionItemBase
    {
        /// <summary>
        /// The name of the genre.
        /// </summary>
        public string Name { get; }
    }
}