using System.Diagnostics.CodeAnalysis;

namespace StrixMusic.Sdk.Core.Data
{

    /// <summary>
    /// A device that controls playback of an audio player.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Nullable SourceCore in IDeviceBase is overridden by non-nullable SourceCore in ICoreMember.")]
    public interface ICoreDevice : ICoreMember, IDevice
    {
        /// <inheritdoc cref="ITrackCollection"/>
        public new ITrackCollection? PlaybackQueue { get; }
    }
}
