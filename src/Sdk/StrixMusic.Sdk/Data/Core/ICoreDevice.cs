using System.Diagnostics.CodeAnalysis;

namespace StrixMusic.Sdk.Core.Data
{

    /// <summary>
    /// A device that controls playback of an audio player.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Nullable SourceCore in IDeviceBase is overridden by non-nullable SourceCore in ICoreMember.")]
    public interface ICoreDevice : IDevice, ICoreMember
    {
        /// <inheritdoc cref="ITrackCollection"/>
        public new ITrackCollection? PlaybackQueue { get; }
    }
}
