using System.Diagnostics.CodeAnalysis;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A collection of <see cref="ITrack"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Nullable SourceCore in ITrackCollectionBase is overridden by non-nullable SourceCore in ICoreMember.")]
    public interface ITrackCollection : ITrackCollectionBase, IPlayableCollectionBase
    {
    }
}