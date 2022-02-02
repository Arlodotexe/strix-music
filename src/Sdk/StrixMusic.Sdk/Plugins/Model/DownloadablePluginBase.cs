using OwlCore.ComponentModel;
using System;
using StrixMusic.Sdk.Models;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IDownloadable"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class DownloadablePluginBase : IDownloadable, IDelegatable<IDownloadable>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected DownloadablePluginBase(IDownloadable inner)
        {
            Inner = inner;
        }

        /// <inheritdoc/>
        public IDownloadable Inner { get; set; }

        /// <inheritdoc/>
        public virtual DownloadInfo DownloadInfo => Inner.DownloadInfo;

        /// <inheritdoc/>
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => Inner.DownloadInfoChanged += value;
            remove => Inner.DownloadInfoChanged -= value;
        }

        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation) => Inner.StartDownloadOperationAsync(operation);
    }
}
