using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    public interface ISearchQueryBase
    {
        /// <summary>
        /// The text that was entered to create this search entry.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// The date and time this entry was created.
        /// </summary>
        public DateTime CreatedAt { get; }
    }
}