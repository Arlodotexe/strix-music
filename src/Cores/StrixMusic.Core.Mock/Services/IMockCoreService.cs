using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.Mock.Services
{
    /// <summary>
    /// Service to get mock data
    /// </summary>
    public interface IMockCoreService
    {
        /// <summary>
        /// This method returns the artist
        /// </summary>
        /// <returns >IReadOnlyList<IArtist></returns>
        public IReadOnlyList<IArtist> GetArtist();

        /// <summary>
        /// This method returns the artist
        /// </summary>
        /// <returns >IReadOnlyList<IAlbum></returns>
        public IReadOnlyList<IAlbum> GetRelease();
    }
}
