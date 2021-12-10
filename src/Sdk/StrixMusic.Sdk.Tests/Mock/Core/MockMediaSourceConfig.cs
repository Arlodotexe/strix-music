using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.IO;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    public class MockMediaSourceConfig : IMediaSourceConfig
    {
        public MockMediaSourceConfig(string id, ICoreTrack track)
        {
            Id = id;
            Track = track;
        }

        public ICoreTrack Track { get; set; }

        public string Id { get; set; }

        public Uri? MediaSourceUri { get; set; }

        public Stream? FileStreamSource { get; set; }

        public string? FileStreamContentType { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}
