// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// Describes the type of device used for playback.
    /// </summary>
    /// <remarks>
    /// If the device type is anything but <see cref="Remote"/>, playback is expected to be handled outside of a core (by a playback handler in the SDK, for example).
    /// </remarks>
    public enum DeviceType
    {
        /// <summary>
        /// This device provides playback state for audio that is played on this device.
        /// </summary>
        Local,

        /// <summary>
        /// Audio is played back on a remote device, via the a third-party API.
        /// </summary>
        Remote,

        /// <summary>
        /// Audio is played back by the app, but is being streamed over Bluetooth.
        /// </summary>
        Bluetooth,

        /// <summary>
        /// Audio is played back by the app, but is cast to another device via some version of DLNA, such as Miracast, AllShare, ReadyMedia, etc.
        /// </summary>
        DLNA,

        /// <summary>
        /// Audio is being played back by the app, but is cast to another device via the Chromecast protocol.
        /// </summary>
        Chromecast,
    }
}
