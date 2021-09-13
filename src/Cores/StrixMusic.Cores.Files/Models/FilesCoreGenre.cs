using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Cores.Files.Models
{
    /// <inheritdoc/>
    public sealed class FilesCoreGenre : ICoreGenre
    {
        /// <summary>
        /// Creates a new instance of <see cref="FilesCoreGenre"/>.
        /// </summary>
        /// <param name="sourceCore">The source core that this instance belongs to.</param>
        /// <param name="genre">The name of the genre.</param>
        public FilesCoreGenre(ICore sourceCore, string genre)
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
