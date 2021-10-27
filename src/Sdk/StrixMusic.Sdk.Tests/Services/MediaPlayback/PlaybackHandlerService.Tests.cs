using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Services.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Services.MediaPlayback
{
    [TestClass]
    public class PlaybackHandlerServiceTest
    {
        private PlaybackHandlerService _handlerService;

        [TestInitialize]
        public void Setup()
        {
            // Create handler service, pass in mock audio player, mock IMediaSourceConfig, mock coretrack as needed.
        }

        [TestCleanup]
        public void Cleanup()
        {
            // dispose media players, handler services, tracks, etc.
        }

        public void ShuffleQueue_AddMoreOfMe()
        {
            // TODO
            // Create playback handler instance

            // Turning on shuffle
            // Make sure previous items are emptied
            // Make sure all items from previous and next exist in next after shuffle
            // Make sure shuffle actually shuffles (no sequential items)
            // Needs to complete within 800ms
            // etc

            // Turning off shuffle
            // Needs to restore the queue exactly how it was
            // Needs to complete within 800ms
            // etc

            // Both on / off
            // Make sure current item doesn't change when toggling shuffle.
        }
    }
}
