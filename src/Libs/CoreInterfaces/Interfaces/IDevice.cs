using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.Services.Settings.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents a device that a user can connect to for playback
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Identifies the device
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The displayed name of this device
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The source core which created the object.
        /// </summary>
        public ICore SourceCore { get; set; }

        /// <summary>
        /// If true, the device is currently active and playing audio.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// The currently playing collection.
        /// </summary>
        IPlayableCollectionBase PlaybackContext { get; set; }

        /// <summary>
        /// If the song is currently playing this will represent the time in milliseconds that the song is currently playing.
        /// </summary>
        long? Position { get; }

        /// <inheritdoc cref="PlaybackState"/>
        PlaybackState State { get; }

        /// <summary>
        /// True if the player is using a shuffled track list.
        /// </summary>
        /// <remarks>If null, the player cannot be shuffled.</remarks>
        bool? ShuffleState { get; set; }

        /// <inheritdoc cref="Enums.RepeatState"/>
        RepeatState RepeatState { get; set; }

        /// <summary>
        /// The volume of the device. If null, the volume cannot be changed.
        /// </summary>
        double? VolumePercent { get; set; }

        /// <inheritdoc cref="Enums.DeviceType" />
        DeviceType DeviceType { get; set; }

        /// <summary>
        /// The rate of the playback for the current track. If unknown, value is 1.
        /// </summary>
        double PlaybackSpeed { get; set; }

        /// <summary>
        /// Resume the device if in the state <see cref="Enums.PlaybackState.Paused"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Not a reserved keyword.")]
        void Resume();

        /// <summary>
        /// Pauses the device if in the state <see cref="Enums.PlaybackState.Playing"/>
        /// </summary>
        void Pause();

        /// <summary>
        /// Advances to the next track. If there is no next track, playback is paused.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Not a reserved keyword.")]
        void Next();

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        void Previous();

        /// <summary>
        /// Seeks the track to a given timestamp.
        /// </summary>
        /// <param name="position">Time in milliseconds to seek the song to.</param>
        /// <returns><see langword="true"/> if the <see cref="ITrack"/> was seeked to successfully, <see langword="false"/> otherwise.</returns>
        bool Seek(long position);

        /// <summary>
        /// Toggles shuffle on or off.
        /// </summary>
        void ToggleShuffle();

        /// <summary>
        /// Asks the device to toggle to the next repeat state.
        /// </summary>
        void ToggleRepeat();

        /// <summary>
        /// Switches to this device.
        /// </summary>
        void SwitchTo();
    }
}
