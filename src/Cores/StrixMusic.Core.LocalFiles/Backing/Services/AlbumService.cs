using OwlCore.Provisos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    /// <summary>
    /// Album service for creating or getting the album metadata.
    /// </summary>
    public class AlbumService : IAsyncInit
    {
        /// <summary>
        /// Creates a new instance of <see cref="AlbumService"/>.
        /// </summary>
        public AlbumService()
        {
        }

        /// <summary>
        /// Initializes the album service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitAsync()
        {
            throw new NotImplementedException();
        }
    }
}
