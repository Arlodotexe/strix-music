using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        Task<IReadOnlyList<IArtist>> GetArtistsAsync();

        /// <summary>
        /// This method returns the artist
        /// </summary>
        Task<IReadOnlyList<IAlbum>> GetAlbumsAsync();

        /// <summary>
        /// This method returns the library
        /// </summary>
        Task<MockLibrary> GetLibraryAsync();
    }
}
