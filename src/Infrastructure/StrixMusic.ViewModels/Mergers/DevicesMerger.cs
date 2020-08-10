using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels
{
    public static partial class Mergers
    {
        /// <summary>
        /// Marges multiple <see cref="IRecentlyPlayed"/> into one.
        /// </summary>
        /// <param name="devices">The devices to merge.</param>
        /// <returns><inheritdoc cref="ILibrary"/></returns>
        public static IAsyncEnumerable<IDevice> MergeDevices(params IAsyncEnumerable<IDevice>[] devices)
        {
            // TODO
            // Note: Need to not include devices that are of DeviceType.Local, and whos source core doesn't match the core that is playing the current track.
            return devices.First();
        }
    }
}
