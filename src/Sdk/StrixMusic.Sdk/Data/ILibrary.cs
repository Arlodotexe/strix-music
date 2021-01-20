using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface ILibrary : ILibraryBase, IPlayableCollectionGroup, ISdkMember, IMerged<ICoreLibrary>
    {
    }
}