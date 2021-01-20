using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that converts a <see cref="ICoreLyrics"/> to a <see cref="ILyrics"/>.
    /// </summary>
    public class MergedLyrics : ILyrics, IMergedMutable<ICoreLyrics>
    {
        private readonly ICoreLyrics _source;

        /// <summary>
        /// Creates a new instance of <see cref="MergedLyrics"/>.
        /// </summary>
        /// <param name="source">The source to wrap around.</param>
        public MergedLyrics(ICoreLyrics source)
        {
            _source = source ?? ThrowHelper.ThrowArgumentNullException<ICoreLyrics>(nameof(source));

            Sources = source.IntoList();
            Track = new MergedTrack(source.Track.IntoList());
        }

        /// <inheritdoc />
        public Dictionary<TimeSpan, string>? TimedLyrics => _source.TimedLyrics;

        /// <inheritdoc />
        public string? TextLyrics => _source.TextLyrics;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores => _source.SourceCore.IntoList();

        /// <inheritdoc />
        void IMergedMutable<ICoreLyrics>.AddSource(ICoreLyrics itemToMerge)
        {
            ThrowHelper.ThrowNotSupportedException($"Merging lyrics from multiple sources not yet supported.");
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreLyrics>.RemoveSource(ICoreLyrics itemToRemove)
        {
            ThrowHelper.ThrowNotSupportedException($"Merging lyrics from multiple sources not yet supported.");
        }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreLyrics> Sources { get; }

        /// <inheritdoc />
        public ITrack Track { get; }

        /// <inheritdoc />
        public bool Equals(ICoreLyrics other)
        {
            return false;
        }
    }
}