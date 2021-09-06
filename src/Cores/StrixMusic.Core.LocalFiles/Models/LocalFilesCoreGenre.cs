using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Core;


namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc/>
    public class LocalFilesCoreGenre : ICoreGenre
    {
        /// <inheritdoc/>
        public LocalFilesCoreGenre(ICore sourceCore, string genre)
        {
            SourceCore = sourceCore;
            Name = genre;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
