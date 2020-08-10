using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing a collection of playable items.
    /// </summary>
    /// <remarks>No <see langword="class"/> should ever directly implement this interface. The items in this collection, the count, and the method for getting them are defined in a child <see langword="interface" />.</remarks>
    public interface IPlayableCollectionBase : IPlayable
    {
        /// <summary>
        /// An external link related to the collection.
        /// </summary>
        Uri? Url { get; }

        /// <summary>
        /// Owner of the collection.
        /// </summary>
        IUserProfile? Owner { get; }
    }
}
