using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IImage"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class ImagePluginBase : IImage, IDelegatable<IImage>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ImagePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected ImagePluginBase(ModelPluginMetadata registration, IImage inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public virtual IImage Inner { get; }

        /// <inheritdoc/>
        public virtual Uri Uri => Inner.Uri;

        /// <inheritdoc/>
        public virtual double Height => Inner.Height;

        /// <inheritdoc/>
        public virtual double Width => Inner.Width;

        /// <inheritdoc/>
        public virtual IReadOnlyList<ICoreImage> Sources => Inner.Sources;

        /// <inheritdoc/>
        public virtual IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImage other) => Inner.Equals(other);
    }
}
