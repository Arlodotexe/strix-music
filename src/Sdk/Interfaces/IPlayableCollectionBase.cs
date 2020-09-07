using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Enums;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Interface representing a collection of playable items.
    /// </summary>
    /// <remarks>No <see langword="class"/> should ever directly implement this interface. The items in this collection, the count, and the method for getting them are defined in a child <see langword="interface" />.</remarks>
    public interface IPlayableCollectionBase : IPlayable
    {
    }
}
