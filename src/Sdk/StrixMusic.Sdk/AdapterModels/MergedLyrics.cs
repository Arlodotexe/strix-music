// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// A concrete class that converts a <see cref="ICoreLyrics"/> to a <see cref="ILyrics"/>.
    /// </summary>
    public sealed class MergedLyrics : ILyrics, IMergedMutable<ICoreLyrics>
    {
        private readonly ICoreLyrics _source;

        /// <summary>
        /// Creates a new instance of <see cref="MergedLyrics"/>.
        /// </summary>
        public MergedLyrics(ICoreLyrics source, MergedCollectionConfig config)
        {
            _source = source;

            Sources = source.IntoList();
            Track = new MergedTrack(source.Track.IntoList(), config);
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
