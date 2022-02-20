using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Holds data that uniquely identifies an item played from an<see cref="IAudioPlayerService"/>.
    /// </summary>
    public record PlaybackItem
    {
        /// <summary>
        /// The media source to be played.
        /// </summary>
        public IMediaSourceConfig? MediaConfig { get; set; }

        /// <summary>
        /// The track that holds information of from all merged source.
        /// </summary>
        public ITrack? Track { get; set; }
    }
}
