using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IImage"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class ImagePluginBase : IModelPlugin, IImage, IDelegatable<IImage>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ImagePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal ImagePluginBase(ModelPluginMetadata registration, IImage inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        virtual public IImage Inner { get; }

        /// <inheritdoc/>
        virtual public Uri Uri => Inner.Uri;

        /// <inheritdoc/>
        virtual public double Height => Inner.Height;

        /// <inheritdoc/>
        virtual public double Width => Inner.Width;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreImage> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

        /// <inheritdoc/>
        virtual public ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        virtual public bool Equals(ICoreImage other) => Inner.Equals(other);
    }
}
