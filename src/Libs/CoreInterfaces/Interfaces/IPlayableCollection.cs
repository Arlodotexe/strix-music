using System;
using System.Collections.Generic;
using StrixMusic.CoreInterfaces.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing a collection of <see cref="ITrack"/>.
    /// </summary>
    public interface IPlayableCollection
    {
        /// <summary>
        /// Id for the collection.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Cover images for the collection.
        /// </summary>
        IList<IImage> Images { get; }

        /// <summary>
        /// An external link related to the collection.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Provides comments about the collection.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Name of the collection
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Number of tracks in the collection.
        /// </summary>
        int TrackCount { get; }

        /// <summary>
        /// Owner of the collection.
        /// </summary>
        IUser? Owner { get; }

        /// <summary>
        /// List of <see cref="ITrack"/>s that this collection contains.
        /// </summary>
        IList<ITrack> Tracks { get; }

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
