using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents an item that can be played.
    /// </summary>
    public interface IPlayable
    {
        /// <summary>
        /// The ID of the playable item.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of the playable item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Cover images for the item.
        /// </summary>
        IReadOnlyList<IImage> Images { get; }

        /// <summary>
        /// Provides comments about the item.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Attempts to play the item, or resumes playback if already playing.
        /// </summary>
        void Play();

        /// <summary>
        /// Attempts to pause the item.
        /// </summary>
        void Pause();
    }
}
