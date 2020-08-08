using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing a collection of playable items.
    /// </summary>
    /// <remarks>No <see langword="class"/> should ever directly implement this interface. The items in this collection, the count, and the method for getting them are defined in a child <see langword="interface" />.</remarks>
    public interface IPlayableCollectionBase
    {
        /// <summary>
        /// Id for the collection.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The source core which created the parent.
        /// </summary>
        public ICore SourceCore { get; }

        /// <summary>
        /// The name of the Playable Collection.
        /// </summary>
        /// <remarks>
        /// Example: <example>Genre name, Playlist folder name, Daily Mix, etc</example>
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Cover images for the collection.
        /// </summary>
        IReadOnlyList<IImage> Images { get; }

        /// <summary>
        /// An external link related to the collection.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Provides comments about the collection.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Owner of the collection.
        /// </summary>
        IUserProfile? Owner { get; }

        /// <summary>
        /// If the collection is currently playing, this will represent the time in milliseconds that the song is currently playing.
        /// </summary>
        PlaybackState State { get; }

        /// <summary>
        /// If the collection is currently playing, this will represent the currently playing <see cref="ITrack"/>.
        /// </summary>
        ITrack? PlayingTrack { get; }

        /// <summary>
        /// Attempts to play the collection from the beginning, or resumes playback if already playing.
        /// </summary>
        void Play();

        /// <summary>
        /// Attempts to pause the collection.
        /// </summary>
        void Pause();
    }
}
