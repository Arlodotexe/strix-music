using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    internal class MockCoreConfig : ICoreConfig
    {
        public MockCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        public IServiceProvider? Services { get; set; }

        public IReadOnlyList<AbstractUICollection> AbstractUIElements { get; set; } = new List<AbstractUICollection>();

        public MediaPlayerType PlaybackType { get; internal set; } = MediaPlayerType.None;

        public ICore SourceCore { get; set; }

        public event EventHandler? AbstractUIElementsChanged;

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
