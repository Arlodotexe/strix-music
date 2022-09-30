// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

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
        /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
        internal protected LyricsPluginBase(ModelPluginMetadata registration, ILyrics inner, IStrixDataRoot pluginRoot)
        {
            Metadata = registration;
            Inner = inner;
            Root = pluginRoot;
        }

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public ILyrics Inner { get; }

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

        /// <inheritdoc />
        public IStrixDataRoot Root { get; }
    }
}
