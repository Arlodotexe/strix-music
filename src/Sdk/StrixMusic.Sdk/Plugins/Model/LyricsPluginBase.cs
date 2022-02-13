using System;
using System.Collections.Generic;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="ILyrics"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class LyricsPluginBase : IModelPlugin, ILyrics, IDelegatable<ILyrics>
    {
        /// <summary>
        /// Creates a new instance of <see cref="LyricsPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal LyricsPluginBase(ModelPluginMetadata registration, ILyrics inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public virtual ILyrics Inner { get; }

        /// <inheritdoc/>
        public virtual ITrack Track => Inner.Track;

        /// <inheritdoc/>
        public virtual Dictionary<TimeSpan, string>? TimedLyrics => Inner.TimedLyrics;

        /// <inheritdoc/>
        public virtual string? TextLyrics => Inner.TextLyrics;

        /// <inheritdoc/>
        public virtual bool Equals(ICoreLyrics other) => Inner.Equals(other);

        /// <inheritdoc/>
        public IReadOnlyList<ICoreLyrics> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;
    }
}
